<?php

namespace App\Listeners;

use App\Events\WorkStatusUpdated;
use App\Services\WorkService;
use Illuminate\Contracts\Queue\ShouldQueue;
use Illuminate\Support\Facades\Log;

class HandleWorkStatusUpdated implements ShouldQueue
{
    protected $workService;

    /**
     * Create the event listener.
     */
    public function __construct(WorkService $workService)
    {
        $this->workService = $workService;
    }

    /**
     * Handle the event.
     */
    public function handle(WorkStatusUpdated $event)
    {
        $this->workService->updateWorkStatus($event->work);
    }
}
