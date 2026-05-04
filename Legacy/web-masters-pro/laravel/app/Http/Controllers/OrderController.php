<?php

namespace App\Http\Controllers;

use App\Events\UpdateAcceptWork;
use App\Events\UpdateBalance;
use App\Events\UpdateOrder;
use App\Events\WorkerRefused;
use App\Events\WorkShiftUpdate;
use App\Exports\OrderDetailExport;
use App\Jobs\NotifyWorkers;
use App\Models\AcceptWork;
use App\Models\Customer;
use App\Models\File;
use App\Models\ProfessionLevel;
use App\Models\Order;
use App\Models\User;
use App\Models\Work;
use App\Models\Worker;
use App\Models\WorkShift;
use App\Services\FileService;
use App\Services\NotificationService;
use App\Services\SberService;
use App\Services\WorkService;
use Carbon\Carbon;
use Illuminate\Http\Request;
use Illuminate\Support\Facades\Auth;
use Illuminate\Support\Facades\Log;
use Illuminate\Support\Facades\Validator;
use Maatwebsite\Excel\Facades\Excel;
use PhpOffice\PhpWord\TemplateProcessor;

/**
 * Контроллер управления заказами (Order).
 *
 * Обрабатывает:
 *  - Создание новых заказов с работами
 *  - Получение списка заказов
 *  - Детали заказа
 *  - Отмену заказа
 *  - Утверждение/отказ мастера
 *  - Скачивание документов (Excel, договор)
 *  - Генерацию контракта в DOCX
 */
class OrderController extends Controller
{
    /** @var WorkService Сервис создания и управления работами в заказе */
    protected $workService;
    /** @var NotificationService Сервис отправки уведомлений пользователям */
    protected $notificationService;
    /** @var SberService Сервис интеграции с СБЕР */
    protected $sberService;
    /** @var FileService Сервис управления файлами */
    protected $fileService;

    /**
     * Внедрение зависимостей через конструктор
     *
     * @param WorkService $workService
     * @param NotificationService $notificationService
     * @param SberService $sberService
     * @param FileService $fileService
     */
    public function __construct(WorkService $workService, NotificationService $notificationService, SberService $sberService, FileService $fileService) {
        $this->workService = $workService;
        $this->notificationService = $notificationService;
        $this->sberService = $sberService;
        $this->fileService = $fileService;
    }

    /**
     * Создание нового заказа.
     *
     * Шаги:
     *  1. Валидация данных (массив работ, адреса, даты)
     *  2. Расчёт общей цены заказа
     *  3. Проверка баланса клиента
     *  4. Создание заказа и связанных работ
     *  5. Уведомление подходящих работников (через очередь)
     *
     * @param Request $request
     * @return \Illuminate\Http\JsonResponse
     * @throws \Illuminate\Validation\ValidationException
     */
    public function placeNewOrder(Request $request) {
        // Валидация входных данных: массив работ, адреса, даты
        $request->validate([
            'works' => 'required|array|min:1',
            'works.*.profession_id' => 'required|exists:professions,id',
            'works.*.profession_level_id' => 'required|exists:profession_levels,id',
            'works.*.count' => 'required|integer|min:1',
            'works.*.start_date' => 'required|date',
            'works.*.end_date' => 'required|date|after_or_equal:works.*.start_date',
            'works.*.meet_time' => 'required|date_format:H:i',
            'address_lat' => 'required|numeric',
            'address_lng' => 'required|numeric',
            'address' => 'required|string',
            'meeting_point' => 'required|string',
            'meeting_point_lat' => 'required|numeric',
            'meeting_point_lng' => 'required|numeric',
            'info' => 'nullable|string',
            'info_filelist' => 'nullable|array',
            'info_filelist.*' => 'file|mimes:jpg,jpeg,png,pdf,doc,docx,xls,xlsx',
            'place' => 'nullable|string',
            'quality' => 'nullable|string',
            'quality_filelist' => 'nullable|array',
            'quality_filelist.*' => 'file|mimes:jpg,jpeg,png,pdf,doc,docx,xls,xlsx',
            'place_filelist' => 'nullable|array',
            'place_filelist.*' => 'file|mimes:jpg,jpeg,png,pdf,doc,docx,xls,xlsx',
        ], [
            'works.required' => 'Заказ обязателен для заполнения.',
            'works.array' => 'Заказ должен быть массивом.',
            'works.min' => 'Заказ должен содержать хотя бы один элемент.',
            'works.*.profession_id.required' => 'Выберите профессию',
            'works.*.profession_id.exists' => 'Указанной профессии не существует.',
            'works.*.profession_level_id.required' => 'Выберите уровень',
            'works.*.profession_level_id.exists' => 'Указанный уровень не существует.',
            'works.*.count.required' => 'Кол-во обязательно для заполнения.',
            'works.*.count.integer' => 'Кол-во должно быть целым числом.',
            'works.*.count.min' => 'Кол-во должно быть не меньше 1.',
            'works.*.start_date.required' => 'Дата начала обязательна для заполнения.',
            'works.*.start_date.date' => 'Дата начала должна быть корректной датой.',
            'works.*.end_date.required' => 'Дата окончания обязательна для заполнения.',
            'works.*.end_date.date' => 'Дата окончания должна быть корректной датой.',
            'works.*.end_date.after_or_equal' => 'Дата окончания должна быть не раньше даты начала.',
            'address_lat.required' => 'Укажите адрес объекта',
            'address_lat.numeric' => 'Укажите адрес объекта',
            'address_lng.required' => 'Укажите адрес объекта',
            'address_lng.numeric' => 'Укажите адрес объекта',
            'address.required' => 'Укажите адрес объекта',
            'address.string' => 'Укажите адрес объекта',
            'meeting_point.required' => 'Укажите место встречи',
            'meeting_point.string' => 'Укажите место встречи',
            'meeting_point_lat.required' => 'Укажите место встречи',
            'meeting_point_lat.numeric' => 'Укажите место встречи',
            'meeting_point_lng.required' => 'Укажите место встречи',
            'meeting_point_lng.numeric' => 'Укажите место встречи',
            'info.string' => 'Дополнительная информация должна быть текстом',
            'info_filelist.array' => 'Файлы дополнительной информации должны быть массивом',
            'info_filelist.*.file' => 'Каждый файл дополнительной информации должен быть файлом',
            'info_filelist.*.mimes' => 'Файл должен быть одного из следующих типов: jpg, jpeg, png, pdf, doc, docx, xls, xlsx',
            'place.string' => 'Как добраться должно быть текстом',
            'quality.string' => 'Критерии качества должны быть текстом',
            'quality_filelist.array' => 'Файлы критериев качества должны быть массивом',
            'quality_filelist.*.file' => 'Каждый файл критериев качества должен быть файлом',
            'quality_filelist.*.mimes' => 'Файл должен быть одного из следующих типов: jpg, jpeg, png, pdf, doc, docx, xls, xlsx',
            'place_filelist.array' => 'Файлы места встречи должны быть массивом',
            'place_filelist.*.file' => 'Каждый файл места встречи должен быть файлом',
            'place_filelist.*.mimes' => 'Файл должен быть одного из следующих типов: jpg, jpeg, png, pdf, doc, docx, xls, xlsx',
        ]);

        $totalPrice = 0;
        $works = [];
        // Поиск клиента по текущему пользователю
        $customer = Customer::where('user_id', Auth::user()->id)->first();

        // Если у пользователя нет профиля клиента — блокируем создание заказа
        if (empty($customer)) {
            return $this->createValidationErrorResponse('Профиль клиента не найден.');
        }

        // Расчёт цены для каждой работы
        foreach ($request->works as $workData) {
            // ВНИМАНИЕ: сейчас берём цену из первого уровня профессии, а не из указанного level_id
            //$workData['price_shift'] = ProfessionLevel::find($workData['profession_level_id'])->price;
            $workData['price_shift'] = ProfessionLevel::where('profession_id', $workData['profession_id'])->first()->price;

            $startDate = Carbon::parse($workData['start_date'])->startOfDay();
            $endDate = Carbon::parse($workData['end_date'])->startOfDay();
            $days = $startDate->diffInDays($endDate, false);

            // Цена = цена за смену × кол-во человек × (кол-во дней + 1)
            $totalPrice += $workData['price_shift'] * $workData['count'] * ($days + 1);
            $works[] = $workData;
        }

        // Проверка баланса — если не хватает, отклоняем заказ
        if ($customer->balance < $totalPrice) {
            return $this->createValidationErrorResponse('Недостаточно средств на балансе для создания заказа');
        }

        // Создание заказа
        $order = Order::create([
            'user_id' => Auth::user()->id,
            'customer_id' => $customer->id,
            'address_lat' => $request->address_lat,
            'address_lng' => $request->address_lng,
            'address' => $request->address,
            'meeting_point' => $request->meeting_point,
            'meeting_point_lat' => $request->meeting_point_lat,
            'meeting_point_lng' => $request->meeting_point_lng,
            'info' => isset($request->info) ? $request->info : '',
            'place' => isset($request->place) ? $request->place : '',
            'quality' => isset($request->quality) ? $request->quality : '',
            'total_price' => $totalPrice
        ]);

        try {
            // Создание связанных работ через сервис
            foreach ($works as $workData) {
                $this->workService->createWork([
                    'order_id' => $order->id,
                    'profession_id' => $workData['profession_id'],
                    // Здесь тоже берём первый level_id по профессии, а не переданный
                    //'profession_level_id' => $workData['profession_level_id'],
                    'profession_level_id' => ProfessionLevel::where('profession_id', $workData['profession_id'])->first()->id,
                    'count' => $workData['count'],
                    'start_date' => Carbon::parse($workData['start_date'])
                        ->setTimeFromTimeString($workData['meet_time'])
                        ->format('Y.m.d H:i'),
                    'end_date' => Carbon::parse($workData['end_date'])
                        ->setTimeFromTimeString($workData['meet_time'])
                        ->format('Y.m.d H:i'),
                    'price_shift' => $workData['price_shift'],
                ]);
            }

            // Сохранение файлов дополнительной информации
            if (isset($request->info_filelist)) {
                foreach ($request->info_filelist as $infoFile) {
                    $fileModel = $this->fileService->storeFile('Order', $order->id, 'info_file', $infoFile, 'tws3_public');
                }
            }

            // Сохранение файлов критериев качества
            if (isset($request->quality_filelist)) {
                foreach ($request->quality_filelist as $qualityFile) {
                    $fileModel = $this->fileService->storeFile('Order', $order->id, 'quality_file', $qualityFile, 'tws3_public');
                }
            }

            // Сохранение файлов места встречи
            if (isset($request->place_filelist)) {
                foreach ($request->place_filelist as $placeFile) {
                    $fileModel = $this->fileService->storeFile('Order', $order->id, 'place_file', $placeFile, 'tws3_public');
                }
            }
        }
        catch (\Exception $e) {
            // Откат при ошибке
            $order->delete();
            $this->createValidationErrorResponse($e->getMessage());
        }

        // Списываем деньги с баланса и переводим в удержание
        $customer->balance -= $totalPrice;
        $customer->hold_balance += $totalPrice;
        $customer->save();

        // Обновление баланса у клиента
        broadcast(new UpdateBalance($customer->user_id, $customer->balance));

        // Асинхронная рассылка уведомлений подходящим работникам. ОТКЛЮЧЕНО
        //NotifyWorkers::dispatch($order);

        // Успешный ответ с ID созданного заказа
        return response(['status' => 'success', 'message' => 'Заказ успешно создан', 'orderId' => $order->id], 200);
    }

    /**
     * Получение детальной информации по заказу (для клиента / админа)
     *
     * @param Request $request
     * @param int $orderId
     * @return array
     */
    public function getOrder(Request $request, $orderId) {
        // Загружаем заказ со всеми нужными связями
        $order = Order::with([
            'acceptWorks.user',
            'reports',
            'reports.work',
            'reports.files',
        ])->find($orderId);

        $user = User::find(Auth::user()->id);

        if (empty($order))
            return $this->createValidationErrorResponse('Заказ не найден');

        // Проверка прав: либо владелец, либо есть разрешение на чтение заказов
        if ($order->user_id != $user->id && !$user->checkPermission('Order', 'read'))
            return $this->createValidationErrorResponse('Доступ запрещен');

        $workers = [];
        $reports = [];
        $works = [];
        
        // Формируем массив принятых работников
        foreach ($order->acceptWorks as $acceptWork) {
            $workers[] = [
                'user_id' => $acceptWork->user->id,
                'worker_id' => $acceptWork->worker->id,
                'name' => $acceptWork->user->name,
                'profession' => $acceptWork->profession->name,
                'professionLevel' => $acceptWork->professionLevel->desc,
                'citizenship' => $acceptWork->worker->citizenship,
                'confirm_address' => $acceptWork->confirm_address,
                'confirm_meeting' => $acceptWork->confirm_meeting,
                'status' => $acceptWork->status,
                'current_work_shift' => $acceptWork->worker->currentWorkShift,
                'files' => [
                    'passport_scan' => $acceptWork->worker->getFileId('passport_scan'),
                    'snils' => $acceptWork->worker->getFileId('snils'),
                    'migration_card' => $acceptWork->worker->getFileId('migration_card'),
                    'patent' => $acceptWork->worker->getFileId('patent'),
                    'patent_cheque' => $acceptWork->worker->getFileId('patent_cheque'),
                    'dms' => $acceptWork->worker->getFileId('dms'),
                ]
            ];
        }

        // Формируем массив отчётов по сменам
        foreach ($order->reports as $report) {
            $reports[] = [
                'id' => $report->id,
                'worker_id' => $report->worker->id,
                'name' => $report->user->name,
                'profession' => $report->work->profession->name,
                'professionLevel' => $report->work->professionLevel->desc,
                'index' => $report->workShift->index,
                'date' => $report->workShift->date,
                'photo_1' => $report->getFile('photo_1'),
                'photo_2' => $report->getFile('photo_2'),
                'photo_3' => $report->getFile('photo_3'),
                'photo_4' => $report->getFile('photo_4'),
                'status' => $report->status,
                'comment' => $report->comment,
            ];
        }

        // Формируем массив работ в заказе
        foreach ($order->works as $work) {
            $works[] = [
                'id' => $work->id,
                'profession' => $work->profession->name,
                'professionLevel' => $work->professionLevel->desc,
                'count' => $work->count,
                'found' => $work->found,
                'refused' => $work->refused,
                'dates' => Carbon::parse($work->start_date)->format('d.m.Y') . ' - ' . Carbon::parse($work->end_date)->format('d.m.Y'),
                'price_shift' => $work->price_shift,
                'status' => $work->status,
            ];
        }

        // Возвращаем заказ + работников + отчёты + работы
        return [
            'order' => $this->serializeOrder($order),
            'workers' => $workers,
            'reports' => $reports,
            'works' => $works
        ];
    }

    /**
     * Список заказов конкретного пользователя (для личного кабинета клиента)
     * 
     * @param Request $request
     * @param int $userId
     * @return array
     */
    public function getUserOrdersList(Request $request, $userId) {
        $user = User::find(Auth::user()->id);

        // Проверка прав: либо свои заказы, либо есть разрешение на чтение чужих
        if ($userId != $user->id && !$user->checkPermission('Order', 'read'))
            return $this->createValidationErrorResponse('Доступ запрещен');

        $orders = Order::where('user_id', $userId)->paginate(10);

        return [
            'data' => $orders->map(function ($item) {
                return $this->serializeOrder($item);
            })->toArray(),
            'current_page' => $orders->currentPage(),
            'last_page' => $orders->lastPage(),
        ];
    }

    /**
     * Общий список всех заказов (для админки)
     * 
     * @param Request $request
     * @return array
     */
    public function getOrdersList(Request $request) {
        $orders = Order::paginate(10);

        return [
            'data' => $orders->map(function ($item, $key) use ($orders) {
                $index = ($orders->currentPage() - 1) * $orders->perPage() + $key + 1;
                $orderData = $this->serializeOrder($item);
                $orderData['index'] = $index; // Нумерация строк в таблице
                return $orderData;
            })->toArray(),
            'current_page' => $orders->currentPage(),
            'last_page' => $orders->lastPage(),
        ];
    }

    /**
     * Приватный метод — приводит модель Order к единому виду для API
     * 
     * @param Order $order
     * @return array
     */
    private function serializeOrder($order) {
        return [
            'id' => $order->id,
            'created_at' => Carbon::parse($order->created_at)->format('d.m.Y'),
            'user_id' => $order->user_id,
            'user' => $order->user->name,
            'customer_id' => $order->customer_id,
            'customer' => $order->customer->user->name,
            'status' => $order->status,
            'address_lat' => $order->address_lat,
            'address_lng' => $order->address_lng,
            'address' => $order->address,
            'meeting_point_lat' => $order->meeting_point_lat,
            'meeting_point_lng' => $order->meeting_point_lng,
            'meeting_point' => $order->meeting_point,
            'info' => $order->info,
            'place' => $order->place,
            'quality' => $order->quality,
            'total_price' => $order->total_price,
            'currency' => $order->currency,
            'order_files' => [
                'info_file' => $order->getFile('info_file'),
                'quality_file' => $order->getFile('quality_file'),
            ]
        ];
    }

    /**
     * Скачивание файла работника (паспорт, СНИЛС и т.д.)
     * Доступно клиенту (владелец заказа) и самому работнику 
     * 
     * @param Request $request
     * @param int $orderId
     * @param int $workerId
     * @param int $fileId
     * @return \Illuminate\Http\RedirectResponse
     */
    public function getWorkerFile(Request $request, $orderId, $workerId, $fileId) {
        // Валидация параметров URL
        $validator = Validator::make(
            [
                'orderId' => $orderId,
                'fileId' => $fileId,
                'workerId' => $workerId,
            ],
            [
                'orderId' => 'required|integer|exists:orders,id',
                'fileId' => 'required|integer|exists:files,id',
                'workerId' => 'required|integer|exists:workers,id',
            ],
            [
                'orderId.required' => 'Идентификатор заказа обязателен.',
                'orderId.integer' => 'Идентификатор заказа должен быть числом.',
                'orderId.exists' => 'Указанная заказ не существует.',
                'fileId.required' => 'Идентификатор файла обязателен.',
                'fileId.integer' => 'Идентификатор файла должен быть числом.',
                'fileId.exists' => 'Указанный файл не существует.',
                'workerId.required' => 'Идентификатор работника обязателен.',
                'workerId.integer' => 'Идентификатор работника должен быть числом.',
                'workerId.exists' => 'Указанный идентификатор работника не существует.',
            ]
        );

        if ($validator->fails()) {
            return response(['errors' => $validator->errors()], 422);
        }

        $order = Order::find($orderId);
        $file = File::find($fileId);
        $user = User::find(Auth::user()->id);
        $worker = Worker::find($workerId);

        // Работник может скачивать свои же файлы, если он привязан к заказу
        if (
            $worker->user_id == $user->id && 
            !empty(AcceptWork::where([['order_id', $orderId], ['user_id', $user->id]])->first()) && 
            $file->model_type == 'App\Models\Worker' &&
            $file->model_id == $worker->id
        ) {
            return $this->fileService->getFileDownload($fileId);
        }

        // Клиент-владелец заказа тоже имеет доступ
        if ($order->user_id !== Auth::user()->id && !$user->checkPermission('Order', 'read'))
            $this->createValidationErrorResponse('Доступ запрещен');

        // Проверка, что работник действительно привязан к заказу
        if (empty(AcceptWork::where([['order_id', $orderId], ['worker_id', $workerId]])->first()))
            $this->createValidationErrorResponse('Работник не относится к этому заказу');

        // Возвращаем временную ссылку на файл 
        return $this->fileService->getFileDownload($fileId);
    }

    /**
     * Утверждение мастера клиентом
     * 
     * @param Request $request
     * @param int $orderId
     * @return \Illuminate\Http\JsonResponse
     */
    public function acceptWorker(Request $request, $orderId) {
        $acceptWork = AcceptWork::where([['worker_id', $request->worker_id], ['work_id', $orderId]])->first();

        if (empty($acceptWork))
            return $this->createValidationErrorResponse('Заказ не найден');

        $acceptWork->status = 'accepted';
        $acceptWork->save();

        // Оповещаем всех слушателей об изменении заказа и смен
        broadcast(new UpdateOrder($acceptWork->order_id));
        broadcast(new WorkShiftUpdate($acceptWork->user_id));
        broadcast(new UpdateAcceptWork($acceptWork->user_id));

        return response(['message' => 'Мастер утвержден'], 200);
    }

    /**
     * Отказ от мастера (клиентом)
     * Если мастер уже приехал — создаём платёжку на неустойку через СБЕР
     * 
     * @param Request $request
     * @param int $orderId
     * @return \Illuminate\Http\JsonResponse
     */
    public function refuseWorker(Request $request, $orderId) {
        $acceptWork = AcceptWork::where([['worker_id', $request->worker_id], ['work_id', $orderId]])->first();

        if (empty($acceptWork))
            return $this->createValidationErrorResponse('Заказ не найден');

        $workShiftCount = WorkShift::where([['user_id', $acceptWork->user_id], ['work_id', $acceptWork->work_id]])->count();
        $totalSum = $workShiftCount * $acceptWork->work->price_shift;

        // Возвращаем деньги клиенту
        $customer = $acceptWork->order->customer;
        $customer->balance += $totalSum;
        $customer->hold_balance -= $totalSum;
        $customer->save();

        // Если мастер уже приехал или его приняли и 3 часа не прошло — создаём платёжку на неустойку через СБЕР
        if ($acceptWork->status == 'arrive' || ($acceptWork->status == 'accepted' && !$acceptWork->worker->currentWorkShift['three_hours_passed'])) {
            $this->sberService->createPaymentRequest($acceptWork->order->customer_id, $acceptWork->work->currentWorkShift['id'], 1500);
            $this->notificationService->createNotification([
                'user_id' => $acceptWork->user_id,
                'type' => 'info',
                'title' => 'Выплата неустойки',
                'message' => 'Заказчик отказался от ваших услуг после вашего приезда. Вам будет выплачена неустойка в размере 1500 руб.',
                'url' => '/worker/dashboard'
            ]);
        }

        // Удаляем все смены этого работника по этому заказу
        WorkShift::where([['user_id', $acceptWork->user_id], ['work_id', $acceptWork->work_id]])->delete();

        // Обновляем счётчики найденных/отказанных в работе
        $acceptWork->work->found--;
        $acceptWork->work->refused++;
        $acceptWork->work->save();

        $acceptWork->status = 'refused';
        $acceptWork->save();

        // Уведомление работнику
        $this->notificationService->createNotification([
            'user_id' => $acceptWork->user_id,
            'type' => 'info',
            'title' => 'Вы получили отказ',
            'message' => 'Заказчик отказался от ваших услуг',
            'url' => '/worker/dashboard'
        ]);

        // Оповещаем всех слушателей об изменении заказа и отказе работника
        broadcast(new UpdateOrder($acceptWork->order_id));
        broadcast(new WorkerRefused($acceptWork->user_id, $acceptWork->work_id));
        broadcast(new UpdateBalance($customer->user_id, $customer->balance));
        broadcast(new UpdateAcceptWork($acceptWork->user_id));

        return response(['message' => 'Вы отказались от мастера'], 200);
    }

    /**
     * Скачивание Excel-отчёта по заказу (детализация)
     * 
     * @param Request $request
     * @param int $orderId
     * @return \Symfony\Component\HttpFoundation\BinaryFileResponse|\Illuminate\Http\JsonResponse
     */
    public function downloadOrderDetailExcel(Request $request, $orderId) {
        $order = Order::find($orderId);
        $user = User::find(Auth::user()->id);

        if (empty($order))
            return response(['message' => 'Заказ не найден'], 422);

        if ($order->user_id != $user->id && !$user->checkPermission('Order', 'read'))
            return response(['message' => 'Доступ запрещен'], 403);

        return Excel::download(new OrderDetailExport($orderId), "order-$orderId.xlsx");
    }

    /**
     * Получение готового договора (DOCX) между клиентом и работником
     * 
     * @param Request $request
     * @param int $orderId
     * @param int $workerId
     * @return \Illuminate\Http\RedirectResponse|\Illuminate\Http\JsonResponse
     */
    public function getWorkerContract(Request $request, $orderId, $workerId) {
        // Валидация параметров
        $validator = Validator::make(
            [
                'orderId' => $orderId,
                'workerId' => $workerId,
            ],
            [
                'orderId' => 'required|integer|exists:orders,id',
                'workerId' => 'required|integer|exists:workers,id',
            ],
            [
                'orderId.required' => 'Идентификатор заказа обязателен.',
                'orderId.integer' => 'Идентификатор заказа должен быть числом.',
                'orderId.exists' => 'Указанная заказ не существует.',
                'workerId.required' => 'Идентификатор работника обязателен.',
                'workerId.integer' => 'Идентификатор работника должен быть числом.',
                'workerId.exists' => 'Указанный идентификатор работника не существует.',
            ]
        );

        if ($validator->fails()) {
            return response(['errors' => $validator->errors()], 422);
        }

        $order = Order::find($orderId);
        $user = User::find(Auth::user()->id);
        $worker = Worker::find($workerId);

        // Работник может скачать свой договор
        if (
            $worker->user_id == $user->id && 
            !empty(AcceptWork::where([['order_id', $orderId], ['user_id', $user->id]])->first())
        ) {
            return $this->generateContract($order->customer_id, $workerId, $orderId);
        }
        // Клиент-владелец тоже имеет доступ
        if ($order->user_id !== Auth::user()->id && !$user->checkPermission('Order', 'read'))
            $this->createValidationErrorResponse('Доступ запрещен');

        // Проверка привязки работника к заказу
        if (empty(AcceptWork::where([['order_id', $orderId], ['worker_id', $workerId]])->first()))
            $this->createValidationErrorResponse('Работник не относится к этому заказу');

        return $this->generateContract($order->customer_id, $workerId, $orderId);
    }

    /**
     * Генерация DOCX-договора из шаблона
     *
     * @param int $customerId
     * @param int $workerId
     * @param int $orderId
     * @return \Illuminate\Http\Response
     */
    private function generateContract($customerId, $workerId, $orderId) {
        $customer = Customer::find($customerId);
        $worker = Worker::find($workerId);

        // Загружаем шаблон
        $templateProcessor = new TemplateProcessor(storage_path('app/docs/templates/contract_template.docx'));

        // Заполняем плейсхолдеры
        $templateProcessor->setValue('ORDER_ID', $orderId);

        $templateProcessor->setValue('CUSTOMER_NAME', $customer->user->name);
        $templateProcessor->setValue('CUSTOMER_BASE', $customer->type == 'ip' ? 'Свидетельства регистрации' : 'Устава');
        if (!empty($customer->info['postAddress'])) {
            $templateProcessor->setValue('CUSTOMER_ADDRESS', $customer->info['postAddress']);
        }
        if (!empty($customer->info['legalAddress'])) {
            $templateProcessor->setValue('CUSTOMER_ADDRESS', $customer->info['legalAddress']);
        }
        $templateProcessor->setValue('CUSTOMER_INN', $customer->info['inn']);
        $templateProcessor->setValue('CUSTOMER_KPP', empty($customer->info['kpp']) ? '' : 'КПП '.$customer->info['kpp']);
        if (!empty($customer->info['ogrn'])) {
            $templateProcessor->setValue('CUSTOMER_OGRN', 'ОГРН '.$customer->info['ogrn']);
        }
        if (!empty($customer->info['ogrnip'])) {
            $templateProcessor->setValue('CUSTOMER_OGRN', 'ОГРНИП '.$customer->info['ogrnip']);
        }
            
        $templateProcessor->setValue('CUSTOMER_BANK_NAME', $customer->info['bankName']);
        $templateProcessor->setValue('CUSTOMER_ACCOUNT_NUMBER', $customer->info['accountNumber']);
        $templateProcessor->setValue('CUSTOMER_CORRESPONDENT_ACCOUNT', $customer->info['bankCorAccount']);
        $templateProcessor->setValue('CUSTOMER_BIK', $customer->info['bankBIC']);

        $templateProcessor->setValue('WORKER_NAME', $worker->user->name);
        $templateProcessor->setValue('WORKER_INN', $worker->inn);
        $templateProcessor->setValue('WORKER_SNILS', $worker->snils);
        $templateProcessor->setValue('WORKER_BANK_NAME', $worker->bank_name);
        $templateProcessor->setValue('WORKER_ACCOUNT_NUMBER', $worker->account_number);
        $templateProcessor->setValue('WORKER_CORRESPONDENT_ACCOUNT', $worker->bank_cor_account);
        $templateProcessor->setValue('WORKER_BIK', $worker->bank_bic);

        // Сохраняем временный файл
        $outputPath = storage_path("app/temp/contract_{$orderId}_{$workerId}.docx");
        $templateProcessor->saveAs($outputPath);

        // Отдаём файл и удаляем после скачивания
        return response()->download($outputPath)->deleteFileAfterSend(true);
    }
}
