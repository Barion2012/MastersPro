<?php

namespace App\Console\Commands;

use App\Models\PaymentRequest;
use App\Services\SberService;
use Illuminate\Console\Command;

class CheckPaymentRequests extends Command
{
    /**
     * The name and signature of the console command.
     *
     * @var string
     */
    protected $signature = 'payment:check';
    private $sberService;

    /**
     * The console command description.
     *
     * @var string
     */
    protected $description = 'Check and process payment requests';

    /**
     * Execute the console command.
     */

    public function __construct(SberService $sberService)
    {
        parent::__construct();
        $this->sberService = $sberService;
    }

    public function handle()
    {
        $paymentRequests = PaymentRequest::get();

        foreach ($paymentRequests as $paymentRequest) {
            switch ($paymentRequest->status) {
                case 'created':
                    $this->processCreatedSmartContract($paymentRequest);
                    break;
                case 'pending':
                    $this->processConfirmSmartContract($paymentRequest);
                    break;
                case 'failed':
                    $this->processFailedPaymentRequest($paymentRequest);
                    break;
            }
            
        }
    }

    private function processCreatedSmartContract(PaymentRequest $paymentRequest)
    {
        try {
            $paymentRequest->smart_contract_id = $this->sberService->createSmartContract($paymentRequest->amount, $paymentRequest->customer_id, $paymentRequest->worker_id);
            $paymentRequest->status = 'pending';
            $paymentRequest->save();
        }
        catch (\Exception $e) {
            $paymentRequest->status = 'failed';
            $paymentRequest->response = $e->getMessage();
            $paymentRequest->save();
        }
    }

    private function processConfirmSmartContract(PaymentRequest $paymentRequest) {
        try {
            $paymentRequest->response = $this->sberService->confirmSmartContract($paymentRequest->smart_contract_id);
            $paymentRequest->status = 'completed';
            $paymentRequest->payed_at = now();
            $paymentRequest->save();
        } catch (\Exception $e) {
            $paymentRequest->status = 'failed';
            $paymentRequest->response = $e->getMessage();
            $paymentRequest->save();
        }
    }

    private function processFailedPaymentRequest(PaymentRequest $paymentRequest) {
        if ($paymentRequest->smart_contract_id) {
            $this->processConfirmSmartContract($paymentRequest);
        }
        else {
            $this->processCreatedSmartContract($paymentRequest);
        }
    }
}
