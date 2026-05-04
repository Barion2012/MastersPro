<?php

namespace App\Providers;

use Illuminate\Support\ServiceProvider;
use App\Services\TicketService;

class TickerServiceProvider extends ServiceProvider
{
    /**
     * Register services.
     */
    public function register(): void
    {
        $this->app->bind(TicketService::class, function ($app) {
            return new TicketService();
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
