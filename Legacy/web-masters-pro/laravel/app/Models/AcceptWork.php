<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class AcceptWork extends Model
{
    public $fillable = ['user_id', 'worker_id', 'order_id', 'work_id', 'profession_id', 'profession_level_id', 'status'];

    protected static function boot()
    {
        parent::boot();

        static::deleting(function ($acceptWork)
        {
            WorkShift::where([['work_id', $acceptWork->work_id], ['user_id', $acceptWork->user_id]])->delete();
        });
    }
    
    public function user() {
        return $this->belongsTo(User::class);
    }

    public function worker() {
        return $this->belongsTo(Worker::class);
    }

    public function order() {
        return $this->belongsTo(Order::class);
    }

    public function work() {
        return $this->belongsTo(Work::class);
    }

    public function profession() {
        return $this->belongsTo(Profession::class);
    }

    public function professionLevel() {
        return $this->belongsTo(ProfessionLevel::class);
    }
}
