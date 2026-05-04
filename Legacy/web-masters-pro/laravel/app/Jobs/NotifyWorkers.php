<?php

namespace App\Jobs;

use App\Events\WorkAvailable;
use App\Models\AcceptWork;
use Illuminate\Contracts\Queue\ShouldQueue;
use Illuminate\Foundation\Bus\Dispatchable;
use Illuminate\Foundation\Queue\Queueable;
use Illuminate\Queue\InteractsWithQueue;
use Illuminate\Queue\SerializesModels;

use App\Models\Order;
use App\Models\WorkerProfession;
use Carbon\Carbon;
use Illuminate\Support\Facades\Log;

class NotifyWorkers implements ShouldQueue
{
    use Queueable, Dispatchable, InteractsWithQueue, SerializesModels;

    /**
     * Create a new job instance.
     */
    protected $order;

    public function __construct(Order $order)
    {
        $this->order = $order;
    }

    /**
     * Execute the job.
     */
    public function handle(): void
    {
        $this->order->load('works');
        
        foreach($this->order->works as $work) {
            if ($work->found >= $work->count)
                continue;

            if (Carbon::parse($work->end_date)->isPast())
                continue;

            $workerProfessions = WorkerProfession::where('profession_level_id', $work->profession_level_id)->get();
            
            foreach($workerProfessions as $workerProfession) {
                $accept = AcceptWork::where([
                    ['user_id', $workerProfession->worker->user_id],
                    ['order_id', $this->order->id],
                    ['work_id', $work->id],
                    ['profession_id', $work->profession_id],
                    ['profession_level_id', $work->profession_level_id]
                ])->first();

                if (empty($accept))
                    broadcast(new WorkAvailable($this->order->id, $work->id, $workerProfession->worker->user_id));
            }
        }

        
    }
}
