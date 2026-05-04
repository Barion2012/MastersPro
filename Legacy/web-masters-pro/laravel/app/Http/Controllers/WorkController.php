<?php

namespace App\Http\Controllers;

use App\Events\UpdateAcceptWork;
use App\Events\UpdateBalance;
use App\Events\UpdateOrder;
use App\Events\WorkerRefused;
use App\Events\WorkShiftUpdate;
use App\Events\WorkUnavailable;
use App\Models\AcceptWork;
use App\Models\Order;
use App\Models\Report;
use App\Models\User;
use App\Models\Work;
use App\Models\Worker;
use App\Models\WorkShift;
use App\Services\WorkService;
use App\Services\FileService;
use App\Services\NotificationService;
use App\Services\SberService;
use App\Services\TicketService;
use Carbon\Carbon;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Auth;
use Illuminate\Support\Facades\Validator;

class WorkController extends Controller
{
    protected $workService;
    protected $fileService;
    protected $notificationService;
    protected $ticketService;
    protected $sberService;

    public function __construct(WorkService $workService, FileService $fileService, NotificationService $notificationService, TicketService $ticketService, SberService $sberService) {
        $this->ticketService = $ticketService;
        $this->workService = $workService;
        $this->fileService = $fileService;
        $this->notificationService = $notificationService;
        $this->sberService = $sberService;
    }

    public function acceptWorkOrder(Request $request, $workId) {
        $validator = Validator::make(
            [
                'workId' => $workId,
            ],
            [
                'workId' => 'required|integer|exists:works,id',
            ],
            [
                'workId.required' => 'Идентификатор работы обязателен.',
                'workId.integer' => 'Идентификатор работы должен быть числом.',
                'workId.exists' => 'Указанная работа не существует.',
            ]
        );

        if ($validator->fails()) {
            return response(['errors' => $validator->errors()], 422);
        }

        $work = Work::find($workId);

        $accept = AcceptWork::where([
            ['user_id', Auth::user()->id],
            ['order_id', $work->order_id],
            ['work_id', $workId],
            ['profession_id', $work->profession_id],
            ['profession_level_id', $work->profession_level_id],
        ])->first();

        if(empty($accept))
            return $this->createValidationErrorResponse('Вам не предлагалась эта работа.');

        $accept->status = 'pending';
        $accept->save();

        if ($work->found >= $work->count) {
            broadcast(new WorkUnavailable($work->id));
        }
        broadcast(new UpdateOrder($work->order_id));
        broadcast(new UpdateAcceptWork(Auth::user()->id));

        return response(['message' => 'Заказ принят'], 200);
    }

    public function refuseWorkOrder(Request $request, $workId) {
        $work = Work::find($workId);
        $user = User::find(Auth::user()->id);
        $accept = AcceptWork::where([
            ['user_id', Auth::user()->id],
            ['order_id', $work->order_id],
            ['work_id', $workId],
            ['profession_id', $work->profession_id],
            ['profession_level_id', $work->profession_level_id]
        ])->first();

        if (empty($accept))
            return $this->createValidationErrorResponse('Заказ не найден');

        $this->workService->refuseWorkerMatch($accept);

        return response(['message' => 'Вы отказались от работы'], 200);
    }

    public function checkAcceptWork(Request $request) {
        $acceptWork = AcceptWork::where('user_id', Auth::user()->id)
            ->whereNotIn('status', ['refused', 'completed'])
            ->first();

        return $acceptWork ? ['workId' => $acceptWork->work_id] : false;
    }

    public function getAcceptWork(Request $request) {
        $acceptWork = AcceptWork::where('user_id', Auth::user()->id)
            ->whereNotIn('status', ['refused', 'completed'])
            ->first();
        if (empty($acceptWork)) {
            return [
                'status' => 'none'
            ];
        }
        $order = Order::find($acceptWork->order_id);
        $work = Work::find($acceptWork->work_id);
        
        return [
            'status' => $acceptWork->status,
            'confirm_address' => $acceptWork->confirm_address,
            'confirm_meeting' => $acceptWork->confirm_meeting,
            'work_shifts' => $acceptWork->work->workShifts,
            'worker_shift_count' => WorkShift::where([
                ['work_id', $work->id],
                ['user_id', $acceptWork->user_id],
            ])->count(),
            'current_work_shift' => $acceptWork->worker->currentWorkShift,
            'address_lat' => $order->address_lat,
            'address_lng' => $order->address_lng,
            'address' => $order->address,
            'meeting_point_lat' => $order->meeting_point_lat,
            'meeting_point_lng' => $order->meeting_point_lng,
            'meeting_point' => $order->meeting_point,
            'order' => $work->order,
            'work' => $work->serialize(),
            'info' => $order->info,
            'place' => $order->place,
            'quality' => $order->quality,
            'start_date' => Carbon::parse($work->start_date)->format('d.m.Y'),
            'end_date' => Carbon::parse($work->end_date)->format('d.m.Y'),
            'shift_start_time' => Carbon::parse($work->start_date)->format('H:i'),
            'files' => [
                'passport_scan' => $acceptWork->worker->getFileId('passport_scan'),
                'snils' => $acceptWork->worker->getFileId('snils'),
                'migration_card' => $acceptWork->worker->getFileId('migration_card'),
                'patent' => $acceptWork->worker->getFileId('patent'),
                'patent_cheque' => $acceptWork->worker->getFileId('patent_cheque'),
                'dms' => $acceptWork->worker->getFileId('dms'),
            ],
            'order_files' => [
                'info_file' => $order->getFile('info_file'),
                'quality_file' => $order->getFile('quality_file'),
                'place_file' => $order->getFile('place_file'),
            ]
        ];
    }

    public function acceptWorkAddress(Request $request, $workId) {
        $acceptWork = AcceptWork::where([['user_id', Auth::user()->id], ['work_id', $workId]])->first();

        if (empty($acceptWork))
            return $this->createValidationErrorResponse('Заказ не найден');

        $acceptWork->confirm_address = true;
        $acceptWork->status = 'confirmed';
        $acceptWork->save();

        $this->notificationService->createNotification([
            'user_id' => $acceptWork->order->customer->user_id,
            'type' => 'info',
            'title' => 'Мастер подтвердил выход на объект',
            'message' => 'Мастер подтвердил выход на объект.',
            'url' => '/customer/order/'.$acceptWork->order_id.'/detail'
        ]);

        broadcast(new UpdateOrder($acceptWork->order_id));

        return response(['message' => 'Выход на объект подтвержден'], 200);
    }

    public function acceptWorkMeeting(Request $request, $workId) {
        $acceptWork = AcceptWork::where([['user_id', Auth::user()->id], ['work_id', $workId]])->first();

        if (empty($acceptWork))
            return $this->createValidationErrorResponse('Заказ не найден');

        $acceptWork->confirm_meeting = true;
        $acceptWork->status = 'arrive';
        $acceptWork->save();

        $this->notificationService->createNotification([
            'user_id' => $acceptWork->order->customer->user_id,
            'type' => 'info',
            'title' => 'Мастер на месте встречи',
            'message' => 'Мастер достиг места встречи.',
            'url' => '/customer/order/'.$acceptWork->order_id.'/detail'
        ]);

        broadcast(new UpdateOrder($acceptWork->order_id));

        return response(['message' => 'Адрес встречи подтвержден'], 200);
    }

    public function acceptWorkShiftMeeting(Request $request, $workShiftId) {
        $workShift = WorkShift::find($workShiftId);
        
        if (empty($workShift))
            return $this->createValidationErrorResponse('Смена не найдена');

        $acceptWork = AcceptWork::where([['user_id', Auth::user()->id], ['work_id', $workShift->work_id]])->first();

        if (empty($acceptWork))
            return $this->createValidationErrorResponse('Заказ не найден');

        $workShift->confirm_meeting = true;
        $workShift->time_start = Carbon::now()->format('H:i');
        $workShift->save();

        $acceptWork->status = 'arrive';
        $acceptWork->save();

        $this->notificationService->createNotification([
            'user_id' => $workShift->order->customer->user_id,
            'type' => 'info',
            'title' => 'Мастер на месте встречи',
            'message' => 'Мастер достиг места встречи.',
            'url' => '/customer/order/'.$workShift->order->id.'/detail'
        ]);

        broadcast(new UpdateOrder($workShift->order->id));
        broadcast(new UpdateAcceptWork($workShift->user_id));
        broadcast(new WorkShiftUpdate($workShift->user_id));

        return response(['message' => 'Адрес встречи подтвержден'], 200);
    }

    public function acceptWorkShiftAddress(Request $request, $workShiftId) {
        $workShift = WorkShift::find($workShiftId);
        
        if (empty($workShift))
            return $this->createValidationErrorResponse('Смена не найдена');

        $workShift->confirm_address = true;
        $workShift->save();

        $this->notificationService->createNotification([
            'user_id' => $workShift->order->customer->user_id,
            'type' => 'info',
            'title' => 'Мастер подтвердил выход на объект',
            'message' => 'Мастер подтвердил выход на объект.',
            'url' => '/customer/order/'.$workShift->order->id.'/detail'
        ]);

        broadcast(new UpdateOrder($workShift->order->id));
        broadcast(new UpdateAcceptWork($workShift->user_id));
        broadcast(new WorkShiftUpdate($workShift->user_id));

        return response(['message' => 'Адрес встречи подтвержден'], 200);
    }

    public function createReport(Request $request, $workId, $workShiftId) {
        $acceptWork = AcceptWork::where([['user_id', Auth::user()->id], ['work_id', $workId]])->first();

        if (empty($acceptWork))
            return $this->createValidationErrorResponse('Заказ не найден');

        $validator = Validator::make(
            $request->all(),
            [
                'photo_1' => 'nullable|file|mimes:jpeg,png,jpg|max:20480',
                'photo_2' => 'nullable|file|mimes:jpeg,png,jpg|max:20480',
                'photo_3' => 'nullable|file|mimes:jpeg,png,jpg|max:20480',
                'photo_4' => 'nullable|file|mimes:jpeg,png,jpg|max:20480',
            ],
            [
                'photo_1.file' => 'Фото 1 должно быть файлом.',
                'photo_1.mimes' => 'Фото 1 должно быть в формате jpeg, png или jpg.',
                'photo_1.max' => 'Фото 1 не должно превышать 20MB.',
                'photo_2.file' => 'Фото 2 должно быть файлом.',
                'photo_2.mimes' => 'Фото 2 должно быть в формате jpeg, png или jpg.',
                'photo_2.max' => 'Фото 2 не должно превышать 20MB.',
                'photo_3.file' => 'Фото 3 должно быть файлом.',
                'photo_3.mimes' => 'Фото 3 должно быть в формате jpeg, png или jpg.',
                'photo_3.max' => 'Фото 3 не должно превышать 20MB.',
                'photo_4.file' => 'Фото 4 должно быть файлом.',
                'photo_4.mimes' => 'Фото 4 должно быть в формате jpeg, png или jpg.',
                'photo_4.max' => 'Фото 4 не должно превышать 20MB.',
            ]
        );

        if ($validator->fails()) {
            return response(['errors' => $validator->errors()], 422);
        }

        if (!$request->hasFile('photo_1') && !$request->hasFile('photo_2') && !$request->hasFile('photo_3') && !$request->hasFile('photo_4')) {
            $this->createValidationErrorResponse('Хотя бы одно фото должно быть загружено.');
        }

        $report = Report::create([
            'user_id' => Auth::user()->id,
            'worker_id' => Worker::where('user_id', Auth::user()->id)->first()->id,
            'order_id' => $acceptWork->order_id,
            'work_id' => $acceptWork->work_id,
            'work_shift_id' => $workShiftId,
        ]);

        try {
            if ($request->file('photo_1'))
                $this->fileService->storeFile('Report', $report->id, 'photo_1', $request->file('photo_1'), 'tws3_public');
            if ($request->file('photo_2'))
                $this->fileService->storeFile('Report', $report->id, 'photo_2', $request->file('photo_2'), 'tws3_public');
            if ($request->file('photo_3'))
                $this->fileService->storeFile('Report', $report->id, 'photo_3', $request->file('photo_3'), 'tws3_public');
            if ($request->file('photo_4'))
                $this->fileService->storeFile('Report', $report->id, 'photo_4', $request->file('photo_4'), 'tws3_public');
        }
        catch (\Exception $e) {
            $report->delete();
            $this->createValidationErrorResponse('Непредвиденная ошибка. Попробуйте позже');
        }

        broadcast(new UpdateOrder($acceptWork->order_id));
        broadcast(new UpdateAcceptWork($acceptWork->user_id));

        $this->notificationService->createNotification([
            'user_id' => $acceptWork->order->customer->user_id,
            'type' => 'info',
            'title' => 'Отчет о выполнении работы',
            'message' => 'Мастер отправил отчет о выполнении работы.',
            'url' => '/customer/order/'.$acceptWork->order_id.'/detail'
        ]);
        
        return response(['message' => 'Отчет отправлен'], 200);
    }

    public function acceptReport(Request $request, $reportId) {
        $report = Report::find($reportId);

        if (empty($report))
            return $this->createValidationErrorResponse('Отчет не найден');

        $report->status = 'accepted';
        $report->save();

        $report->worker->save();

        $order = Order::find($report->order_id);
        $order->customer->hold_balance -= $report->workShift->price;
        $order->customer->save();

        $this->sberService->createPaymentRequest($order->customer_id, $report->work_shift_id);

        $this->notificationService->createNotification([
            'user_id' => $report->worker->user_id,
            'type' => 'info',
            'title' => 'Отчет принят',
            'message' => 'Ваш отчет принят.',
        ]);

        $nextWorkShift = WorkShift::where([
            ['order_id', $report->order_id],
            ['user_id', $report->user_id],
            ['index', '>', $report->workShift->index]
        ])->orderBy('index', 'asc')->first();

        if (empty($nextWorkShift)) {
            $acceptWork = AcceptWork::where([['user_id', $report->user_id], ['work_id', $report->work_id]])->first();
            $acceptWork->status = 'completed';
            $acceptWork->save();

            $this->notificationService->createNotification([
                'user_id' => $report->worker->user_id,
                'type' => 'success',
                'title' => 'Работа завершена',
                'message' => 'Вы успешно завершили работу по заказу #' . $report->order_id . '.',
                'url' => '/worker/orders'
            ]);
        }

        if ($order->status == 'work') {
            $orderWorkShifts = WorkShift::where('order_id', $report->order_id)->get();

            $allAccepted = true;
            foreach ($orderWorkShifts as $workShift) {
                if (!empty($workShift->report) && $workShift->report->status != 'accepted') {
                    $allAccepted = false;
                    break;
                }
            }

            if ($allAccepted) {
                $order->status = 'done';
                $order->save();

                $this->notificationService->createNotification([
                    'user_id' => $order->customer->user_id,
                    'type' => 'success',
                    'title' => 'Заказ завершен',
                    'message' => 'Ваш заказ #' . $order->id . ' успешно завершен.',
                    'url' => '/customer/order/' . $order->id . '/detail'
                ]);
            }
        }
        

        broadcast(new WorkShiftUpdate($report->user_id));
        broadcast(new UpdateOrder($report->order_id));
        broadcast(new UpdateAcceptWork($report->user_id));
        broadcast(new UpdateBalance($order->customer->user_id, $order->customer->balance));

        return response(['message' => 'Отчет принят'], 200);
    }

    public function refuseReport(Request $request, $reportId) {
        $report = Report::find($reportId);
        $order = Order::find($report->order_id);

        if (empty($report))
            return $this->createValidationErrorResponse('Отчет не найден');

        $report->status = 'refused';
        $report->comment = $request->input('comment', '');

        $report->save();

        $ticket = $this->ticketService->createTicket([
            'title' => 'Отчет отклонен',
            'description' => 'Отчет №' . $report->id . ' был отклонен. Комментарий: ' . $request->input('comment', 'Отчет отклонен без комментариев'),
            'user_id' => $order->user_id,
            'report_id' => $report->id
        ]);

        if ($request->file('filelist')) {
            try {
                foreach ($request->file('filelist') as $index => $file) {
                    $this->fileService->storeFile('Ticket', $ticket->id, 'file'.$index, $file, 'tws3_public');
                }
            }
            catch (\Exception $e) {
                $this->createValidationErrorResponse('Непредвиденная ошибка. Попробуйте позже');
            }
        }
        

        $this->notificationService->createNotification([
            'user_id' => $report->worker->user_id,
            'type' => 'info',
            'title' => 'Отчет отклонен',
            'message' => 'Ваш отчет отклонен.',
            'url' => '/worker/active'
        ]);

        $this->notificationService->sendNotificationByRole([
            'title' => 'Новый тикет в системе',
            'message' => 'Пользователь ' . Auth::user()->name . ' создал новый тикет: "' . $ticket->title . '".',
            'url' => '/support/ticket/' . $ticket->id,
        ], 'manager');

        broadcast(new WorkShiftUpdate($report->user_id));
        broadcast(new UpdateOrder($report->order_id));
        broadcast(new UpdateAcceptWork($report->user_id));

        return response(['message' => 'Отчет отклонен'], 200);
    }

    public function workerNotMeet(Request $request, $workId) {
        $work = Work::find($workId);

        $accept = AcceptWork::where([
            ['user_id', Auth::user()->id],
            ['order_id', $work->order_id],
            ['work_id', $workId],
            ['profession_id', $work->profession_id],
            ['profession_level_id', $work->profession_level_id]
        ])->first();

        if (empty($accept))
            return $this->createValidationErrorResponse('Заказ не найден');

        $this->notificationService->createNotification([
            'user_id' => $accept->user_id,
            'type' => 'warning',
            'title' => 'Вас не встретили на объекте',
            'message' => 'Вы не были встречены на объекте. Заказ отменен. Вам будет начислена компенсация в размере 1500 рублей.',
        ]);
        $this->sberService->createPaymentRequest($accept->order->customer_id, $accept->work->currentWorkShift['id'], 1500);
        $this->workService->refuseWorkerMatch($accept);
        
        broadcast(new UpdateOrder($work->order_id));

        return response(['message' => 'Вы отказались от работы'], 200);
    }
}
