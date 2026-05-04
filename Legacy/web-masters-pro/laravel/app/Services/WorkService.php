<?
namespace App\Services;

use App\Events\OrderStatusChanged;
use App\Events\UpdateAcceptWork;
use App\Events\UpdateOrder;
use App\Events\WorkUnavailable;
use App\Models\AcceptWork;
use App\Models\Work;
use App\Models\Order;
use App\Models\WorkShift;
use Carbon\Carbon;
use Illuminate\Support\Facades\Log;

class WorkService {
    public function createWork(array $data) {
        return Work::create($data);
    }

    public function getUserOrderWorks($userId, $perPage = 10) {
        $works = Work::where('user_id', $userId)->paginate($perPage);

        return [
            'data' => $works->map(function ($work) {
                return $this->serializeWork($work);
            })->toArray(),
            'current_page' => $works->currentPage(),
            'last_page' => $works->lastPage(),
        ];
    }

    public function getWorkById($workId) {
        $work = Work::find($workId);
        
        return $this->serializeWork($work);
    }

    public function serializeWork($work) {
        return $work->serialize();
    }

    public function updateWorkStatus(Work $work)
    {
        if ($work->status === 'completed' || $work->status === 'expired') {
            return;
        }

        $status = $work->found < $work->count ? 'search' : 'work';
        if ($work->status !== $status) {
            $work->status = $status;
            $work->saveQuietly();
        }

        $this->checkOrderStatus($work->order);
    }

    public function checkOrderStatus(Order $order)
    {
        if (!$order) {
            return;
        }

        $order->load('works');
        $works = $order->works;

        if ($works->isEmpty()) {
            return;
        }

        $workCount = $works->count();
        $workInSearch = $works->where('status', 'search')->count();
        $workInWork = $works->where('status', 'work')->count();
        $workInCompleted = $works->where('status', 'completed')->count();
        $workInExpired = $works->where('status', 'expired')->count();

        $newStatus = $order->status;

        if ($workInSearch > 0) {
            $newStatus = 'search';
        } elseif ($workInSearch == 0 && $workInWork > 0) {
            $newStatus = 'work';
        } elseif (($workInCompleted + $workInExpired) === $workCount) {
            $newStatus = 'done';
        }

        if ($order->status !== $newStatus) {
            $order->status = $newStatus;
            $order->saveQuietly();
            event(new OrderStatusChanged($order));
        }
    }

    public function matchWorker($work, $worker) {
        AcceptWork::create([
            'user_id' => $worker->user_id,
            'worker_id' => $worker->id,
            'order_id' => $work->order_id,
            'work_id' => $work->id,
            'profession_id' => $work->profession_id,
            'profession_level_id' => $work->profession_level_id,
            'status' => 'matched'
        ]);

        $work->found++;
        $work->save();

        $startDateTime = Carbon::parse($work->start_date);
        $endDateTime = Carbon::parse($work->end_date);

        // считаем дни включительно: с 20 по 20 => 1, с 20 по 21 => 2, с 20 по 27 => 8
        $startDay = $startDateTime->copy()->startOfDay();
        $endDay = $endDateTime->copy()->startOfDay();
        $daysInclusive = $startDay->diffInDays($endDay) + 1;
        if ($daysInclusive < 1) {
            $daysInclusive = 1;
        }

        for ($i = 0; $i < $daysInclusive; $i++) {
            // сохраняем дату и время из start_date для каждой смены, увеличивая день на i
            $shiftDateTime = $startDateTime->copy()->addDays($i);
            $shiftDate = $shiftDateTime->format('d.m.Y H:i');

            WorkShift::create([
            'user_id' => $worker->user_id,
            'order_id' => $work->order_id,
            'work_id' => $work->id,
            'date' => $shiftDate,
            'index' => $i + 1,
            'price' => $work->price_shift,
            'currency' => $work->currency,
            ]);
        }

        if ($work->found >= $work->count) {
            broadcast(new WorkUnavailable($work->id));
        }

        broadcast(new UpdateOrder($work->order_id));
        broadcast(new UpdateAcceptWork($worker->user_id));
    }

    public function refuseWorkerMatch($acceptWork) {
        $acceptWork->status = 'refused';
        $acceptWork->save();

        $work = Work::find($acceptWork->work_id);

        $work->found--;
        $work->refused++;
        $work->save();

        WorkShift::where([['order_id', $work->order_id], ['user_id', $acceptWork->user_id]])->delete();

        broadcast(new UpdateOrder($work->order_id));
        broadcast(new UpdateAcceptWork($acceptWork->user_id));
    }
}