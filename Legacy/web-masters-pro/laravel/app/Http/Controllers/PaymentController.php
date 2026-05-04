<?php

namespace App\Http\Controllers;

use App\Models\PaymentRequest;
use App\Services\NotisendService;
use App\Services\SberService;
use Carbon\Carbon;
use Illuminate\Http\Request;

/**
 * Контроллер для работы с платежами и интеграцией с СБЕР
 *
 * Основные функции:
 *  - Получение списка всех платёжных запросов (для админки)
 *  - Тестовые методы для отладки интеграции со Сбером и Notisend
 *
 * Использует:
 *  - SberService — работа с API Сбера
 *  - NotisendService — отправка SMS-кодов аутентификации
 */
class PaymentController extends Controller
{
    /** @var SberService Сервис интеграции с API Сбера */
    protected $sberService;
    /** @var NotisendService Сервис отправки SMS через Notisend */
    protected $notisendService;

    /**
     * Внедрение зависимостей через конструктор
     *
     * @param SberService $sberService
     * @param NotisendService $notisendService
     */
    public function __construct(SberService $sberService, NotisendService $notisendService) {
        $this->sberService = $sberService;
        $this->notisendService = $notisendService;
    }

    /**
     * Получение списка всех платёжных запросов
     *
     * Используется в админке для мониторинга выплат
     *
     * Возвращает:
     *  - Пагинацию
     *  - Имена клиента и работника
     *  - Сумму в рублях (делим на 100, т.к. хранится в копейках)
     *  - Статус и время оплаты
     *
     * @param Request $request
     * @return array
     */
    public function getPaymentRequestsList(Request $request) {
        $paymentRequests = PaymentRequest::with(['customer', 'worker'])
            ->orderBy('created_at', 'desc')->paginate(20);

        $data = $paymentRequests->map(function ($paymentRequest) {
            return [
                'id' => $paymentRequest->id,
                'customer' => $paymentRequest->customer->user->name,
                'worker' => $paymentRequest->worker->user->name,
                'amount' => $paymentRequest->amount / 100,
                'status' => $paymentRequest->status,
                'payed_at' => $paymentRequest->payed_at ? Carbon::parse($paymentRequest->payed_at)->format('Y-m-d H:i:s') : '-',
                'response' => $paymentRequest->response ? $paymentRequest->response : null,
            ];
        });

        return [
            'data' => $data,
            'current_page' => $paymentRequests->currentPage(),
            'last_page' => $paymentRequests->lastPage(),
            'per_page' => $paymentRequests->perPage(),
        ];
    }

    /**
     * ТЕСТОВЫЙ МЕТОД — для отладки интеграции
     *
     * Раскомментируйте нужную строку для проверки:
     *  - Регистрация бенефициаров
     *  - Создание смарт-контракта
     *  - Получение токенов
     *  - Отправка SMS-кода
     *
     * @param Request $request
     * @return mixed
     */
    public function test(Request $request) {
        //return $this->sberService->getBeneficiaryState(2); 
        return $this->sberService->getBeneficiaryRegistry(); 
        //return $this->sberService->createSmartContract(1000, 1, 2); 
        //return $this->sberService->getRefreshToken();
        //return $this->sberService->getAccessToken();
        //return $this->notisendService->sendAuthCode('89081696981', '9524');
    }
}
