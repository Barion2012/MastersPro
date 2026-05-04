<?php

namespace App\Console\Commands;

use App\Models\Customer;
use App\Models\Profession;
use App\Models\ProfessionLevel;
use App\Models\User;
use App\Models\Worker;
use App\Models\WorkerProfession;
use Illuminate\Console\Command;
use Illuminate\Support\Facades\Hash;
use Illuminate\Support\Str;

/**
 * Artisan-команда для быстрого создания тестовых пользователей
 *
 * Использование:
 *   php artisan user:generate
 *
 * Позволяет создать:
 *   - Админа
 *   - Работника (с профилем Worker + привязкой к профессии)
 *   - Клиента (с профилем Customer + реквизитами ИП)
 */
class GenerateUser extends Command
{
    /**
     * Имя и синтаксис команды
     *
     * @var string
     */
    protected $signature = 'user:generate';

    /**
     * Описание команды (отображается в `php artisan`)
     *
     * @var string
     */
    protected $description = 'Generate a new user';

    /**
     * Основная логика выполнения команды
     *
     * @return void
     */
    public function handle()
    {
        // Интерактивный выбор роли пользователя
        $role = $this->choice('Выберите роль пользователя', ['admin', 'worker', 'customer'], 0);

        // Генерируем уникальный идентификатор для имени/email
        $uuid = uniqid();

        // Генерируем сложный случайный пароль (16 символов)
        $password = Str::random(16);

        // Создаём объект пользователя
        $user = new User();

        // Заполняем базовые поля
        $user->role = $role;
        $user->name = $role."#".$uuid;
        $user->email = $role."-".$uuid."@example.com";
        $user->password = Hash::make($password);
        $user->phone = '+7999'.rand(1000000, 9999999);
        $user->phone_verified_at = now();
        $user->save();  

        // В зависимости от роли — создаём дополнительные объекты
        switch ($role) {
            case 'admin':
                break;
            case 'worker':
                $this->generateWorker($user->id);
                break;
            case 'customer':
                $this->generateCustomer($user->id);
                break;
        }

        // Выводим информацию о созданном пользователе
        $this->info("Пользователь с ролью '{$role}' успешно создан.");
        $this->line("Email: {$user->email}");
        $this->line("Пароль: {$password}");
    }

    /**
     * Создаёт объект работника (Worker) и привязывает его к одной профессии
     *
     * @param int $userId
     * @return void
     */
    public function generateWorker($userId)
    {
        $worker = new Worker();

        // Основные данные работника
        $worker->user_id = $userId;
        $worker->status = 'check';
        $worker->citizenship = 'citizen';
        $worker->inn = '000000000000';
        $worker->snils = '000-000-000 00';
        $worker->address = 'г. Москва, ул. Пушкина, д. Колотушкина';
        $worker->passport_series = '0000';
        $worker->passport_number = '000000';
        $worker->passport_issued_by = 'ОВД Москвы';
        $worker->location = 'Москва';
        $worker->location_lat = '55.7558';
        $worker->location_lng = '37.6173';

        // Банковские реквизиты (заглушки)
        $worker->account_number = '00000000000000000000';
        $worker->bank_name = 'ПАО СБЕРБАНК';
        $worker->bank_bic = '000000000';
        $worker->bank_cor_account = '00000000000000000000';

        $worker->save();

        // Привязываем работника к первой доступной профессии
        $profession = Profession::first();

        WorkerProfession::create([
            'worker_id' => $worker->id,
            'profession_id' => $profession->id,
            'profession_level_id' => ProfessionLevel::where('profession_id', $profession->id)->first()->id,
        ]);
    }

    /**
     * Создаёт объект клиента (Customer) с реквизитами ИП
     *
     * @param int $userId
     * @return void
     */
    public function generateCustomer($userId)
    {
        $customer = new Customer();

        $customer->user_id = $userId;
        $customer->type = 'ip';
        $customer->beneficiary_id = uniqid('', true);

        // Массив с реквизитами и личными данными
        $customer->info = [
            'surname' => 'Иванов',
            'name' => 'Иван',
            'patronymic' => 'Иванович',
            'birthDate' => '01.01.1990',
            'seria' => '0000',
            'number' => '000000',
            'issuer' => 'ОВД Москвы',
            'issuerCode' => '000-000',
            'issueDate' => '01.01.2010',
            'inn' => '000000000000',
            'ogrnip' => '000000000000000',
            'postAddress' => 'г. Москва, ул. Пушкина, д. Колотушкина',
            'accountNumber' => '00000000000000000000',
            'bankBIC' => '000000000',
            'bankCorAccount' => '00000000000000000000',
            'bankName' => 'ПАО СБЕРБАНК',
        ];

        $customer->save();
    }
}