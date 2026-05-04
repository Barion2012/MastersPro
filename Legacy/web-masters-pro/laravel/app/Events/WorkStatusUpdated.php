<?php

namespace App\Events;

use Illuminate\Broadcasting\Channel;
use Illuminate\Broadcasting\InteractsWithSockets;
use Illuminate\Broadcasting\PresenceChannel;
use Illuminate\Broadcasting\PrivateChannel;
use Illuminate\Contracts\Broadcasting\ShouldBroadcast;
use Illuminate\Foundation\Events\Dispatchable;
use Illuminate\Queue\SerializesModels;
use App\Models\Work;
use Illuminate\Support\Facades\Log;

class WorkStatusUpdated
{
    use Dispatchable, InteractsWithSockets, SerializesModels;

    public $work;

    /**
     * Create a new event instance.
     */
    public function __construct(Work $work)
    {
        $this->work = $work;
    }
}
