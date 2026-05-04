<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    /**
     * Run the migrations.
     */
    public function up(): void
    {
        Schema::create('users', function (Blueprint $table) {
            $table->id();
            $table->string('name'); // Имя пользователя
            $table->string('email')->unique(); // Электронная почта
            $table->timestamp('email_verified_at')->nullable(); // Подтверждение электронной почты
            $table->string('phone')->unique(); // Номер телефона
            $table->timestamp('phone_verified_at')->nullable(); // Подтверждение номера телефона
            $table->string('password'); // Пароль
            $table->string('role')->default('user'); // Роль пользователя
            $table->rememberToken(); // Токен для запоминания сессии
            $table->timestamps();
        });

        Schema::create('password_reset_tokens', function (Blueprint $table) {
            $table->string('email')->primary(); // Электронная почта
            $table->string('token'); // Токен сброса пароля
            $table->timestamp('created_at')->nullable(); // Дата создания
        });

        Schema::create('sessions', function (Blueprint $table) {
            $table->string('id')->primary();
            $table->foreignId('user_id')->nullable()->index(); // Идентификатор пользователя
            $table->string('ip_address', 45)->nullable(); // IP-адрес
            $table->text('user_agent')->nullable(); // Информация о браузере
            $table->longText('payload'); // Данные сессии
            $table->integer('last_activity')->index(); // Время последней активности
        });
    }

    /**
     * Reverse the migrations.
     */
    public function down(): void
    {
        Schema::dropIfExists('users');
        Schema::dropIfExists('password_reset_tokens');
        Schema::dropIfExists('sessions');
    }
};
