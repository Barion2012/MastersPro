<?php

namespace App\Listeners;

use App\Events\OrderStatusChanged;
use Illuminate\Contracts\Queue\ShouldQueue;
use Illuminate\Queue\InteractsWithQueue;
use Mockery\Matcher\Not;
use App\Services\NotificationService;

class HandleOrderStatusChanged implements ShouldQueue
{
    protected $notificationService;

    public function __construct(NotificationService $notificationService)
    {
        $this->notificationService = $notificationService;
    }
    /**
     * Handle the event.
     */
    public function handle(OrderStatusChanged $event): void
    {
        switch ($event->order->status) {
            case 'search':
                $this->notificationService->createNotification([
                    'user_id' => $event->order->customer->user_id,
                    'type' => 'info',
                    'title' => 'Начат поиск исполнителей',
                    'message' => 'Пожалуйста, ожидайте назначения исполнителей.',
                    'url' => '/customer/order/'.$event->order->id.'/search'
                ]);
                break;
            case 'work':
                $this->notificationService->createNotification([
                    'user_id' => $event->order->customer->user_id,
                    'type' => 'info',
                    'title' => 'Исполнители назначены. Работа начата',
                    'message' => 'Исполнители назначены. Работа начата. Пожалуйста, следите за отчетами.',
                    'url' => '/customer/order/'.$event->order->id.'/detail'
                ]);
                break;
            case 'completed':
                $this->notificationService->createNotification([
                    'user_id' => $event->order->customer->user_id,
                    'type' => 'info',
                    'title' => 'Работа завершена',
                    'message' => 'Работа завершена. Спасибо за использование нашего сервиса.',
                    'url' => '/customer/order/'.$event->order->id.'/detail'
                ]);
                break;
        }
    }
}
