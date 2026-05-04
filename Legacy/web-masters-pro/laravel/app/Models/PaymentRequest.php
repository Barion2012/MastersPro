<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class PaymentRequest extends Model
{
    public $fillable = [
        'customer_id',
        'worker_id',
        'work_shift_id',
        'order_id',
        'smart_contract_id',
        'status',
        'response',
        'amount',
        'currency',
    ];

    protected $casts = [
        'response' => 'array',
        'amount' => 'decimal:2',
    ];

    public function customer()
    {
        return $this->belongsTo(Customer::class);
    }

    public function worker()
    {
        return $this->belongsTo(Worker::class);
    }

    public function workShift()
    {
        return $this->belongsTo(WorkShift::class);
    }

    public function order()
    {
        return $this->belongsTo(Order::class);
    }
} 
