<?php

namespace App\Http\Controllers;

use App\Models\Customer;
use App\Models\ProfessionLevel;
use Illuminate\Http\Request;
use App\Services\UserService;
use App\Services\FileService;
use App\Models\VerificationCode;
use App\Models\Worker;
use App\Models\WorkerProfession;
use App\Services\NotisendService;
use App\Services\SberService;
use Illuminate\Support\Carbon;
use Illuminate\Support\Facades\Hash;
use Illuminate\Support\Str;
use Illuminate\Support\Facades\Validator;
use Illuminate\Validation\ValidationException;
use Illuminate\Support\Facades\Auth;

/**
 * Контроллер регистрации пользователей двух типов:
 *  - Worker (исполнитель)
 *  - Customer (заказчик)
 *
 * Все методы — API-эндпоинты, возвращают JSON.
 * Используется DI через конструктор (UserService, FileService, SberService).
 */

class RegisterController extends Controller
{
    /** @var UserService Сервис работы с пользователями (создание, поиск, нормализация телефона) */
    protected $userService;
    /** @var FileService Сервис загрузки и хранения файлов в S3 */
    protected $fileService;
    /** @var SberService Сервис интеграции с СБЕР (создание бенефициара, синхронизация баланса) */
    protected $sberService;
    /** @var NotisendService Сервис отправки SMS через Notisend */
    protected $notisendService;

    /**
     * Внедрение зависимостей.
     *
     * @param UserService $userService
     * @param FileService $fileService
     * @param SberService $sberService
     * @param NotisendService $notisendService
     */
    public function __construct(UserService $userService, FileService $fileService, SberService $sberService, NotisendService $notisendService) {
        $this->userService = $userService;
        $this->fileService = $fileService;
        $this->sberService = $sberService;
        $this->notisendService = $notisendService;
    }

    /**
     * Подтверждение номера телефона по SMS-коду.
     * Проверяет:
     *  - валидность телефона
     *  - наличие и корректность кода
     *  - отсутствие уже зарегистрированного пользователя
     *
     * @param Request $request
     * @return \Illuminate\Http\JsonResponse
     * @throws ValidationException
     */
    public function confirmPhone(Request $request) {
        // Валидация входных данных: телефон, код, согласия
        $request->validate([
            'phone' => ['required', 'regex:/^(\+7|8)[\s\-()]*\d{3}[\s\-()]*\d{3}[\s\-()]*\d{2}[\s\-()]*\d{2}$/'],
            'code' => ['required', 'min:4', 'max:4'],
            'agree1' => ['required'],
            'agree2' => ['required']
        ], [
            'phone.required' => 'Введите номер телефона',
            'phone.regex' => 'Введите коректный номер телефона',
            'code.required' => 'Введите код',
            'code.min' => 'Код должен состоять из 4 символов',
            'code.max' => 'Код должен состоять из 4 символов',
            'agree1.required' => 'Вы должны дать согласие',
            'agree2.required' => 'Вы должны дать согласие'
        ]);

        // Нормализация номера телефона через сервис (удаление пробелов, скобок и т.д.)
        $phone = $this->userService->serializePhone($request->phone);
        // Поиск записи с верификационным кодом
        $code = VerificationCode::where('phone', $phone)->first();
        // Поиск пользователя по оригинальному номеру
        $user = $this->userService->getUserByPhone($request->phone);

        // Проверка: пользователь уже существует
        if (!empty($user)) 
            $this->createValidationErrorResponse('Пользователь с таким номером телефона уже зарегистрирован');

        // Проверка: код не найден
        if (empty($code)) 
            $this->createValidationErrorResponse('Код не найден. Запросите код еще раз');

        // Проверка: код не совпадает
        if ($code->code != $request->code)
            $this->createValidationErrorResponse('Введен неверный код');

        // Успешное подтверждение
        return response(['status' => 'success', 'message' => 'Код подтвержден'], 200);
    }

    /**
     * Валидация данных формы работника (без сохранения).
     * Вызывает confirmPhone() для проверки телефона.
     * Правила валидации зависят от гражданства (citizen, eaeu, other).
     *
     * @param Request $request
     * @return \Illuminate\Http\JsonResponse
     * @throws ValidationException
     */
    public function confirmWorkerForm(Request $request) {
        $this->confirmPhone($request);

        // Общие сообщения об ошибках валидации
        $validatorErrors = [
            'phone.required' => 'Введите номер телефона',
            'phone.regex' => 'Введите коректный номер телефона',
            'name.required' => 'ФИО обязателен для заполнения',
            'name.min' => 'ФИО должно содержать минимум 3 символа',
            'email.required' => 'Почта обязательна для заполнения',
            'email.email' => 'Введите корректный адрес электронной почты',
            'name.min' => 'ФИО должно содержать минимум 3 символа',
            'citizenship.required' => 'Укажите гражданство',
            'inn.required' => 'ИНН обязателен для заполнения',
            'inn.digits' => 'ИНН должен содержать 12 цифр',
            'snils.required' => 'Поле СНИЛС обязательно для заполнения.',
            'snils.string' => 'Поле СНИЛС должно быть строкой.',
            'snils.regex' => 'СНИЛС должен соответствовать формату 123-456-789 01.',
            'address.required' => 'Адрес обязателен для заполнения',
            'passport_series.required' => 'Серия паспорта обязательна для заполнения',
            'passport_series.min' => 'Серия паспорта должна содержать 4 символа',
            'passport_series.max' => 'Серия паспорта должна содержать 4 символа',
            'passport_number.required' => 'Номер паспорта обязателен для заполнения',
            'passport_number.min' => 'Номер паспорта должен содержать 6 символов',
            'passport_number.max' => 'Номер паспорта должен содержать 6 символов',
            'passport_issued_by.required' => 'Поле "Кем выдан паспорт" обязательно для заполнения',
            'passport_scan.required' => 'Фото паспорта обязателен для загрузки',
            'passport_scan.file' => 'Фото паспорта должен быть файлом',
            'passport_scan.mimes' => 'Фото паспорта должно быть в формате JPG, PNG, PDF.',
            'passport_reg_scan.required' => 'Фото паспорта обязателен для загрузки',
            'passport_reg_scan.file' => 'Фото паспорта должен быть файлом',
            'passport_reg_scan.mimes' => 'Фото паспорта должно быть в формате JPG, PNG, PDF.',
            'passport_selfie.required' => 'Фото с паспортом обязательно для загрузки',
            'passport_selfie.file' => 'Фото с паспортом должно быть файлом',
            'passport_selfie.mimes' => 'Фото с паспортом должно быть в формате JPG, PNG, PDF.',
            'snils.required' => 'СНИЛС обязателен для заполнения',
            'snils.file' => 'СНИЛС должен быть файлом',
            'snils.mimes' => 'СНИЛС должен быть в формате JPG, PNG, PDF.',
            'migration_card.required' => 'Миграционная карта обязательна для загрузки',
            'migration_card.file' => 'Миграционная карта должна быть файлом',
            'migration_card.mimes' => 'Миграционная карта должна быть в формате JPG, PNG, PDF.',
            'patent.required' => 'Патент обязателен для загрузки',
            'patent.file' => 'Патент должен быть файлом',
            'patent.mimes' => 'Патент должен быть в формате JPG, PNG, PDF.',
            'patent_cheque.required' => 'Чек об оплате патента обязателен для загрузки',
            'patent_cheque.file' => 'Чек об оплате патента должен быть файлом',
            'dms.required' => 'Полис ДМС обязателен для загрузки',
            'dms.file' => 'Полис ДМС должен быть файлом',
            'dms.mimes' => 'Полис ДМС должен быть в формате JPG, PNG, PDF.',
            'bank_name.required' => 'Название банка обязательно для заполнения',
            'account_number.required' => 'Номер счета обязателен для заполнения',
            'bank_cor_account.required' => 'Корреспондентский счет обязателен для заполнения',
            'bank_bic.required' => 'БИК обязателен для заполнения',
        ];
        
        // Разные правила валидации в зависимости от гражданства
        switch ($request->input('citizenship')) {
            case 'citizen':
                $validator = Validator::make($request->all(), [
                    'phone' => ['required', 'regex:/^(\+7|8)[\s\-()]*\d{3}[\s\-()]*\d{3}[\s\-()]*\d{2}[\s\-()]*\d{2}$/'],
                    'name' => ['required', 'min:3'],
                    'email' => ['required', 'email', 'unique:users,email'],
                    'citizenship' => ['required'],
                    'inn' => ['required', 'string', 'digits:12'],
                    'snils' => ['required', 'string', 'regex:/^\d{3}-\d{3}-\d{3}\s\d{2}$/'],
                    //'address' => ['required'],
                    'passport_series' => ['required', 'min:4', 'max:4'],
                    'passport_number' => ['required', 'min:6', 'max:6'],
                    'passport_issued_by' => ['required'],
                    'passport_scan' => ['required', 'file', 'mimes:jpg,jpeg,png,pdf', 'max:20480'],
                    'passport_reg_scan' => ['required', 'file'], 'mimes:jpg,jpeg,png,pdf', 'max:20480',
                    'passport_selfie' => ['required', 'file', 'mimes:jpg,jpeg,png,pdf', 'max:20480'],
                    //'snils' => ['required', 'file'],
                    'bank_name' => 'required',
                    'account_number' => 'required',
                    'bank_cor_account' => 'required',
                    'bank_bic' => 'required',
                ], $validatorErrors);
                break;
            case 'eaeu':
                $validator = Validator::make($request->all(), [
                    'phone' => ['required', 'regex:/^(\+7|8)[\s\-()]*\d{3}[\s\-()]*\d{3}[\s\-()]*\d{2}[\s\-()]*\d{2}$/'],
                    'name' => ['required', 'min:3'],
                    'email' => ['required', 'email', 'unique:users,email'],
                    'citizenship' => ['required'],
                    'inn' => ['required', 'string', 'digits:12'],
                    'snils' => ['required', 'string', 'regex:/^\d{3}-\d{3}-\d{3}\s\d{2}$/'],
                    'passport_series' => ['required', 'min:4', 'max:4'],
                    'passport_number' => ['required', 'min:6', 'max:6'],
                    'passport_issued_by' => ['required'],
                    'passport_scan' => ['required', 'file', 'mimes:jpg,jpeg,png,pdf', 'max:20480'],
                    'passport_reg_scan' => ['required', 'file', 'mimes:jpg,jpeg,png,pdf', 'max:20480'],
                    'passport_selfie' => ['required', 'file', 'mimes:jpg,jpeg,png,pdf', 'max:20480'],
                    'migration_card' => ['required', 'file'], 'mimes:jpg,jpeg,png,pdf', 'max:20480',
                    //'snils' => ['required', 'file'],
                    'bank_name' => 'required',
                    'account_number' => 'required',
                    'bank_cor_account' => 'required',
                    'bank_bic' => 'required',
                ], $validatorErrors);
                break;
            case 'other':
                $validator = Validator::make($request->all(), [
                    'phone' => ['required', 'regex:/^(\+7|8)[\s\-()]*\d{3}[\s\-()]*\d{3}[\s\-()]*\d{2}[\s\-()]*\d{2}$/'],
                    'name' => ['required', 'min:3'],
                    'email' => ['required', 'email', 'unique:users,email'],
                    'citizenship' => ['required'],
                    'inn' => ['required', 'string', 'digits:12'],
                    'snils' => ['required', 'string', 'regex:/^\d{3}-\d{3}-\d{3}\s\d{2}$/'],
                    'address' => ['required'],
                    'passport_series' => ['required', 'min:4', 'max:4'],
                    'passport_number' => ['required', 'min:6', 'max:6'],
                    'passport_issued_by' => ['required'],
                    'passport_scan' => ['required', 'file', 'mimes:jpg,jpeg,png,pdf', 'max:20480'],
                    'passport_reg_scan' => ['required', 'file', 'mimes:jpg,jpeg,png,pdf', 'max:20480'],
                    'passport_selfie' => ['required', 'file', 'mimes:jpg,jpeg,png,pdf', 'max:20480'],
                    'migration_card' => ['required', 'file', 'mimes:jpg,jpeg,png,pdf', 'max:20480'],
                    //'snils' => ['required', 'file'],
                    'patent' => ['required', 'file', 'mimes:jpg,jpeg,png,pdf', 'max:20480'],
                    'patent_cheque' => ['required', 'file', 'mimes:jpg,jpeg,png,pdf', 'max:20480'],
                    'dms' => ['required', 'file', 'mimes:jpg,jpeg,png,pdf', 'max:20480'],
                    'bank_name' => 'required',
                    'account_number' => 'required',
                    'bank_cor_account' => 'required',
                    'bank_bic' => 'required',
                ], $validatorErrors);
                break;
        }

        // Если валидация не пройдена — бросаем исключение
        if ($validator->fails()) {
            throw new ValidationException($validator);
        }

        // Успешная валидация формы
        return response(['status' => 'success', 'message' => 'Форма не содержит ошибок'], 200);
    }

    /**
     * Полная регистрация работника.
     * Последовательно:
     *  1. confirmWorkerForm() — проверка формы
     *  2. Валидация локации и профессий
     *  3. Создание пользователя
     *  4. Создание записи Worker
     *  5. Загрузка файлов
     *  6. Привязка профессий
     *  7. Авторизация
     *
     * @param Request $request
     * @return \Illuminate\Http\JsonResponse
     */
    public function registerWorker(Request $request) {
        $this->confirmWorkerForm($request);

        // Валидация геолокации и массива профессий
        $validator = Validator::make($request->all(), [
            'location' => ['required', 'string'],
            'location_lat' => ['required', 'numeric'],
            'location_lng' => ['required', 'numeric'],
            'professions' => ['required', 'array'],
            'professions.*.profession_id' => ['required', 'exists:professions,id'],
            'professions.*.profession_level_id' => ['required', 'exists:profession_levels,id'],
        ], [
            'location.required' => 'Локация обязательна для заполнения',
            'location.string' => 'Локация должна быть строкой',
            'location_lat.required' => 'Поле location_lat обязательно для заполнения',
            'location_lat.numeric' => 'Поле location_lat должно быть числом',
            'location_lng.required' => 'Поле location_lng обязательно для заполнения',
            'location_lng.numeric' => 'Поле location_lng должно быть числом',
            'professions.required' => 'Профессии обязательны для заполнения',
            'professions.array' => 'Профессии должны быть массивом',
            'professions.*.profession_id.required' => 'Профессия обязательна для заполнения',
            'professions.*.profession_id.exists' => 'Указанная профессия не существует',
            'professions.*.profession_level_id.required' => 'Уровень профессии обязательн для заполнения',
            'professions.*.profession_level_id.exists' => 'Указанный уровень профессии не существует',
        ]);

        // Если валидация не пройдена — бросаем исключение
        if ($validator->fails()) {
            throw new ValidationException($validator);
        }

        // Создание пользователя через сервис
        $user = $this->userService->createUser([
            'name' => $request->input('name'),
            'email' => $request->input('email'),
            'phone' => $request->input('phone'),
            'phone_verified_at' => now(),
            'password' => Hash::make(Str::random(16)),
            'role' => 'worker'
        ]);

        // Создание записи работника
        $worker = new Worker();

        $worker->user_id = $user->id;
        $worker->status = 'check';
        $worker->citizenship = $request->input('citizenship');
        $worker->inn = $request->input('inn');
        $worker->snils = $request->input('snils');
        $worker->address = empty($request->input('address')) ? '' : $request->input('address');
        $worker->passport_series = $request->input('passport_series');
        $worker->passport_number = $request->input('passport_number');
        $worker->passport_issued_by = $request->input('passport_issued_by');
        $worker->location = $request->input('location');
        $worker->location_lat = $request->input('location_lat');
        $worker->location_lng = $request->input('location_lng');
        $worker->account_number = $request->input('account_number');
        $worker->bank_bic = $request->input('bank_bic');
        $worker->bank_cor_account = $request->input('bank_cor_account');
        $worker->bank_name = $request->input('bank_name');

        $worker->save();

        // Загрузка документов в зависимости от гражданства
        try {
            switch ($request->input('citizenship')) {
                case 'citizen':
                    $this->fileService->storeFile('Worker', $worker->id, 'passport_scan', $request->file('passport_scan'), 'tws3_private');
                    $this->fileService->storeFile('Worker', $worker->id, 'passport_reg_scan', $request->file('passport_reg_scan'), 'tws3_private');
                    $this->fileService->storeFile('Worker', $worker->id, 'passport_selfie', $request->file('passport_selfie'), 'tws3_private');
                    //$this->fileService->storeFile('Worker', $worker->id, 'snils', $request->file('snils'), 'tws3_private');
                    break;
                case 'eaeu':
                    $this->fileService->storeFile('Worker', $worker->id, 'passport_scan', $request->file('passport_scan'), 'tws3_private');
                    $this->fileService->storeFile('Worker', $worker->id, 'passport_reg_scan', $request->file('passport_reg_scan'), 'tws3_private');
                    $this->fileService->storeFile('Worker', $worker->id, 'passport_selfie', $request->file('passport_selfie'), 'tws3_private');
                    $this->fileService->storeFile('Worker', $worker->id, 'migration_card', $request->file('migration_card'), 'tws3_private');
                    //$this->fileService->storeFile('Worker', $worker->id, 'snils', $request->file('snils'), 'tws3_private');
                    break;
                case 'other':
                    $this->fileService->storeFile('Worker', $worker->id, 'passport_scan', $request->file('passport_scan'), 'tws3_private');
                    $this->fileService->storeFile('Worker', $worker->id, 'passport_reg_scan', $request->file('passport_reg_scan'), 'tws3_private');
                    $this->fileService->storeFile('Worker', $worker->id, 'passport_selfie', $request->file('passport_selfie'), 'tws3_private');
                    $this->fileService->storeFile('Worker', $worker->id, 'migration_card', $request->file('migration_card'), 'tws3_private');
                    //$this->fileService->storeFile('Worker', $worker->id, 'snils', $request->file('snils'), 'tws3_private');
                    $this->fileService->storeFile('Worker', $worker->id, 'patent', $request->file('patent'), 'tws3_private');
                    $this->fileService->storeFile('Worker', $worker->id, 'patent_cheque', $request->file('patent_cheque'), 'tws3_private');
                    $this->fileService->storeFile('Worker', $worker->id, 'dms', $request->file('dms'), 'tws3_private');
                    break;
            }
        }
        catch (\Exception $e) {
            // Откат: удаляем пользователя и работника при ошибке загрузки
            $user->delete();
            $worker->delete();
            $this->createValidationErrorResponse('Непредвиденная ошибка. Попробуйте позже');
        }

        // Привязка профессий. Уровень берется из первой записи в ProfessionLevel по profession_id. В системе есть реализация уровней профессий, которые заказчик препочел скрыть и все свести временно к одной профессии. 
        foreach($request->input('professions', []) as $profession) {
            WorkerProfession::create([
                'worker_id' => $worker->id,
                'profession_id' => $profession['profession_id'],
                //'profession_level_id' => $profession['profession_level_id'],
                'profession_level_id' => ProfessionLevel::where('profession_id', $profession['profession_id'])->first()->id,
            ]);
        }

        // Авторизация пользователя
        Auth::login($user);

        // Успешная регистрация
        return response(['status' => 'success', 'message' => 'Работник успешно зарегистрирован'], 200);
    }

    /**
     * Регистрация заказчика (ИП, ООО, АО, ПАО).
     * Включает:
     *  - Подтверждение телефона
     *  - Валидацию по типу организации
     *  - Создание пользователя
     *  - Сохранение данных в Customer
     *  - Загрузку файлов
     *  - Регистрацию в СБЕР 
     *
     * @param Request $request
     * @return \Illuminate\Http\JsonResponse
     */
    public function registerCustomer(Request $request) {
        $this->confirmPhone($request);

        // Сообщения об ошибках валидации
        $validatorErrors = [
            'phone.required' => 'Введите номер телефона',
            'phone.regex' => 'Введите коректный номер телефона',
            'surname.required' => 'Фамилия обязательна для заполнения.',
            'surname.min' => 'Фамилия должна содержать минимум 3 символа.',
            'surname.max' => 'Фамилия не должна превышать 50 символов.',
            'name.required' => 'Имя обязательно для заполнения.',
            'name.min' => 'Имя должно содержать минимум 3 символа.',
            'name.max' => 'Имя не должно превышать 50 символов.',
            'patronymic.required' => 'Отчество обязательно для заполнения.',
            'patronymic.min' => 'Отчество должно содержать минимум 3 символа.',
            'patronymic.max' => 'Отчество не должно превышать 50 символов.',
            'email.required' => 'Email обязателен для заполнения.',
            'email.email' => 'Введите корректный email.',
            'email.max' => 'Email не должен превышать 255 символов.',
            'birthDate.required' => 'Дата рождения обязательна для заполнения.',
            'birthDate.date' => 'Дата рождения не является датой.',
            'passport_scan.required' => 'Фото паспорта обязательно для загрузки.',
            'passport_scan.file' => 'Фото паспорта должно быть файлом.',
            'passport_scan.mimes' => 'Фото паспорта должно быть в формате JPG, PNG, PDF.',
            'passport_scan.max' => 'Размер фото паспорта не должен превышать 5 МБ.',
            'passport_selfie.required' => 'Фото с паспортом обязательно для загрузки',
            'passport_selfie.file' => 'Фото с паспортом должно быть файлом',
            'passport_scan.mimes' => 'Фото паспорта должно быть в формате JPG, PNG, PDF.',
            'passport_scan.max' => 'Размер фото паспорта не должен превышать 5 МБ.',
            'seria.required' => 'Серия паспорта обязательна для заполнения.',
            'seria.digits' => 'Серия паспорта должна содержать ровно 4 цифры.',
            'number.required' => 'Номер паспорта обязателен для заполнения.',
            'number.digits' => 'Номер паспорта должен содержать ровно 6 цифр.',
            'issuer.required' => 'Кем выдан паспорт обязателен для заполнения.',
            'issuer.min' => 'Кем выдан паспорт должен содержать минимум 3 символа.',
            'issuer.max' => 'Кем выдан паспорт не должен превышать 150 символов.',
            'issuerDate.required' => 'Дата выдачи паспорта обязательна для заполнения.',
            'issuerDate.date' => 'Дата выдачи паспорта не является датой.',
            'issuerCode.required' => 'Код подразделения обязателен для заполнения.',
            'issuerCode.regex' => 'Код подразделения должен быть в формате ###-###.',
            'inn.required' => 'ИНН обязателен для заполнения.',
            'inn.digits' => 'ИНН должен содержать ровно 12 цифр для ИП и 10 для ООО.',
            'accountNumber.required' => 'Расчётный счёт обязателен для заполнения.',
            'accountNumber.digits_between' => 'Расчётный счёт должен содержать от 20 до 25 цифр.',
            'bankBIC.required' => 'БИК обязателен для заполнения.',
            'bankBIC.digits' => 'БИК должен содержать ровно 9 цифр.',
            'bankCorAccount.required' => 'Корреспондентский счёт обязателен для заполнения.',
            'bankCorAccount.digits' => 'Корреспондентский счёт должен содержать ровно 20 цифр.',
            'bankName.required' => 'Наименование банка обязательно для заполнения.',
            'bankName.min' => 'Наименование банка должно содержать минимум 3 символа.',
            'bankName.max' => 'Наименование банка не должно превышать 50 символов.',
            'ogrnip.required' => 'ОГРНИП обязателен для заполнения.',
            'ogrnip.digits' => 'ОГРНИП должен содержать ровно 15 цифр.',
            'ogrnip.regex' => 'ОГРНИП должен начинаться с 3 и содержать 15 цифр.',
            'postAddress.required' => 'Юридический адрес обязателен для заполнения.',
            'postAddress.max' => 'Юридический адрес не должен превышать 250 символов.',
            'legalAddress.required' => 'Юридический адрес обязателен для заполнения.',
            'legalAddress.max' => 'Юридический адрес не должен превышать 250 символов.',
            'ogrn.required' => 'ОГРН обязателен для заполнения.',
            'ogrn.digits' => 'ОГРН должен содержать ровно 13 цифр.',
            'kpp.required' => 'КПП обязателен для заполнения.',
            'kpp.digits' => 'КПП должен содержать ровно 9 цифр.',
            'orgName.required' => 'Наименование организации обязательно для заполнения.',
            'orgName.min' => 'Наименование организации должно содержать минимум 3 символа.',
            'orgName.max' => 'Наименование организации не должно превышать 160 символов.',
        ];

        // Валидация по типу организации
        switch ($request->input('type')) {
            case 'ip':
                $validator = Validator::make($request->all(), [
                    'phone' => ['required', 'regex:/^(\+7|8)[\s\-()]*\d{3}[\s\-()]*\d{3}[\s\-()]*\d{2}[\s\-()]*\d{2}$/'],
                    'surname' => ['required', 'string', 'min:3', 'max:50'],
                    'name' => ['required', 'string', 'min:3', 'max:50'],
                    'patronymic' => ['required', 'string', 'min:3', 'max:50'],
                    'email' => ['required', 'string', 'email', 'max:255'],
                    'birthDate' => ['required', 'date'],
                    'passport_scan' => ['required', 'file', 'mimes:jpg,jpeg,png,pdf', 'max:20480'],
                    'passport_selfie' => ['required', 'file', 'mimes:jpg,jpeg,png,pdf', 'max:20480'],
                    'seria' => ['required', 'string', 'digits:4'],
                    'number' => ['required', 'string', 'digits:6'],
                    'issuer' => ['required', 'string', 'min:3', 'max:150'],
                    'issuerCode' => ['required', 'string', 'regex:/^\d{3}-\d{3}$/'],
                    'issueDate' => ['required', 'date'],
                    'inn' => ['required', 'string', 'digits:12'],
                    'accountNumber' => ['required', 'string', 'digits_between:20,25'],
                    'bankBIC' => ['required', 'string', 'digits:9'],
                    'bankCorAccount' => ['required', 'string', 'digits:20'],
                    'bankName' => ['required', 'string', 'min:3', 'max:50'],
                    'ogrnip' => ['required', 'string', 'digits:15', 'regex:/^3[0-9]{14}$/'],
                    'postAddress' => ['required', 'string', 'max:250'],
                ], $validatorErrors);

                $username = $request->input('surname').' '.$request->input('name').' '.$request->input('patronymic');
                break;
            case 'ooo':
                $validator = Validator::make($request->all(), [
                    'phone' => ['required', 'regex:/^(\+7|8)[\s\-()]*\d{3}[\s\-()]*\d{3}[\s\-()]*\d{2}[\s\-()]*\d{2}$/'],
                    'ogrn' => ['required', 'string', 'digits:13'],
                    'inn' => ['required', 'string', 'digits:10'],
                    'kpp' => ['required', 'string', 'digits:9'],
                    'orgName' => ['required', 'string', 'min:3', 'max:160'],
                    'accountNumber' => ['required', 'string', 'digits_between:20,25'],
                    'bankBIC' => ['required', 'string', 'digits:9'],
                    'bankCorAccount' => ['required', 'string', 'digits:20'],
                    'bankName' => ['required', 'string', 'min:3', 'max:50'],
                    'legalAddress' => ['required', 'string', 'max:250'],
                    'email' => ['required', 'string', 'email', 'max:255'],
                ], $validatorErrors);
                
                $username = $request->input('orgName');
                break;
            case 'ao':
                $validator = Validator::make($request->all(), [
                    'phone' => ['required', 'regex:/^(\+7|8)[\s\-()]*\d{3}[\s\-()]*\d{3}[\s\-()]*\d{2}[\s\-()]*\d{2}$/'],
                    'ogrn' => ['required', 'string', 'digits:13'],
                    'inn' => ['required', 'string', 'digits:10'],
                    'kpp' => ['required', 'string', 'digits:9'],
                    'orgName' => ['required', 'string', 'min:3', 'max:160'],
                    'accountNumber' => ['required', 'string', 'digits_between:20,25'],
                    'bankBIC' => ['required', 'string', 'digits:9'],
                    'bankCorAccount' => ['required', 'string', 'digits:20'],
                    'bankName' => ['required', 'string', 'min:3', 'max:50'],
                    'legalAddress' => ['required', 'string', 'max:250'],
                    'email' => ['required', 'string', 'email', 'max:255'],
                ], $validatorErrors);
                
                $username = $request->input('orgName');
                break;
            case 'pao':
                $validator = Validator::make($request->all(), [
                    'phone' => ['required', 'regex:/^(\+7|8)[\s\-()]*\d{3}[\s\-()]*\d{3}[\s\-()]*\d{2}[\s\-()]*\d{2}$/'],
                    'ogrn' => ['required', 'string', 'digits:13'],
                    'inn' => ['required', 'string', 'digits:10'],
                    'kpp' => ['required', 'string', 'digits:9'],
                    'orgName' => ['required', 'string', 'min:3', 'max:160'],
                    'accountNumber' => ['required', 'string', 'digits_between:20,25'],
                    'bankBIC' => ['required', 'string', 'digits:9'],
                    'bankCorAccount' => ['required', 'string', 'digits:20'],
                    'bankName' => ['required', 'string', 'min:3', 'max:50'],
                    'legalAddress' => ['required', 'string', 'max:250'],
                    'email' => ['required', 'string', 'email', 'max:255'],
                ], $validatorErrors);
                
                $username = $this->replaceQuotes($request->input('orgName'));
                break;
        }

        // Если валидация не пройдена — бросаем исключение
        if ($validator->fails()) {
            throw new ValidationException($validator);
        }

        // Создание пользователя
        $user = $this->userService->createUser([
            'name' => $username,
            'email' => $request->input('email'),
            'phone' => $request->input('phone'),
            'phone_verified_at' => now(),
            'password' => Hash::make(Str::random(16)),
            'role' => 'customer'
        ]);

        // Сохранение данных заказчика
         switch ($request->input('type')) {
            case 'ip':
                $customer = new Customer();
                $customer->user_id = $user->id;
                $customer->type = 'ip';
                $customer->beneficiary_id = uniqid('', true); // ЗАГЛУШКА
                $customer->info = [
                    'surname' => $request->input('surname'),
                    'name' => $request->input('name'),
                    'patronymic' => $request->input('patronymic'),
                    'birthDate' => $request->input('birthDate'),
                    'seria' => $request->input('seria'),
                    'number' => $request->input('number'),
                    'issuer' => $request->input('issuer'),
                    'issuerCode' => $request->input('issuerCode'),
                    'issueDate' => $request->input('issueDate'),
                    'inn' => $request->input('inn'),
                    'ogrnip' => $request->input('ogrnip'),
                    'postAddress' => $request->input('postAddress'),
                    'accountNumber' => $request->input('accountNumber'),
                    'bankBIC' => $request->input('bankBIC'),
                    'bankCorAccount' => $request->input('bankCorAccount'),
                    'bankName' => $this->replaceQuotes($request->input('bankName')),
                ];
                $customer->save();

                try {
                    $this->fileService->storeFile('Customer', $customer->id, 'passport_scan', $request->file('passport_scan'), 'tws3_private');
                    $this->fileService->storeFile('Customer', $customer->id, 'passport_scan', $request->file('passport_selfie'), 'tws3_private');
                    //$this->fileService->storeFile('Customer', $customer->id, 'certificate', $request->file('certificate'), 'tws3_private');
                }
                catch (\Exception $e) {
                    $user->delete();
                    $this->createValidationErrorResponse('Непредвиденная ошибка. Попробуйте позже');
                }

                break;
            case 'ooo': 
                $customer = new Customer();
                $customer->user_id = $user->id;
                $customer->type = 'ooo';
                $customer->beneficiary_id = uniqid('', true); // ЗАГЛУШКА
                $customer->info = [
                    'ogrn' => $request->input('ogrn'),
                    'inn' => $request->input('inn'),
                    'kpp' => $request->input('kpp'),
                    'orgName' => $this->replaceQuotes($request->input('orgName')),
                    'accountNumber' => $request->input('accountNumber'),
                    'bankBIC' => $request->input('bankBIC'),
                    'bankCorAccount' => $request->input('bankCorAccount'),
                    'bankName' => $this->replaceQuotes($request->input('bankName')),
                    'legalAddress' => $this->replaceQuotes($request->input('legalAddress')),
                ];
                $customer->save();
                break;
            case 'pao': 
                $customer = new Customer();
                $customer->user_id = $user->id;
                $customer->type = 'pao';
                $customer->beneficiary_id = uniqid('', true); // ЗАГЛУШКА
                $customer->info = [
                    'ogrn' => $request->input('ogrn'),
                    'inn' => $request->input('inn'),
                    'kpp' => $request->input('kpp'),
                    'orgName' => $this->replaceQuotes($request->input('orgName')),
                    'accountNumber' => $request->input('accountNumber'),
                    'bankBIC' => $request->input('bankBIC'),
                    'bankCorAccount' => $request->input('bankCorAccount'),
                    'bankName' => $this->replaceQuotes($request->input('bankName')),
                    'legalAddress' => $this->replaceQuotes($request->input('legalAddress')),
                ];
                $customer->save();
                break;
            case 'ao': 
                $customer = new Customer();
                $customer->user_id = $user->id;
                $customer->type = 'ao';
                $customer->beneficiary_id = uniqid('', true); // ЗАГЛУШКА
                $customer->info = [
                    'ogrn' => $request->input('ogrn'),
                    'inn' => $request->input('inn'),
                    'kpp' => $request->input('kpp'),
                    'orgName' => $this->replaceQuotes($request->input('orgName')),
                    'accountNumber' => $request->input('accountNumber'),
                    'bankBIC' => $request->input('bankBIC'),
                    'bankCorAccount' => $request->input('bankCorAccount'),
                    'bankName' => $this->replaceQuotes($request->input('bankName')),
                    'legalAddress' => $this->replaceQuotes($request->input('legalAddress')),
                ];
                $customer->save();
                break;
        }

        // Интеграция с СБЕР (если включена)
        if (!config('sber.reg_disable')) {
            try {
                $this->sberService->createBeneficiary($customer->id);
                $this->sberService->syncCustomerBalance($customer->id);
            }
            catch(\Exception $e) {
                $user->delete();
                $this->createValidationErrorResponse('Ошибка регистрации покупателя в торговой площадке. Проверьте правильность ИНН и платежных реквизитов.');
            }
        }

        // Авторизация пользователя
        Auth::login($user);

        // Успешная регистрация
        return response(['status' => 'success', 'message' => 'Заказчик успешно зарегистрирован'], 200);
    }

    /**
     * Валидация массива профессий.
     * Проверяет:
     *  - наличие массива
     *  - существование profession_id и profession_level_id
     *  - уникальность profession_id
     *
     * @param Request $request
     * @return void
     * @throws ValidationException
     */
    public function validateProfessionArray($request) {
        $validator = Validator::make($request->all(), [
            'professions' => ['required', 'array'],
            'professions.*.profession_id' => ['required', 'exists:professions,id'],
            'professions.*.profession_level_id' => ['required', 'exists:profession_levels,id'],
        ], [
            'professions.required' => 'Профессии обязательны для заполнения',
            'professions.array' => 'Профессии должны быть массивом',
            'professions.*.profession_id.required' => 'Профессия обязательна для заполнения',
            'professions.*.profession_id.exists' => 'Указанная профессия не существует',
            'professions.*.profession_level_id.required' => 'Уровень профессии обязательн для заполнения',
            'professions.*.profession_level_id.exists' => 'Указанный уровень профессии не существует',
        ]);

        if ($validator->fails()) {
            throw new ValidationException($validator);
        }

        $professionIds = array_column($request->input('professions', []), 'profession_id');
        if (count($professionIds) !== count(array_unique($professionIds))) {
            $validator = Validator::make([], []);
            $validator->errors()->add('professions', 'Поле profession_id должно быть уникальным в массиве professions');
            throw new ValidationException($validator);
        }
    }

    /**
     * Отправка SMS-кода для подтверждения телефона.
     * Ограничение: повторная отправка через 60 секунд.
     * Код пока фиксированный: 111111 (для тестирования).
     *
     * @param Request $request
     * @return void
     */
    public function sendSmsCode(Request $request) {
        $request->validate([
            'phone' => ['required', 'regex:/^(\+7|8)[\s\-()]*\d{3}[\s\-()]*\d{3}[\s\-()]*\d{2}[\s\-()]*\d{2}$/'],
            'agree1' => ['required', 'accepted'],
            'agree2' => ['required', 'accepted'],
            'agree3' => ['required', 'accepted'],
        ], [
            'phone.required' => 'Введите номер телефона',
            'phone.regex' => 'Введите коректный номер телефона',
            'agree1.required' => 'Вы должны дать согласие',
            'agree1.accepted' => 'Вы должны дать согласие',
            'agree2.required' => 'Вы должны дать согласие',
            'agree2.accepted' => 'Вы должны дать согласие',
            'agree3.required' => 'Вы должны дать согласие',
            'agree3.accepted' => 'Вы должны дать согласие',
        ]);

        $phone = $this->userService->serializePhone($request->phone);

        $verificationCode = VerificationCode::where('phone', $phone)->first();

        // Проверка интервала между запросами
        if (!empty($verificationCode)) {
            if (!Carbon::parse($verificationCode->code_sent_at)->addSeconds(60)->isPast())
                $this->createValidationErrorResponse('Повторно код можно запросить через 60 секунд');
        }
        else
            $verificationCode = new VerificationCode();

        if (config('notisend.voice_disable') && config('notisend.sms_disable'))
            $code = 1111;
        else 
            $code = rand(1000, 9999);

        $verificationCode->phone = $phone;
        $verificationCode->save();

        $verificationCode->update([
            'code' => $code,
            'code_sent_at' => now(),
            'phone' => $phone
        ]);

        // Отправка SMS
        $this->notisendService->sendAuthCode($phone, $code);
    }
}