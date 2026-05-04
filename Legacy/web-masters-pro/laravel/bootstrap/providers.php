<?php

/* Список провайдеров приложения */

return [
    App\Providers\AppServiceProvider::class, // Базовый сервис-провайдер приложения
    App\Providers\FileServiceProvider::class, // Сервис для работы с файлами (загрузка, хранение)
    App\Providers\NotificationServiceProvider::class, // Сервис для отправки уведомлений 
    App\Providers\NotisendServiceProvider::class, // Сервис для отправки SMS через Notisend
    App\Providers\SberServiceProvider::class, // Сервис для работы с Сбербанком
    App\Providers\TickerServiceProvider::class, // Сервис для работы с тикетами
    App\Providers\UserServiceProvider::class, // Сервис для работы с пользователями
    App\Providers\WorkServiceProvider::class, // Сервис для работы с заказами и работами
];
