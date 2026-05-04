<?php

namespace App\Providers;

use Illuminate\Support\ServiceProvider;
use App\Services\SberService;

class SberServiceProvider extends ServiceProvider
{
    /**
     * Register services.
     */
    public function register(): void
    {
        $this->app->bind(SberService::class, function ($app) {
            return new SberService();
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
