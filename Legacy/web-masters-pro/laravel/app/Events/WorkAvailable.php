<?php

namespace App\Events;

use App\Models\Order;
use App\Models\Work;
use Illuminate\Broadcasting\Channel;
use Illuminate\Broadcasting\InteractsWithSockets;
use Illuminate\Broadcasting\PresenceChannel;
use Illuminate\Broadcasting\PrivateChannel;
use Illuminate\Contracts\Broadcasting\ShouldBroadcast;
use Illuminate\Contracts\Broadcasting\ShouldBroadcastNow;
use Illuminate\Foundation\Events\Dispatchable;
use Illuminate\Queue\SerializesModels;

class WorkAvailable implements ShouldBroadcastNow
{
    use Dispatchable, InteractsWithSockets, SerializesModels;

    /**
     * Create a new event instance.
     */
    public $address_lat;
    public $address_lng;
    public $address;
    public $meeting_point_lat;
    public $meeting_point_lng;
    public $meeting_point;
    public $work;
    public $workId;
    public $userId;

    public function __construct($orderId, $workId, $userId)
    {
        $order = Order::find($orderId);
        $work = Work::find($workId);

        $this->address_lat = $order->address_lat;
        $this->address_lng = $order->address_lng;
        $this->address = $order->address;
        $this->meeting_point_lat = $order->meeting_point_lat;
        $this->meeting_point_lng = $order->meeting_point_lng;
        $this->meeting_point = $order->meeting_point;
        $this->workId = $work->id;
        $this->work = $work->serialize();
        $this->userId = $userId;
    }

    /**
     * Get the channels the event should broadcast on.
     *
     * @return array<int, \Illuminate\Broadcasting\Channel>
     */
    public function broadcastOn(): array
    {
        return [
            new PrivateChannel('user-notification.'.$this->userId),
        ];
    }
}
