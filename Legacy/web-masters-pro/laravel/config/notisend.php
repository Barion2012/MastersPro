<?php 

/**
 * Конфигурация интеграции с Notisend (SMS + голосовые вызовы).
 *
 * Файл возвращает массив настроек, которые используются в сервисе NotisendService.
 * Все параметры берутся из .env
 *
 * Используется в:
 *  - \App\Services\NotisendService
 *
 * @see \App\Services\NotisendService
 */

return [
    /**
     * Базовый URL API Notisend.
     *
     * Пример: https://api.notisend.ru/v1
     * Обязательно с протоколом и без слеша в конце.
     *
     * @env NOTISEND_API_URL
     */
    'api_url' => env('NOTISEND_API_URL'),

    /**
     * Идентификатор проекта для голосовых вызовов.
     * Выдаётся в личном кабинете Notisend.
     *
     * @env NOTISEND_VOICE_PROJECT
     */
    'voice_project' => env('NOTISEND_VOICE_PROJECT'),

    /**
     * Секретный ключ для подписи голосовых запросов.
     * Никогда не коммитить в Git!
     *
     * @env NOTISEND_VOICE_KEY
     */
    'voice_key' => env('NOTISEND_VOICE_KEY'),

    /**
     * Отключение голосового канала.
     *
     * Если true — sendAuthCode() не будет отправлять голосовые звонки.
     * SMS при этом работает (если sms_disable = false).
     *
     * @env NOTISEND_VOICE_DISABLE
     * @default false
     */
    'voice_disable' => env('NOTISEND_VOICE_DISABLE', false),

    /**
     * Идентификатор проекта для SMS.
     * Выдаётся в личном кабинете Notisend.
     *
     * @env NOTISEND_SMS_PROJECT
     */
    'sms_project' => env('NOTISEND_SMS_PROJECT'),

    /**
     * Секретный ключ для подписи SMS-запросов.
     * Никогда не коммитить в Git!
     *
     * @env NOTISEND_SMS_KEY
     */
    'sms_key' => env('NOTISEND_SMS_KEY'),

    /**
     * Отключение SMS-канала.
     *
     * Если true — sendAuthCode() не будет отправлять SMS.
     * Голосовой канал при этом работает (если voice_disable = false).
     *
     * @env NOTISEND_SMS_DISABLE
     * @default false
     */
    'sms_disable' => env('NOTISEND_SMS_DISABLE', false),
];
