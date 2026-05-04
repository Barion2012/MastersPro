<?php

namespace App\Console\Commands;

use App\Events\UpdateBalance;
use App\Events\UpdateOrder;
use App\Models\AcceptWork;
use App\Models\Work;
use App\Models\WorkerProfession;
use App\Services\NotificationService;
use App\Services\WorkService;
use Illuminate\Console\Command;
use Illuminate\Support\Carbon;

class WorkMatchWorkers extends Command
{
    /**
     * The name and signature of the console command.
     *
     * @var string
     */
    protected $signature = 'work:match-workers';

    /**
     * The console command description.
     *
     * @var string
     */
    protected $description = 'Match workers to works';

    protected WorkService $workService;
    protected NotificationService $notificationService;

    public function __construct(WorkService $workService, NotificationService $notificationService)
    {
        parent::__construct();
        $this->workService = $workService;
        $this->notificationService = $notificationService;
    }

    /**
     * Execute the console command.
     */
    public function handle()
    {
        $works = $this->getSearchWorks();

        $this->info('Найдено работ для обработки: ' . count($works));

        foreach ($works as $work) {
            $workers = WorkerProfession::where([['profession_id', $work->profession_id], ['profession_level_id', $work->profession_level_id]])
                ->with('worker')
                ->get()
                ->pluck('worker');

            $this->info('Обрабатывается работа ID ' . $work->id . ', найдено работников по профессии: ' . count($workers));

            $radiuses = [1, 3, 5, 10, 20, 40];
            $foundWorkers = [];
            $nededCount = $work->count - $work->found;

            foreach ($radiuses as $radius) {
                $foundWorkers = $this->findWorkersInRadius($workers, $work->order, $radius);

                if (count($foundWorkers) >= $nededCount) {
                    break;
                }
            }

            if (count($foundWorkers) > $nededCount) {
                $foundWorkers = array_slice($foundWorkers, 0, $nededCount);
            }

            foreach ($foundWorkers as $worker) {
                $this->workService->matchWorker($work, $worker);
                $this->notificationService->createNotification([
                    'user_id' => $worker->user_id,
                    'type' => 'success',
                    'title' => 'Новая заявка на работу',
                    'message' => 'Появилась новая заявка на работу в вашем районе.',
                ]);

                $this->info('Создана заявка на работу ID ' . $work->id . ' для работника ID ' . $worker->id);
            }

            broadcast(new UpdateOrder($work->order_id));
        }
    }

    public function findWorkersInRadius($workers, $order, $radiusKm)
    {
        $this->info('Поиск работников в радиусе ' . $radiusKm . ' км');

        $foundWorkers = [];

        foreach ($workers as $worker) {
            if (!$this->inRadius($order->address_lat, $order->address_lng, $worker->location_lat, $worker->location_lng, $radiusKm))
                continue;

            $accept = AcceptWork::where([
                ['user_id', $worker->user_id],
            ])->whereNotIn('status', ['refused', 'completed'])->first();

            if (!empty($accept))
                continue;

            $checkAccept = AcceptWork::where([
                ['user_id', $worker->user_id],
                ['order_id', $order->id],
            ])->first();

            if (!empty($checkAccept))
                continue;

            $foundWorkers[] = $worker;
        }

        $this->info('Найдено работников в радиусе ' . $radiusKm . ' км: ' . count($foundWorkers));
        return $foundWorkers;
    }

    public function inRadius(float $lat1, float $lon1, float $lat2, float $lon2, float $radiusKm) {
        $R = 6371;

        $lat1 = deg2rad($lat1);
        $lon1 = deg2rad($lon1);

        $lat2 = deg2rad($lat2);
        $lon2 = deg2rad($lon2);

        $x = ($lon2 - $lon1) * cos(($lat1 + $lat2) / 2);
        $y = $lat2 - $lat1;
        $distance = $R * sqrt($x * $x + $y * $y);

        return $distance <= $radiusKm;
    }

    public function getSearchWorks()
    {
        $worksExpired = Work::where('start_date', '<', now())
            ->where('status', '!=', 'expired')
            ->get();
        
        foreach ($worksExpired as $work) {
            if ($work->found >= $work->count)
                continue;
            $work->status = 'expired';
            $work->save();

            broadcast(new UpdateOrder($work->order_id));

            $this->info('Работа ID ' . $work->id . ' помечена как просроченная.');
            
            $this->notificationService->createNotification([
                'user_id' => $work->order->customer->user_id,
                'type' => 'warning',
                'title' => 'Работа просрочена',
                'message' => 'Не найдено ни одного работника для работы ID ' . $work->id . '. Работа помечена как просроченная. Возврат средств будет произведен в ближайшее время.',
            ]);

            $startDate = Carbon::parse($work->start_date);
            $endDate = Carbon::parse($work->end_date)->startOfDay();
            $workShiftCount = $startDate->diffInDays($endDate, true);
            $workShiftCount = ceil($workShiftCount);

            if ($workShiftCount == 0) {
                $workShiftCount = 1;
            }
            $this->info('Количество смен для возврата: ' . $workShiftCount);
            $this->info('Цена за смену: ' . $work->price_shift);
            $this->info('Количество работников в заказе: ' . $work->count);
            $totalSum = $workShiftCount * $work->price_shift * ($work->count - $work->found);
            $this->info('Возврат средств заказчику в размере: ' . $totalSum);
            $customer = $work->order->customer;
            $customer->balance += $totalSum;
            $customer->hold_balance -= $totalSum;
            $customer->save();
            broadcast(new UpdateBalance($customer->user_id, $customer->balance));
        }


        $works = Work::where('start_date', '>', now())
            ->where('status', 'search')
            ->get();

        return $works;
    }
}
