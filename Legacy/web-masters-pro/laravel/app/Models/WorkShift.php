<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class WorkShift extends Model
{
    public $fillable = ['user_id', 'order_id', 'work_id', 'date', 'index', 'price', 'smart_contract_id', 'time_start', 'time_end'];

    public function user() {
        return $this->belongsTo(User::class);
    }

    public function order() {
        return $this->belongsTo(Order::class);
    }

    public function work() {
        return $this->belongsTo(Work::class);
    }

    public function report() {
        return $this->hasOne(Report::class, 'work_shift_id', 'id');
    }
}
