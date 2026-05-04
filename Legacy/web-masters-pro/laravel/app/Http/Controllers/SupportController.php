<?php

namespace App\Http\Controllers;

use App\Events\SendTicketMessage;
use App\Models\Ticket;
use App\Services\FileService;
use Illuminate\Http\Request;
use App\Services\NotificationService;
use App\Services\TicketService;
use Illuminate\Support\Facades\Auth;
use Illuminate\Support\Facades\Validator;

class SupportController extends Controller
{
    protected $notificationService;
    protected $ticketService;
    protected $fileService;

    public function __construct(NotificationService $notificationService, TicketService $ticketService, FileService $fileService) {
        $this->notificationService = $notificationService;
        $this->ticketService = $ticketService;
        $this->fileService = $fileService;
    }

    public function getSupportTicketsList(Request $request) {
        return $this->ticketService->getSupportTicketsList();
    }

    public function getSupportTicketsById(Request $request, $ticketId) {
        $validator = Validator::make(
            ['ticket_id' => $ticketId],
            ['ticket_id' => 'required|exists:tickets,id'],
            [
                'ticket_id.required' => 'ID тикета обязателен.',
                'ticket_id.exists' => 'Тикет не найден.',
            ]
        );

        if ($validator->fails()) {
            return response()->json(['error' => $validator->errors()->first('ticket_id')], 404);
        }

        return $this->ticketService->getSupportTicketById($ticketId);
    }

    public function createTicket(Request $request) {
        $request->validate([
            'title' => 'required|string|max:255',
            'description' => 'required|string',
            'filelist' => 'nullable|array',
            'filelist.*' => 'file|mimes:jpg,jpeg,png,pdf,doc,docx|max:4096',
        ], [
            'title.required' => 'Тема обращения обязательна для заполнения.',
            'title.string' => 'Тема обращения должена быть строкой.',
            'title.max' => 'Тема обращения не должна превышать 255 символов.',
            'description.required' => 'Описание обязательно для заполнения.',
            'description.string' => 'Описание должно быть строкой.',
            'filelist.array' => 'Список файлов должен быть массивом.',
            'filelist.*.file' => 'Каждый элемент должен быть файлом.',
            'filelist.*.mimes' => 'Допустимые форматы файлов: jpg, jpeg, png, pdf.',
            'filelist.*.max' => 'Размер файла не должен превышать 4МБ.',
        ]);

        $ticket = $this->ticketService->createTicket([
            'title' => $request->input('title'),
            'description' => $request->input('description'),
            'user_id' => Auth::user()->id,
        ]);

        if($request->file('filelist')) {
            try {
                foreach ($request->file('filelist') as $index => $file) {
                    $this->fileService->storeFile('Ticket', $ticket->id, 'file'.$index, $file, 'tws3_public');
                }
            }
            catch (\Exception $e) {
                $this->createValidationErrorResponse('Непредвиденная ошибка. Попробуйте позже');
            }
        }

        $this->notificationService->createNotification([
            'user_id' => Auth::user()->id,
            'title' => 'Новый тикет создан',
            'message' => 'Ваш тикет "' . $ticket->title . '" был успешно создан.',
            'url' => '/support/ticket/' . $ticket->id,
        ]);

        $this->notificationService->sendNotificationByRole([
            'title' => 'Новый тикет в системе',
            'message' => 'Пользователь ' . Auth::user()->name . ' создал новый тикет: "' . $ticket->title . '".',
            'url' => '/support/ticket/' . $ticket->id,
        ], 'manager');

        return $ticket;
    }

    public function sendTicketMessage(Request $request, $ticketId) {
        $request->validate([
            'message' => 'required|string',
            'filelist' => 'nullable|array',
            'filelist.*' => 'file|mimes:jpg,jpeg,png,pdf,doc,docx|max:4096',
        ], [
            'message.required' => 'Сообщение обязательно для заполнения.',
            'message.string' => 'Сообщение должно быть строкой.',
            'filelist.array' => 'Список файлов должен быть массивом.',
            'filelist.*.file' => 'Каждый элемент должен быть файлом.',
            'filelist.*.mimes' => 'Допустимые форматы файлов: jpg, jpeg, png, pdf.',
            'filelist.*.max' => 'Размер файла не должен превышать 4МБ.',
        ]);

        $ticket = $this->ticketService->getTicketById($ticketId);

        if (!$ticket) {
            return response()->json(['error' => 'Тикет не найден.'], 404);
        }

        $ticketMessage = $this->ticketService->createTicketMessage([
            'message' => $request->input('message'),
            'user_id' => Auth::user()->id,
        ], $ticketId);

        if ($request->file('filelist')) {
            try {
                foreach ($request->file('filelist') as $index => $file) {
                    $this->fileService->storeFile('TicketMessage', $ticketMessage->id, 'file'.$index, $file, 'tws3_public');
                }
            }
            catch (\Exception $e) {
                $this->createValidationErrorResponse('Непредвиденная ошибка. Попробуйте позже');
            }
        }

        if ($ticket->user_id !== Auth::user()->id) {
            $this->notificationService->createNotification([
                'user_id' => $ticket->user_id,
                'title' => 'Новое сообщение в тикете',
                'message' => 'Вы получили новое сообщение в тикете "' . $ticket->title . '".',
                'url' => '/support/ticket/' . $ticket->id,
            ]);
        }

        broadcast(new SendTicketMessage($ticketId));

        return response()->json(['message' => 'Сообщение отправлено успешно.'], 200);
    }

    public function closeTicket(Request $request, $ticketId) {
        $validator = Validator::make(
            ['ticket_id' => $ticketId],
            ['ticket_id' => 'required|exists:tickets,id'],
            [
                'ticket_id.required' => 'ID тикета обязателен.',
                'ticket_id.exists' => 'Тикет не найден.',
            ]
        );

        if ($validator->fails()) {
            return response()->json(['error' => $validator->errors()->first('ticket_id')], 404);
        }

        $this->ticketService->changeTicketStatus($ticketId, 'closed');

        return response()->json(['message' => 'Тикет успешно закрыт.'], 200);
    }
}
