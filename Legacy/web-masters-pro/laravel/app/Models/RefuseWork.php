<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class RefuseWork extends Model
{
    public $fillable = [
        'user_id',
        'worker_id',
        'order_id',
        'work_id',
    ];
}
