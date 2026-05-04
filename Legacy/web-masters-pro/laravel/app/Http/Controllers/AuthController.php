<?php

namespace App\Http\Controllers;

use App\Models\VerificationCode;
use App\Services\NotisendService;
use App\Services\SberService;
use Carbon\Carbon;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Auth;
use Illuminate\Validation\ValidationException;
use App\Services\UserService;

/**
 * Контроллер авторизации и аутентификации пользователей.
 *
 * Обрабатывает:
 *  - Классический логин по email + пароль
 *  - Вход по SMS-коду (без пароля)
 *  - Отправку SMS-кода для входа
 *  - Выход из системы (logout)
 *
 * Использует Laravel Sanctum для выдачи API-токенов.
 */
class AuthController extends Controller {
    /** @var UserService Сервис для работы с пользователями (поиск, нормализация телефона, получение текущего пользователя) */
    protected $userService;
    /** @var NotisendService Сервис отправки SMS через Notisend */
    protected $notisendService;

    /**
     * Внедрение зависимостей через конструктор (DI).
     *
     * @param UserService       $userService
     * @param NotisendService   $notisendService
     */
    public function __construct(UserService $userService, NotisendService $notisendService) {
        $this->userService = $userService;
        $this->notisendService = $notisendService;
    }

    /**
     * Классический вход по email и паролю.
     *
     * Использует Auth::attempt() — проверяет хеш пароля.
     * При успехе:
     *  - создаёт Sanctum-токен
     *  - возвращает токен + данные пользователя (через UserService)
     *
     * @param Request $request
     * @return array
     * @throws ValidationException
     */
    public function login(Request $request) {
        // Попытка авторизации по email + password
        if (Auth::attempt($request->only('email', 'password'))) {
            return [
                'token' => Auth::user()->createToken('authentication')->plainTextToken,
                'user' => $this->userService->getCurrentUser(true)
            ];
        }

        // Ошибка авторизации
        throw ValidationException::withMessages([
            'email' => ['Почта или пароль не верны'],
        ]);
    }

    /**
     * Вход по SMS-коду (без пароля).
     *
     * Шаги:
     *  1. Валидация телефона и кода (4 цифры)
     *  2. Поиск пользователя по номеру
     *  3. Проверка существования и актуальности кода
     *  4. Проверка срока жизни кода (5 минут)
     *  5. Авторизация + удаление использованного кода
     *  6. Выдача токена
     *
     * @param Request $request
     * @return array
     * @throws ValidationException
     */
    public function loginBySmsCode(Request $request) {
        // Валидация формата телефона и кода
        $request->validate([
            'phone' => ['required', 'regex:/^(\+7|8)[\s\-()]*\d{3}[\s\-()]*\d{3}[\s\-()]*\d{2}[\s\-()]*\d{2}$/'],
            'code' => ['required', 'min:4', 'max:4']
        ], [
            'phone.required' => 'Введите номер телефона',
            'phone.regex' => 'Введите коректный номер телефона',
            'code.required' => 'Введите код из СМС',
            'code.min' => 'Код должен состоять из 4 символов',
            'code.max' => 'Код должен состоять из 4 символов'
        ]);

        // Поиск пользователя по оригинальному номеру
        $user = $this->userService->getUserByPhone($request->phone);
        // Поиск кода по нормализованному номеру
        $code = VerificationCode::where('phone', $this->userService->serializePhone($request->phone))->first();

        // Проверка: пользователь существует
        if (empty($user))
            $this->createValidationErrorResponse('Пользователь с таким номером не найден');

        // Проверка: код был отправлен
        if (empty($code))
            $this->createValidationErrorResponse('Код не был отправлен на номер');

        // Проверка: код совпадает
        if ($code->code != $request->code)
            $this->createValidationErrorResponse('Введен неверный код');

        // Проверка: код не просрочен (5 минут)
        if (Carbon::parse($code->code_sent_at)->addMinutes(5)->isPast())
            $this->createValidationErrorResponse('Введен неверный код');

        // Авторизация пользователя
        Auth::login($user);

        // Удаляем использованный код (одноразовый)
        $code->delete();

        // Возвращаем только токен (данные пользователя фронт получит отдельно)
        return [
            'token' => Auth::user()->createToken('authentication')->plainTextToken,
        ];
    }

    /**
     * Отправка SMS-кода для входа.
     *
     * Особенности:
     *  - Ограничение: не чаще 1 раза в 60 секунд
     *  - При отключенном Notisend (notisend.disable = true) — код фиксированный: 1111
     *  - Иначе — рандомный 4-значный
     *  - Отправка через NotisendService
     *
     * @param Request $request
     * @return void
     * @throws ValidationException
     */
    public function sendSmsCode(Request $request) {
        // Валидация телефона
        $request->validate([
            'phone' => ['required', 'regex:/^(\+7|8)[\s\-()]*\d{3}[\s\-()]*\d{3}[\s\-()]*\d{2}[\s\-()]*\d{2}$/']
        ], [
            'phone.required' => 'Введите номер телефона',
            'phone.regex' => 'Введите коректный номер телефона'
        ]);

        // Нормализация телефона
        $phone = $this->userService->serializePhone($request->phone);

        // Поиск существующего кода
        $verificationCode = VerificationCode::where('phone', $phone)->first();

        // Проверка частоты запросов
        if (!empty($verificationCode)) {
            if (!Carbon::parse($verificationCode->code_sent_at)->addSeconds(60)->isPast())
                $this->createValidationErrorResponse('Повторно код можно запросить через 60 секунд');
        }
        else 
            $verificationCode = new VerificationCode(); // Создаём новую запись, если ранее не было
       

        // Генерация кода
        if (config('notisend.voice_disable') && config('notisend.sms_disable'))
            $code = 1111; // Фиксированный код при отключённом Notisend
        else 
            $code = rand(1000, 9999); // Реальный рандом

        // Сохраняем код и метку времени
        $verificationCode->phone = $phone;
        $verificationCode->save();

        $verificationCode->update([
            'code' => $code,
            'code_sent_at' => now(),
            'phone' => $phone
        ]);

        // Отправка SMS
        $this->notisendService->sendAuthCode($phone, $code);

        // Ответ не возвращаем — фронт просто показывает "Код отправлен"
    }

    /**
     * Выход из системы (logout).
     *
     * Действия:
     *  - Удаление текущего токена пользователя
     *  - Auth::logout() — разлогинивает пользователя
     *  - invalidate() + regenerateToken() — защита от session fixation
     *
     * @param Request $request
     * @return void
     */
    public function logout(Request $request) {
        $request->user()?->currentAccessToken()?->delete();

        Auth::logout();
        
        $request->session()->invalidate();
        $request->session()->regenerateToken();
    }
}
