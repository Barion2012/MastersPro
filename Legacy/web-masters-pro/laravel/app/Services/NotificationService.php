<?php 

namespace App\Services;

use App\Events\UserNotification;
use App\Models\Notification;
use App\Models\User;

class NotificationService {
    public function createNotification(array $data) {
        $notification = new Notification();
        $notification->user_id = $data['user_id'];
        $notification->type = $data['type'] ?? 'info';
        $notification->title = $data['title'];
        $notification->message = $data['message'];
        $notification->url = $data['url'] ?? null;
        $notification->save();

        broadcast(new UserNotification($notification));

        return $notification;
    }

    public function sendNotificationByRole(array $data, $role) {
        User::byRole($role)->each(function ($user) use ($data) {
            $this->createNotification(array_merge($data, ['user_id' => $user->id]));
        });
    }

    public function getNotificationsByUserId(int $userId) {
        return Notification::where('user_id', $userId)->get()->map(function ($notification) {
            return [
                'id' => $notification->id,
                'type' => $notification->type,
                'title' => $notification->title,
                'message' => $notification->message,
                'url' => $notification->url,
                'date' => $notification->created_at->format('d.m.Y H:i'),
            ];
        });
    }

    public function deleteNotification(int $notificationId) {
        $notification = Notification::find($notificationId);
        if ($notification) {
            $notification->delete();
            return true;
        }
        return false;
    }

    public function deleteAllNotificationsByUserId(int $userId) {
        Notification::where('user_id', $userId)->delete();
    }
}