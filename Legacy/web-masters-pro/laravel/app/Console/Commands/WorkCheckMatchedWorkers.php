<?php

namespace App\Console\Commands;

use App\Events\UpdateAcceptWork;
use App\Models\AcceptWork;
use App\Services\NotificationService;
use App\Services\WorkService;
use Carbon\Carbon;
use Illuminate\Console\Command;

class WorkCheckMatchedWorkers extends Command
{
    /**
     * The name and signature of the console command.
     *
     * @var string
     */
    protected $signature = 'work:check-match';

    /**
     * The console command description.
     *
     * @var string
     */
    protected $description = 'Check matched workers for works';

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
        $acceptWorks = AcceptWork::where('status', 'matched')->get();

        $this->info('Найдено принятых работ для проверки: ' . count($acceptWorks));

        foreach ($acceptWorks as $acceptWork) {
            if (Carbon::parse($acceptWork->created_at)->addMinutes(20)->isFuture())
                continue;

            $this->workService->refuseWorkerMatch($acceptWork);
            $this->notificationService->createNotification([
                'user_id' => $acceptWork->user_id,
                'type' => 'warning',
                'title' => 'Отказ от работы',
                'message' => 'Вы не подтвердили выполнение работы в отведенное время.',
            ]);
        }

        $this->info('Проверка завершена.');
    }
}
