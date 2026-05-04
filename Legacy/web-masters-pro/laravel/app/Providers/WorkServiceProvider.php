<?php

namespace App\Providers;

use Illuminate\Support\ServiceProvider;
use App\Services\WorkService;

class WorkServiceProvider extends ServiceProvider
{
    /**
     * Register services.
     */
    public function register(): void
    {
        $this->app->bind(WorkService::class, function ($app) {
            return new WorkService();
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
