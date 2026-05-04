<?php

namespace App\Events;

use App\Models\Notification;
use Carbon\Carbon;
use Illuminate\Broadcasting\Channel;
use Illuminate\Broadcasting\InteractsWithSockets;
use Illuminate\Broadcasting\PresenceChannel;
use Illuminate\Broadcasting\PrivateChannel;
use Illuminate\Contracts\Broadcasting\ShouldBroadcast;
use Illuminate\Contracts\Broadcasting\ShouldBroadcastNow;
use Illuminate\Foundation\Events\Dispatchable;
use Illuminate\Queue\SerializesModels;

class UserNotification implements ShouldBroadcastNow
{
    use Dispatchable, InteractsWithSockets, SerializesModels;

    public $userId;
    public $title;
    public $message;
    public $type;
    public $date;
    public $url;

    /**
     * Create a new event instance.
     */
    public function __construct(Notification $notification)
    {
        $this->userId = $notification->user_id;
        $this->title = $notification->title;
        $this->message = $notification->message;
        $this->type = $notification->type ?? 'info';
        $this->date = Carbon::parse($notification->created_at)->format('d.m.Y H:i');
        $this->url = $notification->url ?? null;
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
