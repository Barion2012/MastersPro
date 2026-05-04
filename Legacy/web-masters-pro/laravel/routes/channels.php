<?php

use App\Models\Ticket;
use Illuminate\Support\Facades\Auth;
use Illuminate\Support\Facades\Broadcast;

Broadcast::channel('user-notification.{id}', function ($user, $id) {
    return Auth::check();
});

Broadcast::channel('ticket-notification.{id}', function ($user, $id) {
    $ticket = Ticket::find($id);

    return $ticket->user_id == Auth::user()->id || $user->checkPermission('Ticket', 'handle');
});