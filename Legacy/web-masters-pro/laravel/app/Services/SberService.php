<?php 

namespace App\Services;

use App\Models\Customer;
use App\Models\PaymentRequest;
use App\Models\Worker;
use App\Models\WorkShift;
use Carbon\Carbon;
use Faker\Provider\ar_EG\Payment;
use Illuminate\Support\Facades\Auth;
use Illuminate\Support\Facades\Http;
use Illuminate\Support\Facades\Log;
use Illuminate\Support\Facades\Redis;

class SberService {
    public function initTokens() {
        $response = Http::withOptions([
            'cert' => '/var/www/html/certs/pem/sber-client-cert.pem', // Клиентский сертификат
            'ssl_key' => '/var/www/html/certs/pem/sber-client-key-nopass.pem', // Приватный ключ без пароля
            'verify' => false, // CA-цепочка
            'headers' => [
                'Content-Type' => 'application/x-www-form-urlencoded',
            ],
        ])->post(config('sber.api').'/ic/sso/api/v2/oauth/token?grant_type=authorization_code&code='.config('sber.code').'&client_id='.config('sber.client_id').'&redirect_uri=https%3A%2F%2Fwww.sberbank.ru%2Fru%2Fperson&client_secret='.config('sber.client_secret'), []);
        
        return $response;
    }

    public function getAccessToken() {
        $accessToken = Redis::get('sber:access_token');

        if ($accessToken && Redis::ttl('sber:access_token') > 0)
            return $accessToken;

        return $this->refreshAccessToken();
    }

    public function getRefreshToken() {
        return Redis::get('sber:refresh_token');
    }

    public function refreshAccessToken() {
        $refreshToken = Redis::get('sber:refresh_token');

        try {
            $response = Http::withOptions([
                'cert' => '/var/www/html/certs/pem/sber-client-cert.pem', // Клиентский сертификат
                'ssl_key' => '/var/www/html/certs/pem/sber-client-key-nopass.pem', // Приватный ключ без пароля
                'verify' => false, // CA-цепочка
                'headers' => [
                    'Content-Type' => 'application/x-www-form-urlencoded',
                ],
            ])->post(config('sber.api').'/ic/sso/api/v2/oauth/token?grant_type=refresh_token&client_id='.config('sber.client_id').'&client_secret='.config('sber.client_secret').'&refresh_token='.$refreshToken, []);

            if (!$response->successful()) {
                throw new \Exception('Ошибка обновления токенов: ' . $response->body());
            }

            $data = $response->json();

            $this->saveTokens($data['access_token'], $data['refresh_token']);

            return $data['access_token'];
        }
        catch (\Exception $e) {
            throw new \Exception('Token refresh failed: ' . $e->getMessage());
        }
    }

    public function saveTokens($accessToken, $refreshToken) {
        Redis::setex('sber:access_token', 3600, $accessToken);
        Redis::set('sber:refresh_token', $refreshToken);
    }

    public function post($url, $content) {
        $data = [
            'content' => $content,
            'signature' => $this->signContent($content)
        ];

        $response = Http::withOptions([
            'cert' => '/var/www/html/certs/pem/sber-client-cert.pem', // Клиентский сертификат
            'ssl_key' => '/var/www/html/certs/pem/sber-client-key-nopass.pem', // Приватный ключ без пароля
            'verify' => false, // CA-цепочка
            'headers' => [
                'Content-Type' => 'application/json',
                'RqUID' => $this->generateUuidLike(),
                'Authorization' => $this->getAccessToken()
            ],
        ])->post(config('sber.api').$url, $data);

        if (!$response->successful()) {
            Log::error('Ошибка отправки запроса');
            Log::error($response->body());
            Log::error($data);
            Log::error($response->headers());
            Log::error(json_encode($data));
            throw new \Exception($response->body()); 
        }

        return $response;
    }

    public function get($url) {
        $response = Http::withOptions([
            'cert' => '/var/www/html/certs/pem/sber-client-cert.pem', // Клиентский сертификат
            'ssl_key' => '/var/www/html/certs/pem/sber-client-key-nopass.pem', // Приватный ключ без пароля
            'verify' => false, // CA-цепочка
            'headers' => [
                'Content-Type' => 'application/json',
                'RqUID' => $this->generateUuidLike(),
                'Authorization' => $this->getAccessToken()
            ],
        ])->get(config('sber.api').$url);

        if (!$response->successful()) {
            throw new \Exception($response->body());
        }

        return $response;
    }

    public function signContent($content) {
        $response = Http::post('http://cryptopro/sign', [
            'content' => $content
        ]);

        if (!$response->successful()) {
            throw new \Exception('Ошибка подписание контента: ' . $response->body());
        }

        if (empty($response['signature'])) {
            throw new \Exception('Ошибка подписание контента. Отсутствует подпись: ' . $response->body());
        }

        return $response['signature'];
    }

    public function createBeneficiary($customerId) {
        $customer = Customer::find($customerId);
        if (empty($customer)) {
            throw new \Exception('Клиент не найден');
        }

        switch ($customer->type) {
            case 'ip':
                $data = [
                    'beneficiaryId' => $this->generateUuidLike(),
                    'beneficiaryType' => '2',
                    'nominalAccountId' => config('sber.nominal_id'),
                    'contractNumber' => "Договор-".time(),
                    'contractDate' => Carbon::now()->format('Y-m-d'),
                    //'contractExpDate' => Carbon::now()->addYear()->format('Y-m-d'),
                    'surname' => $customer->info['surname'],
                    'name' => $customer->info['name'],
                    'patronymic' => $customer->info['patronymic'],
                    'birthDate' => Carbon::parse($customer->info['birthDate'])->format('Y-m-d'),
                    'identificationDoc' => [
                        'identificationDocType' => 'Паспорт гражданина РФ',
                        'seria' => $customer->info['seria'],
                        'number' => $customer->info['number'],
                        'issuer' => $customer->info['issuer'],
                        'issueDate' => Carbon::parse($customer->info['issueDate'])->format('Y-m-d'),
                        'issuerCode' => $customer->info['issuerCode'],
                    ],
                    'inn' => $customer->info['inn'],
                    'ogrnip' => $customer->info['ogrnip'],
                    'postAddress' => $customer->info['postAddress'],
                    //'isForeignOrg' => false,
                    //'isTaxResidentRF' => true,
                    'isOnlyTaxResidentRF' => true,
                    //'isControlPersonsRF' => true,
                    'phone' => $customer->user->formatted_phone,
                    'email' => $customer->user->email,
                ];
                break;
            case 'ooo':
                $data = [
                    'beneficiaryId' => $this->generateUuidLike(),
                    'beneficiaryType' => '1',
                    'nominalAccountId' => config('sber.nominal_id'),
                    'contractNumber' => "Договор_".time(),
                    'contractDate' => Carbon::now()->format('Y-m-d'),
                    'ogrn' => $customer->info['ogrn'],
                    'inn' => $customer->info['inn'],
                    'kpp' => $customer->info['kpp'],
                    'orgName' => $customer->info['orgName'],
                    'legalAddress' => $customer->info['legalAddress'],
                    'isForeignOrg' => false,
                    'isTaxResidentRF' => true,
                    'isOnlyTaxResidentRF' => true,
                    'isControlPersonsRF' => true,
                    'phone' => $customer->user->formatted_phone,
                    'email' => $customer->user->email,
                ];
                break;
            case 'pao':
                $data = [
                    'beneficiaryId' => $this->generateUuidLike(),
                    'beneficiaryType' => '1',
                    'nominalAccountId' => config('sber.nominal_id'),
                    'contractNumber' => "Договор_".time(),
                    'contractDate' => Carbon::now()->format('Y-m-d'),
                    'ogrn' => $customer->info['ogrn'],
                    'inn' => $customer->info['inn'],
                    'kpp' => $customer->info['kpp'],
                    'orgName' => $customer->info['orgName'],
                    'legalAddress' => $customer->info['legalAddress'],
                    'isForeignOrg' => false,
                    'isTaxResidentRF' => true,
                    'isOnlyTaxResidentRF' => true,
                    'isControlPersonsRF' => true,
                    'phone' => $customer->user->formatted_phone,
                    'email' => $customer->user->email,
                ];
                break;
            case 'ao':
                $data = [
                    'beneficiaryId' => $this->generateUuidLike(),
                    'beneficiaryType' => '1',
                    'nominalAccountId' => config('sber.nominal_id'),
                    'contractNumber' => "Договор_".time(),
                    'contractDate' => Carbon::now()->format('Y-m-d'),
                    'ogrn' => $customer->info['ogrn'],
                    'inn' => $customer->info['inn'],
                    'kpp' => $customer->info['kpp'],
                    'orgName' => $customer->info['orgName'],
                    'legalAddress' => $customer->info['legalAddress'],
                    'isForeignOrg' => false,
                    'isTaxResidentRF' => true,
                    'isOnlyTaxResidentRF' => true,
                    'isControlPersonsRF' => true,
                    'phone' => $customer->user->formatted_phone,
                    'email' => $customer->user->email,
                ];
                break;
            default:
                throw new \Exception('Неизвестный тип клиента: ' . $customer->type);
        }

        $data['account'] = [
            'accountNumber' => $customer->info['accountNumber'],
            'bankBIC' => $customer->info['bankBIC'],
            'bankCorAccount' => $customer->info['bankCorAccount'],
            'bankName' => $customer->info['bankName'],
        ];

        $response = $this->post('/v1/nominal-account/beneficiaries/create', [
            'data' => $data,
            'agreement' => 'Клиент подтверждает, что операция совершается в соответствии с условиями Договора номинального счета'
        ]);

        if (!$response->successful()) {
            throw new \Exception('Ошибка подписание контента: ' . $response->body());
        }

        $responseData = $response->json();

        if (empty($responseData['beneficiaryId'])) {
            throw new \Exception('Ошибка создания бенефициара: ' . $response->body());
        }

        $customer->beneficiary_id = $responseData['beneficiaryId'];
        $customer->save();
    }

    public function getBeneficiaryState($customerId) {
        $customer = Customer::find($customerId);

        if (empty($customer)) {
            throw new \Exception('Клиент не найден');
        }

        return $this->get('/v1/nominal-account/beneficiaries/state/'.$customer->beneficiary_id);
    }

    public function syncCustomerBalance($customerId) {
        $customer = Customer::find($customerId);

        if (empty($customer)) {
            throw new \Exception('Клиент не найден');
        }

        $beneficiaryData = $this->getBeneficiaryState($customer->id);
        $balance = $beneficiaryData['balance']['balance'] - $beneficiaryData['balance']['obligations'] - $beneficiaryData['balance']['pending'] - $beneficiaryData['balance']['blocked']- $beneficiaryData['balance']['debt'];
        //$hold_balance = $beneficiaryData['balance']['obligations'];
        $customer->balance = (int)($balance / 100);
        //$customer->hold_balance = (int)($hold_balance / 100);
        $customer->save();
    }

    public function getBeneficiaryRegistry() {
        return $this->get('/v1/nominal-account/beneficiaries/registry');
    }

    public function createSmartContract($amount, $customerId, $workerId) {
        if (config('sber.pay_disable')) {
            return $this->generateUuidLike();
        }

        $customer = Customer::find($customerId);
        $worker = Worker::find($workerId);

        if (empty($customer)) {
            throw new \Exception('Клиент не найден');
        }

        if (empty($worker)) {
            throw new \Exception('Работник не найден');
        }

        $data = [
            'id' => $this->generateUuidLike(),
            'title' => 'Договор оказания услуг',
            'amount' => (int)$amount,
            'currency' => 'RUB',
            'beneficiaryId' => $customer->beneficiary_id,
            'contractors' => [
                [
                    'typeCode' => 'FL',
                    'personName' => $worker->user->name,
                    'inn' => $worker->inn,
                    'account' => [
                        'accountNumber' => $worker->account_number,
                        'bankBIC' => $worker->bank_bic,
                        'bankCorAccount' => $worker->bank_cor_account,
                        'bankName' => $worker->bank_name,
                    ]
                ]
            ]
        ];

        $response = $this->post('/v1/nominal-account/smart-contracts', [
            'data' => $data,
            'agreement' => 'Клиент подтверждает, что операция совершается в соответствии с условиями Договора номинального счета'
        ]);

        return $response['smartContractId'];
    }

    public function confirmSmartContract($smartContractId) {
        if (config('sber.pay_disable')) {
            return null;
        }

        $paymentRequest = PaymentRequest::where('smart_contract_id', $smartContractId)->first();

        if (empty($paymentRequest)) {
            throw new \Exception('Платежный запрос не найден');
        }

        $customer = Customer::find($paymentRequest->customer_id);
        $worker = Worker::find($paymentRequest->worker_id);

        if (empty($customer)) {
            throw new \Exception('Клиент не найден');
        }

        if (empty($worker)) {
            throw new \Exception('Работник не найден');
        }

        switch ($customer->type) {
            case 'ooo': {
                $payer = [
                    'beneficiaryId' => $customer->beneficiary_id,
                    'typeCode' => 'UL',
                    'orgName' => $customer->info['orgName'],
                    'inn' => $customer->info['inn'],
                    'kpp' => $customer->info['kpp'],
                    'ogrn' => $customer->info['ogrn'],
                ];
                break;
            }
            case 'ip': {
                $payer = [
                    'beneficiaryId' => $customer->beneficiary_id,
                    'typeCode' => 'IP',
                    'personName' => $customer->info['surname'].' '.$customer->info['name'].' '.$customer->info['patronymic'],
                    'inn' => $customer->info['inn'],
                    'ogrnip' => $customer->info['ogrnip'],
                ];

                break;
            }
            default: {
                $payer = [];
            }
        }

        $data = [
            'smartContractId' => $smartContractId,
            'transactions' => [
                [
                    'transactionId' => $this->generateUuidLike(),
                    'transactionType' => 'PAYMENT',
                    'payer' => $payer,
                    'payee' => [
                        'typeCode' => 'FL',
                        'personName' => $worker->user->name,
                        'inn' => $worker->inn,
                        'account' => [
                            'accountNumber' => $worker->account_number,
                            'bankBIC' => $worker->bank_bic,
                            'bankCorAccount' => $worker->bank_cor_account,
                            'bankName' => $worker->bank_name
                        ],
                    ],
                    'amount' => (int)$paymentRequest->amount,
                    'currency' => 'RUB',
                    'purpose' => 'Оплата рабочей смены',
                ]
            ]
        ];

        return $this->post('/v1/nominal-account/smart-contracts/confirmstep', [
            'data' => $data,
            'agreement' => 'Клиент подтверждает, что операция совершается в соответствии с условиями Договора номинального счета'
        ]);
    }

    public function completeSmartContract($smartContractId) {
        if (config('sber.pay_disable')) {
            return null;
        }

        $payment = PaymentRequest::where('smart_contract_id', $smartContractId)->first();

        if (empty($payment)) {
            throw new \Exception('Платеж не найден');
        }

        $worker = Worker::find($payment->worker_id);
        $customer = Customer::find($payment->customer_id);
        $data = [
            'id' => $smartContractId,
            'beneficiaryId' => $customer->beneficiary_id,
            'contractors' => [
                [
                    'typeCode' => 'FL',
                    'personName' => $worker->user->name,
                    'inn' => $worker->inn,
                    'account' => [
                        'accountNumber' => $worker->account_number,
                        'bankBIC' => $worker->bank_bic,
                        'bankCorAccount' => $worker->bank_cor_account,
                        'bankName' => $worker->bank_name,
                    ]
                ]
            ]
        ];

        $this->post('/v1/nominal-account/smart-contracts/completion', [
            'data' => $data,
            'agreement' => 'Клиент подтверждает, что операция совершается в соответствии с условиями Договора номинального счета'
        ]);
    }

    public function getSmartContractInfo($smartContractId) {
        return $this->get('/v1/nominal-account/smart-contracts/'.$smartContractId);
    }

    public function createPaymentRequest($customerId, $workShiftId, $price = null) {
        $customer = Customer::find($customerId);
        $workShift = WorkShift::find($workShiftId);

        if (empty($customer)) {
            throw new \Exception('Клиент не найден');
        }

        if (empty($workShift)) {
            throw new \Exception('Рабочая смена не найдена');
        }

        $worker = Worker::where('user_id', $workShift->user_id)->first();

        PaymentRequest::create([
            'customer_id' => $customer->id,
            'work_shift_id' => $workShift->id,
            'worker_id' => $worker->id,
            'order_id' => $workShift->order_id,
            'status' => 'created',
            'amount' => $price ? (int)($price * 100) : (int)($workShift->price * 100),
        ]);
    }

    function generateUuidLike() {
        $data = random_bytes(16); // Генерируем 16 случайных байт
        $data[6] = chr(ord($data[6]) & 0x0f | 0x40); // Устанавливаем версию 4
        $data[8] = chr(ord($data[8]) & 0x3f | 0x80); // Устанавливаем вариант
        return vsprintf('%s%s-%s-%s-%s-%s%s%s', str_split(bin2hex($data), 4));
    }
}