<?php

/**
 * Конфигурация интеграции со Сбербанком (СБЕР).
 *
 * Файл возвращает массив настроек, которые используются в сервисе SberService.
 * Все параметры берутся из .env
 *
 * Используется в:
 *  - RegisterController@registerCustomer() — для создания бенефициара
 *  - SberService — для HTTP-запросов к API Сбера
 *
 * @see \App\Services\SberService
 */
return [

    /**
     * Базовый URL API Сбера.
     *
     * Пример: https://api.sber.ru/v1/
     * Берётся из .env: SBER_API_URL
     *
     * @env SBER_API_URL
     */
    'api' => env('SBER_API_URL', ''),

    /**
     * Банковский идентификатор сервиса партнёра полученный от менеджера партнёра при регистрации приложения
     *
     * @env SBER_CLIENT_ID
     */
    'client_id' => env('SBER_CLIENT_ID', ''),

    /**
     * Client Secret — секретный ключ приложения.
     * Должен храниться в секрете, никогда не коммитить в Git.
     *
     * @env SBER_CLIENT_SECRET
     */
    'client_secret' => env('SBER_CLIENT_SECRET', ''),

    /**
     * Код для получения токена 
     *
     * @env SBER_CODE
     */
    'code' => env('SBER_CODE', ''),

    /**
     * ID номинального счёта (для выплат, бенефициаров и т.д.).
     * Указывается в запросах на создание бенефициара.
     *
     * @env SBER_NOMINAL_ID
     */
    'nominal_id' => env('SBER_NOMINAL_ID', ''),

    /**
     * Флаг отключения регистрации в СБЕР.
     *
     * Если true — пропускается вызов:
     *   $this->sberService->createBeneficiary()
     *   $this->sberService->syncCustomerBalance()
     *
     * @env SBER_REG_DISABLE
     * @default false
     */
    'reg_disable' => env('SBER_REG_DISABLE', false),

    /**
     * Флаг отключения платёжных операций в СБЕР.
     *
     * Если true — пропускается вызов:
     *   $this->sberService->createPayout()
     *
     * @env SBER_PAY_DISABLE
     * @default false
     */
    'pay_disable' => env('SBER_PAY_DISABLE', false),
];