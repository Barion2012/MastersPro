<?php

namespace App\Providers;

use Illuminate\Support\ServiceProvider;
use App\Services\FileService;

class FileServiceProvider extends ServiceProvider
{
    /**
     * Register services.
     */
    public function register(): void
    {
        $this->app->bind(FileService::class, function ($app) {
            return new FileService();
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
