<?php

namespace App\Providers;

use App\Events\OrderStatusChanged;
use App\Events\WorkStatusUpdated;
use App\Listeners\HandleOrderStatusChanged;
use App\Listeners\HandleWorkStatusUpdated;
use Auth;
use Illuminate\Support\Facades\Log;
use Illuminate\Support\ServiceProvider;
use App\Models\User;
use Illuminate\Support\Facades\Gate;
use App\Models\Work;
use App\Observers\WorkObserver;
use App\Services\WorkService;
use App\Services\NotificationService;
use App\Services\FileService;
use Illuminate\Support\Facades\Event;

class AppServiceProvider extends ServiceProvider
{
    /**
     * Register any application services.
     */
    public function register(): void
    {
        //
    }

    /**
     * Bootstrap any application services.
     */
    public function boot(): void
    {
        Gate::define('viewPulse', function (?User $user) {
            if (empty($user))
                return false;
            
            return $user->checkPermission('App', 'pulse');
        });

        Work::observe(WorkObserver::class);

        $this->registerServices();
        //$this->registerEventListeners();
    }

    public function registerEventListeners(): void
    {
        Event::listen(
            WorkStatusUpdated::class,
            HandleWorkStatusUpdated::class,
        );

        Event::listen(
            OrderStatusChanged::class,
            HandleOrderStatusChanged::class,
        );
    }

    public function registerServices(): void
    {
        $this->app->singleton(WorkService::class, function ($app) {
            return new WorkService();
        });

        $this->app->singleton(NotificationService::class, function ($app) {
            return new NotificationService();
        });

        $this->app->singleton(FileService::class, function ($app) {
            return new FileService();
        });
    }
}
