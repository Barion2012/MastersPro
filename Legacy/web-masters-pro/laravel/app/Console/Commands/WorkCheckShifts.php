<?php

namespace App\Console\Commands;

use App\Events\UpdateAcceptWork;
use App\Events\UpdateOrder;
use App\Models\AcceptWork;
use App\Models\WorkShift;
use Illuminate\Console\Command;
use Illuminate\Support\Carbon;
use App\Services\NotificationService;
use App\Services\WorkService;

class WorkCheckShifts extends Command
{
    /**
     * The name and signature of the console command.
     *
     * @var string
     */
    protected $signature = 'work:check-shifts';

    /**
     * The console command description.
     *
     * @var string
     */
    protected $description = 'Check and update work shifts statuses';

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
        $this->checkShiftAddressConfirmation();
        $this->checkShiftMeetingConfirmation();
        $this->checkShiftTimeStartFifteenMinutesPassed();
        $this->checkShiftTimeStartThreeHoursPassed();
    }

    public function checkShiftAddressConfirmation() {
        $workShiftWithoutAddress = WorkShift::where('confirm_address', false)->get();

        $this->info('Найдено смен без подтвержденного адреса: ' . count($workShiftWithoutAddress));

        foreach ($workShiftWithoutAddress as $shift) {
            $shiftDate = Carbon::createFromFormat('d.m.Y H:i', $shift->date);

            if ($shiftDate->isPast()) {
                $this->workService->refuseWorkerMatch(AcceptWork::where([
                    ['user_id', $shift->user_id], 
                    ['work_id', $shift->work_id], 
                    ['order_id', $shift->order_id]
                ])->first());

                $this->notificationService->createNotification([
                    'user_id' => $shift->user_id,
                    'type' => 'warning',
                    'title' => 'Не подтвержден адрес встречи',
                    'message' => 'Вы не подтвердили адрес встречи для смены от ' . $shift->date . '. Заказ отменен.',
                ]);
            }
        }

        $this->info('Проверка завершена.');
    }

    public function checkShiftMeetingConfirmation() {
        $workShiftWithoutMeeting = WorkShift::where('confirm_meeting', false)->get();

        $this->info('Найдено смен без подтвержденной встречи: ' . count($workShiftWithoutMeeting));

        foreach ($workShiftWithoutMeeting as $shift) {
            $shiftDate = Carbon::createFromFormat('d.m.Y H:i', $shift->date);

            if ($shiftDate->addMinutes(15)->isPast()) {
                $this->workService->refuseWorkerMatch(AcceptWork::where([
                    ['user_id', $shift->user_id], 
                    ['work_id', $shift->work_id], 
                    ['order_id', $shift->order_id]
                ])->first());

                $this->notificationService->createNotification([
                    'user_id' => $shift->user_id,
                    'type' => 'warning',
                    'title' => 'Не подтверждена встреча',
                    'message' => 'Вы не подтвердили встречу для смены от ' . $shift->date . '. Заказ отменен.',
                ]);
            }
        }
    }

    public function checkShiftTimeStartFifteenMinutesPassed() {
        $workShifts = WorkShift::whereNotNull('time_start')->where('not_met', false)->get();
        $this->info('Проверка смен с установленным временем начала: ' . count($workShifts));

        foreach ($workShifts as $shift) {
            if (Carbon::parse($shift->time_start)->addMinutes(15)->isPast()) {
                $shift->not_met = true;
                $shift->save();

                broadcast(new UpdateAcceptWork($shift->user_id));
            }
        }

        $this->info('Проверка завершена.');
    }

    public function checkShiftTimeStartThreeHoursPassed() {
        $workShifts = WorkShift::whereNotNull('date')->get();
        $this->info('Проверка смен с установленным временем начала: ' . count($workShifts));

        foreach ($workShifts as $shift) {
            if (Carbon::parse($shift->date)->addMinutes(180)->isPast()) {
                broadcast(new UpdateAcceptWork($shift->user_id));
                broadcast(new UpdateOrder($shift->order_id));
            }
        }

        $this->info('Проверка завершена.');
    }
}
