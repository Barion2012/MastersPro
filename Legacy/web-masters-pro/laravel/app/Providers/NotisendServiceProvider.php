<?php

namespace App\Providers;

use Illuminate\Support\ServiceProvider;
use App\Services\NotisendService;

/**
 * Сервис-провайдер для регистрации NotisendService
 *
 * @see \App\Services\NotisendService
 * @see config/app.php → 'providers' => [..., NotisendServiceProvider::class]
 */

class NotisendServiceProvider extends ServiceProvider
{
    /**
     * Регистрация сервиса
     *
     * @return void
     */
    public function register(): void
    {
        $this->app->bind(NotisendService::class, function ($app) {
            return new NotisendService();
        });
    }

    /**
     * Bootstrap services.
     */
    public function boot(): void
    {
        //
    }
}
