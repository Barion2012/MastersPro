<?php

namespace App\Http\Controllers;

use App\Models\File;
use Illuminate\Http\Request;
use App\Services\UserService;
use App\Services\FileService;
use App\Services\TicketService;
use Illuminate\Support\Facades\Auth;
use App\Models\User;
use App\Models\Worker;
use App\Models\WorkerProfession;
use App\Services\NotificationService;
use Illuminate\Support\Facades\Hash;

class UserController extends Controller
{
    protected $userService;
    protected $fileService;
    protected $notificationService;
    protected $ticketService;

    public function __construct(UserService $userService, FileService $fileService, NotificationService $notificationService, TicketService $ticketService) {
        $this->userService = $userService;
        $this->fileService = $fileService;
        $this->notificationService = $notificationService;
        $this->ticketService = $ticketService;
    }

    public function getUser(Request $request) {
        return $this->userService->getCurrentUser(true);
    }

    public function getUserGroupList(Request $request, $groupName) {
        $users = User::select('id', 'name', 'phone', 'role', 'email')->where('role', $groupName)->paginate(10);

        $currentPage = $users->currentPage();
        $perPage = $users->perPage();

        $users->getCollection()->transform(function ($user, $key) use ($currentPage, $perPage) {
            $user->index = ($currentPage - 1) * $perPage + $key + 1;
            return $user;
        });

        return $users;
    }

    public function saveUser(Request $request) {
        $request->validate([
            'phone' => ['required', 'regex:/^(\+7|8)[\s\-()]*\d{3}[\s\-()]*\d{3}[\s\-()]*\d{2}[\s\-()]*\d{2}$/'],
            'name' => ['required', 'min:3'],
            'email' => ['required', 'email'],
            'id' => ['exists:users,id'],
            'role' => ['required', 'string', 'in:admin,manager,moderator'],
        ], [
            'phone.required' => 'Введите номер телефона',
            'phone.regex' => 'Введите коректный номер телефона',
            'name.required' => 'ФИО обязателен для заполнения',
            'name.min' => 'ФИО должно содержать минимум 3 символа',
            'email.required' => 'Почта обязательна для заполнения',
            'email.email' => 'Введите корректный адрес электронной почты',
            'name.min' => 'ФИО должно содержать минимум 3 символа',
            'id.exists' => 'Пользователь не найден',
            'role.required' => 'Роль обязательна для заполнения',
            'role.string' => 'Роль должна быть строкой',
            'role.in' => 'Роль должна быть одной из: Администратор, Менеджер, Модератор',
        ]);

        if (empty($request->id)) {
            $this->userService->createUser([
                'name' => $request->name,
                'phone' => $request->phone,
                'email' => $request->email,
                'role' => $request->role,
                'password' => Hash::make(uniqid()),
            ]);
        } else {
            $this->userService->updateUser([
                'id' => $request->id,
                'name' => $request->name,
                'phone' => $request->phone,
                'email' => $request->email,
                'role' => empty($request->role) ? NULL : $request->role,
            ]);
        }
    }

    public function deleteUser(Request $request, $userId) {
        $user = User::find($userId);

        if (empty($user)) {
            return response()->json(['message' => 'Пользователь не найден'], 422);
        }

        if ($user->id == Auth::user()->id) {
            return response()->json(['message' => 'Вы не можете удалить себя'], 422);
        }

        $user->delete();
    }

    public function loadFilteredUserList(Request $request) {
        $request->validate([
            'filter' => ['array'],
            'filter.roles' => ['array'],
            'filter.roles.*' => ['string'],
        ], [
            'filter.roles.array' => 'Роли должны быть массивом',
            'filter.roles.*.string' => 'Каждая роль должна быть строкой',
        ]);

        $users = User::select('id', 'name', 'phone', 'role', 'email')->byRole($request->filter['roles'] )->paginate(10);
        $currentPage = $users->currentPage();
        $perPage = $users->perPage();

        $users->getCollection()->transform(function ($user, $key) use ($currentPage, $perPage) {
            $user->index = ($currentPage - 1) * $perPage + $key + 1;
            return $user;
        });

        return $users;
    }

    public function loadWorkersList(Request $request) {
        $users = User::select('id', 'name', 'phone', 'role', 'email')->byRole('worker')->paginate(10);
        $currentPage = $users->currentPage();
        $perPage = $users->perPage();

        $users->getCollection()->transform(function ($user, $key) use ($currentPage, $perPage) {
            $user->index = ($currentPage - 1) * $perPage + $key + 1;
            $user->worker_id = $user->worker->id;
            $user->citizenship = $user->worker->citizenship;
            $user->profession = $user->worker->workerProfession;
            $user->files = [
                'passport_scan' => $user->worker->getFileId('passport_scan'),
                'passport_reg_scan' => $user->worker->getFileId('passport_reg_scan'),
                'snils' => $user->worker->getFileId('snils'),
                'migration_card' => $user->worker->getFileId('migration_card'),
                'patent' => $user->worker->getFileId('patent'),
                'patent_cheque' => $user->worker->getFileId('patent_cheque'),
                'dms' => $user->worker->getFileId('dms'),
            ];
            
            return $user;
        });

        return $users;
    }

    public function loadCustomersList(Request $request) {
        $users = User::select('id', 'name', 'phone', 'role', 'email')->byRole('customer')->paginate(10);
        $currentPage = $users->currentPage();
        $perPage = $users->perPage();

        $users->getCollection()->transform(function ($user, $key) use ($currentPage, $perPage) {
            $user->index = ($currentPage - 1) * $perPage + $key + 1;
            $user->files = [
                'passport_scan' => $user->customer->getFileId('passport_scan'),
                'certificate' => $user->customer->getFileId('certificate'),
            ];
            return $user;
        });

        return $users;
    }

    public function editWorkerProfessions(Request $request, $workerId) {
        $request->validate([
            'professions' => ['required', 'array'],
            'professions.*.profession_id' => ['required', 'integer', 'exists:professions,id'],
            'professions.*.profession_level_id' => ['required', 'integer', 'exists:profession_levels,id'],
        ], [
            'professions.required' => 'Список профессий обязателен',
            'professions.array' => 'Профессии должны быть массивом',
            'professions.*.profession_id.required' => 'ID профессии обязателен',
            'professions.*.profession_id.integer' => 'ID профессии должен быть числом',
            'professions.*.profession_id.exists' => 'Профессия не найдена',
            'professions.*.profession_level_id.required' => 'ID уровня профессии обязателен',
            'professions.*.profession_level_id.integer' => 'ID уровня профессии должен быть числом',
            'professions.*.profession_level_id.exists' => 'Уровень профессии не найден',
        ]);

        foreach($request->professions as $profession) {
            $workerProfession = WorkerProfession::where([['worker_id', $workerId], ['profession_id', $profession['profession_id']]])->first();

            if ($workerProfession) {
                $workerProfession->profession_level_id = $profession['profession_level_id'];
                $workerProfession->save();
            }
        }

        return response(['message' => 'Работник обновлен'], 200);
    }

    public function editWorkerPaymentRequisites(Request $request, $workerId) {
        $request->validate([
            'account_number' => ['required', 'string', 'digits_between:20,25'],
            'bank_bik' => ['required', 'string', 'digits:9'],
            'bank_cor_account' => ['required', 'string', 'digits:20'],
            'bank_name' => ['required', 'string', 'min:3', 'max:50'],
        ], [
            'account_number.required' => 'Расчётный счёт обязателен для заполнения.',
            'account_number.digits_between' => 'Расчётный счёт должен содержать от 20 до 25 цифр.',
            'bank_bik.required' => 'БИК обязателен для заполнения.',
            'bank_bik.digits' => 'БИК должен содержать ровно 9 цифр.',
            'bank_cor_account.required' => 'Корреспондентский счёт обязателен для заполнения.',
            'bank_cor_account.digits' => 'Корреспондентский счёт должен содержать ровно 20 цифр.',
            'bank_name.required' => 'Наименование банка обязательно для заполнения.',
            'bank_name.min' => 'Наименование банка должно содержать минимум 3 символа.',
            'bank_name.max' => 'Наименование банка не должно превышать 50 символов.',
        ]);

        $worker = Worker::find($workerId);

        if (empty($worker)) {
            return response(['message' => 'Рабочий не найден'], 404);
        }

        $worker->account_number = $request->account_number;
        $worker->bank_bic = $request->bank_bik;
        $worker->bank_cor_account = $request->bank_cor_account;
        $worker->bank_name = $request->bank_name;

        $worker->save();

        return response(['message' => 'Работник обновлен'], 200);
    }

    public function editUserPaymentRequisites(Request $request) {
        $request->validate([
            'account_number' => ['required', 'string', 'digits_between:20,25'],
            'bank_bik' => ['required', 'string', 'digits:9'],
            'bank_cor_account' => ['required', 'string', 'digits:20'],
            'bank_name' => ['required', 'string', 'min:3', 'max:50'],
        ], [
            'account_number.required' => 'Расчётный счёт обязателен для заполнения.',
            'account_number.digits_between' => 'Расчётный счёт должен содержать от 20 до 25 цифр.',
            'bank_bik.required' => 'БИК обязателен для заполнения.',
            'bank_bik.digits' => 'БИК должен содержать ровно 9 цифр.',
            'bank_cor_account.required' => 'Корреспондентский счёт обязателен для заполнения.',
            'bank_cor_account.digits' => 'Корреспондентский счёт должен содержать ровно 20 цифр.',
            'bank_name.required' => 'Наименование банка обязательно для заполнения.',
            'bank_name.min' => 'Наименование банка должно содержать минимум 3 символа.',
            'bank_name.max' => 'Наименование банка не должно превышать 50 символов.',
        ]);

        $worker = Worker::where('user_id', Auth::user()->id)->first();

        if (empty($worker)) {
            return response(['message' => 'Профиль не найден'], 404);
        }

        $worker->account_number = $request->account_number;
        $worker->bank_bic = $request->bank_bik;
        $worker->bank_cor_account = $request->bank_cor_account;
        $worker->bank_name = $request->bank_name;

        $worker->save();

        return response(['message' => 'Профиль обновлен'], 200);
    }

    public function editWorkerPersonalData(Request $request, $workerId) {
        $request->validate([
            'inn' => ['required', 'string', 'digits:12'],
            'snils' => ['required', 'string', 'regex:/^\d{3}-\d{3}-\d{3}\s\d{2}$/'],
            'location' => ['required']
        ], [
            'inn.required' => 'ИНН обязателен для заполнения',
            'inn.digits' => 'ИНН должен содержать 12 цифр',
            'snils.required' => 'Поле СНИЛС обязательно для заполнения.',
            'snils.string' => 'Поле СНИЛС должно быть строкой.',
            'snils.regex' => 'СНИЛС должен соответствовать формату 123-456-789 01.',
            'location.required' => 'Адрес обязателен для заполнения',
        ]);

        $worker = Worker::find($workerId);

        if (empty($worker)) {
            return response(['message' => 'Рабочий не найден'], 404);
        }

        $worker->inn = $request->inn;
        $worker->snils = $request->snils;
        $worker->location = $request->location;

        $worker->save();

        return response(['message' => 'Работник обновлен'], 200);
    }

    public function getUserNotifications(Request $request) {
        $notifications = $this->notificationService->getNotificationsByUserId(Auth::user()->id);
        return $notifications;
    }

    public function deleteUserNotifications(Request $request) {
        $this->notificationService->deleteAllNotificationsByUserId(Auth::user()->id);
    }

    public function getUserTickets(Request $request) {
        return $this->ticketService->getSupportTicketsListByUserId(Auth::user()->id);
    }

    public function getUserFile(Request $request, $userId, $fileId) {
        $file = File::find($fileId);
        $user = User::find($userId);

        if (!$file || !$user)
            return response(['message' => 'Файл не найден'], 404);

        switch ($file->model_type) {
            case 'App\Models\Worker':
                if (!$user || $user->role != 'worker' || $user->worker->id != $file->model_id || $file->model_type != 'App\Models\Worker') {
                    return response(['message' => 'Файл не принадлежит работнику'], 403);
                }
                break;
            case 'App\Models\Customer':
                if (!$user || $user->role != 'customer' || $user->customer->id != $file->model_id || $file->model_type != 'App\Models\Customer') {
                    return response(['message' => 'Файл не принадлежит клиенту'], 403);
                }
                break;
            default:
                return response(['message' => 'Неверный тип файла'], 400);
        }

        return redirect()->away($file->temporary_url);
    }

    public function editCustomer(Request $request, $userId) {
        $request->validate([
            'balance' => ['required', 'numeric', 'min:0'],
        ]);

        $user = User::find($userId);

        if (!$user || $user->role != 'customer') {
            return response(['message' => 'Клиент не найден'], 404);
        }

        $user->customer->balance = $request->input('balance');
        $user->customer->save();

        return response(['message' => 'Клиент обновлен'], 200);
    }
}
