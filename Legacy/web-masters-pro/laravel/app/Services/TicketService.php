<?php 

namespace App\Services;

use App\Models\Ticket;
use Carbon\Carbon;

class TicketService {
    public function getSupportTicketsList() {
        $tickets = Ticket::select('id', 'title', 'description', 'created_at', 'user_id', 'status')
            ->with('user:id,name,role,phone')
            ->orderBy('created_at', 'desc')
            ->paginate(10);
        
        foreach ($tickets as $ticket) {
            $ticket->created = Carbon::parse($ticket->created_at)->format('d.m.Y H:i');
        }
        
        return $tickets;
    }

    public function getSupportTicketById(int $ticketId) {
        $ticket = Ticket::with([
            'user:id,name,role',
            'messages.user:id,name',
            'report.user:id,name,role',
            'report.files',
            'messages.files',
            ])
            ->select('id', 'title', 'description', 'created_at', 'user_id', 'report_id', 'status')
            ->findOrFail($ticketId);

        $result = [
            'id' => $ticket->id,
            'title' => $ticket->title,
            'description' => $ticket->description,
            'created' => Carbon::parse($ticket->created_at)->format('d.m.Y H:i'),
            'user' => [
                'id' => $ticket->user->id,
                'name' => $ticket->user->name,
                'role' => $ticket->user->role,
            ],
            'status' => $ticket->status,
            'messages' => $ticket->messages->map(function ($message) {
                return [
                    'id' => $message->id,
                    'message' => $message->message,
                    'created_at' => Carbon::parse($message->created_at)->format('d.m.Y H:i'),
                    'user' => [
                        'id' => $message->user->id,
                        'name' => $message->user->name,
                    ],
                    'files' => $message->files->map(function ($file) {
                        return [
                            'id' => $file->id,
                            'permanent_url' => $file->permanent_url,
                            'name' => $file->filename,
                        ];
                    })->values(),
                ];
            })->values(),
            'report' => $ticket->report ? [
                'id' => $ticket->report->id,
                'user' => [
                    'id' => $ticket->report->user->id,
                    'name' => $ticket->report->user->name,
                    'role' => $ticket->report->user->role,
                ],
                'files' => $ticket->report->files->map(function ($file) {
                    return [
                        'id' => $file->id,
                        'permanent_url' => $file->permanent_url, // accessor getPermanentUrlAttribute
                    ];
                })->values(),
            ] : null,
            'files' => $ticket->files->map(function ($file) {
                return [
                    'id' => $file->id,
                    'permanent_url' => $file->permanent_url, // accessor getPermanentUrlAttribute
                    'name' => $file->filename,
                ];
            })->values(),
        ];

        return $result;
    }

    public function getSupportTicketsListByUserId($userId) {
        $tickets = Ticket::select('id', 'title', 'description', 'created_at', 'user_id', 'status')
            ->where('user_id', $userId)
            ->with('user:id,name,role,phone')
            ->orderBy('created_at', 'desc')
            ->paginate(10);

        foreach ($tickets as $ticket) {
            $ticket->created = Carbon::parse($ticket->created_at)->format('d.m.Y H:i');
        }

        return $tickets;
    }

    public function getTicketById(int $ticketId) {
        return Ticket::findOrFail($ticketId);
    }

    public function createTicket(array $data) {
        $ticket = Ticket::create([
            'title' => $data['title'],
            'description' => $data['description'],
            'user_id' => $data['user_id'], 
            'report_id' => $data['report_id'] ?? null,
        ]);
        return $ticket;
    }

    public function createTicketMessage(array $data, int $ticketId) {
        $ticket = Ticket::find($ticketId);

        $ticket->messages()->create([
            'message' => $data['message'],
            'user_id' => $data['user_id'],
            'ticket_id' => $ticketId,
        ]);

        return $ticket;
    }

    public function changeTicketStatus(int $ticketId, string $status) {
        $ticket = Ticket::findOrFail($ticketId);
        $ticket->status = $status;
        $ticket->save();
        return $ticket;
    }
}