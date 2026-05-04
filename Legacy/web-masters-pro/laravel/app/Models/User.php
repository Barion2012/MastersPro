<?php

namespace App\Models;

// use Illuminate\Contracts\Auth\MustVerifyEmail;
use Illuminate\Database\Eloquent\Factories\HasFactory;
use Illuminate\Foundation\Auth\User as Authenticatable;
use Illuminate\Notifications\Notifiable;
use Laravel\Sanctum\HasApiTokens;

class User extends Authenticatable {
    /** @use HasFactory<\Database\Factories\UserFactory> */
    use HasApiTokens, HasFactory, Notifiable;

    /**
     * The attributes that are mass assignable.
     *
     * @var list<string>
     */
    protected $fillable = [
        'name',
        'email',
        'password',
        'role',
        'phone',
        'sms_code', 
        'sms_code_sent_at'
    ];

    /**
     * The attributes that should be hidden for serialization.
     *
     * @var list<string>
     */
    protected $hidden = [
        'password',
        'remember_token',
    ];

    /**
     * Get the attributes that should be cast.
     *
     * @return array<string, string>
     */
    protected function casts(): array {
        return [
            'email_verified_at' => 'datetime',
            'password' => 'hashed',
        ];
    }

    public function permissions() {
        return $this->hasMany(Permission::class, 'role', 'role');
    }

    public function worker() {
        return $this->hasOne(Worker::class);
    }

    public function customer() {
        return $this->hasOne(Customer::class);
    }

    public function getPermissionsAttribute() {
        return $this->permissions()->get()->groupBy('model')->map(function ($actions, $model) {
            return $actions->pluck('action')->toArray(); 
        });
    }

    public function checkPermission($model, $action) {
        $permission = $this->permissions()->where([['model', $model], ['action', $action]])->first();

        if (empty($permission))
            return false;

        return true;
    }

    public function setPhoneAttribute($value) {
        $cleaned = preg_replace('/[^\d+]/', '', $value);

        if (strpos($cleaned, '8') === 0 && strlen($cleaned) == 11) {
            $cleaned = '+7' . substr($cleaned, 1);
        }

        $this->attributes['phone'] = $cleaned;
    }

    public function getFormattedPhoneAttribute()
    {
        if (preg_match('/^\+7(\d{3})(\d{3})(\d{2})(\d{2})$/', $this->phone, $matches)) {
            return "+7({$matches[1]}){$matches[2]}-{$matches[3]}-{$matches[4]}";
        }
        
        return $this->phone;
    }

    public function scopeByRole($query, $role) {
        if (empty($role))
            return $query;
        if (is_array($role))
            return $query->whereIn('role', $role);
        
        return $query->where('role', $role);
    }
}
