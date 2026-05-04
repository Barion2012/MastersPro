<?php 

namespace App\Services;

use App\Models\User;
use Illuminate\Support\Facades\Hash;
use Illuminate\Support\Facades\Auth;
use Illuminate\Support\Facades\Validator;
use Illuminate\Validation\ValidationException;

class UserService {
    public function getCurrentUser(bool $serialize = false) {
        return $this->getUserById(Auth::user()->id, $serialize);
    }

    public function getUserById(int $userId, bool $serialize = false)  {
        $user = User::find($userId);

        if ($serialize)
            return $this->serializeUser(user: $user);
        else 
            return $user;
    }

    public function getUserByPhone(string $phone, bool $serialize = false) {
        $cleaned = $this->serializePhone($phone);

        $user = User::where('phone', $cleaned)->first();

        if ($serialize)
            return $this->serializeUser(user: $user);
        else 
            return $user;
    }

    public function registerUser(array $data) {
        $this->createUser([
            'name' => 'Пользователь#'.uniqid(),
            'email' => $data['email'],
            'password' => Hash::make($data['password']),
            'role' => 'user'
        ]);
    }

    public function createUser(array $data) {
        $cleanedPhone = $this->serializePhone($data['phone']);

        $validator = Validator::make([
            'email' => $data['email'],
            'phone' => $cleanedPhone,
        ], [
            'email' => ['required', 'email', 'unique:users,email'],
            'phone' => ['required', 'unique:users,phone']
        ], [
            'email.required' => 'Почта обязательна для заполнения',
            'email.email' => 'Введите корректный адрес электронной почты',
            'email.unique' => 'Пользователь с такой почтой уже существует',
            'phone.required' => 'Введите номер телефона',
            'phone.unique' => 'Пользователь с таким номером телефона уже существует',
        ]);

        if ($validator->fails()) {
            throw ValidationException::withMessages($validator->errors()->toArray());
        }

        return User::create($data);
    }

    public function serializeUser($user) {
        if (empty($user)) {
            return NULL;
        }

        $result = [
            'id' => $user->id,
            'name' => $user->name,
            'email' => $user->email,
            'permissions' => $user->permissions,
            'role' => $user->role,
            'phone' => $user->phone,
        ];

        switch ($user->role) {
            case 'customer':
                $result['customer'] = [
                    'balance' => $user->customer->balance,
                    'hold_balance' => $user->customer->hold_balance,
                    'balance_currency' => $user->customer->balance_currency,          
                    'type' => $user->customer->type,
                    'bank_name' => $user->customer->info['bankName'],
                    'account_number' => $user->customer->info['accountNumber'],
                    'bank_cor_account' => $user->customer->info['bankCorAccount'],
                    'bank_bic' => $user->customer->info['bankBIC'],
                ];
                break;
            case 'worker':
                $result['worker'] = [
                    'id' => $user->worker->id,
                    'profession' => $user->worker->workerProfession,
                    'citizenship' => $user->worker->citizenship,
                    'bank_name' => $user->worker->bank_name,
                    'account_number' => $user->worker->account_number,
                    'bank_cor_account' => $user->worker->bank_cor_account,
                    'bank_bic' => $user->worker->bank_bic,
                ];
                break;
        }

        return $result;
    }

    public function serializePhone($phone) {
        $cleanedPhone = preg_replace('/[^\d+]/', '', $phone);

        if (strpos($cleanedPhone, '8') === 0 && strlen($cleanedPhone) == 11) {
            $cleanedPhone = '+7' . substr($cleanedPhone, 1);
        }

        return $cleanedPhone;
    }

    public function updateUser($data) {
        $user = User::find($data['id']);

        if (empty($user)) {
            throw ValidationException::withMessages(['Пользователь не найден']);
        }

        if ($user->email != $data['email']) {
            $validator = Validator::make([
                'email' => $data['email'],
            ], [
                'email' => ['required', 'email', 'unique:users,email'],
            ], [
                'email.required' => 'Почта обязательна для заполнения',
                'email.email' => 'Введите корректный адрес электронной почты',
                'email.unique' => 'Пользователь с такой почтой уже существует',
            ]);

            if ($validator->fails()) {
                throw ValidationException::withMessages($validator->errors()->toArray());
            }
        }

        if (!empty($data['phone'])) {
            $cleanedPhone = preg_replace('/[^\d+]/', '', $data['phone']);

            if (strpos($cleanedPhone, '8') === 0 && strlen($cleanedPhone) == 11) {
                $cleanedPhone = '+7' . substr($cleanedPhone, 1);
            }

            $data['phone'] = $cleanedPhone;
        }
        
        if ($user->phone != $data['phone']) {
            $validator = Validator::make([
                'phone' => $data['phone'],
            ], [
                'phone' => ['required', 'unique:users,phone'],
            ], [
                'phone.required' => 'Введите номер телефона',
                'phone.unique' => 'Пользователь с таким номером телефона уже существует',
            ]);

            if ($validator->fails()) {
                throw ValidationException::withMessages($validator->errors()->toArray());
            }
        }

        $user->name = $data['name'];
        $user->email = $data['email'];
        $user->phone = $data['phone'];

        if (!empty($data['role'])) {
            $user->role = $data['role'];
        }

        $user->save();
        return $user;
    }
}