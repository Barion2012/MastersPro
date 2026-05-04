<?php

namespace App\Console\Commands;

use App\Events\UserNotification;
use App\Events\WorkAvailable;
use App\Jobs\NotifyWorkers;
use App\Models\AcceptWork;
use App\Models\Notification;
use App\Models\Order;
use App\Models\WorkerProfession;
use App\Services\WorkService;
use Carbon\Carbon;
use Illuminate\Console\Command;
use Illuminate\Support\Facades\Log;

class CheckPendingOrders extends Command
{
    /**
     * The name and signature of the console command.
     *
     * @var string
     */
    protected $signature = 'orders:notify';

    /**
     * The console command description.
     *
     * @var string
     */
    protected $description = 'Check and notify performers for pending orders';

    protected WorkService $workService;

    public function __construct(WorkService $workService)
    {
        parent::__construct();
        $this->workService = $workService;
    }

    /**
     * Execute the console command.
     */
    public function handle()
    {
        $orders = Order::where('status', 'search')->get();
        foreach($orders as $order) {
            $this->workService->checkOrderStatus($order);
            $works = $order->works()
                ->where('end_date', '>', now())
                ->get();

            if ($works->isEmpty()) {
                $order->status = 'done';
                $order->save();
                continue;
            }
        }
    }
}
