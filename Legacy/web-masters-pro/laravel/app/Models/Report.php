<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class Report extends Model
{
    protected $fillable = ['user_id', 'worker_id', 'order_id', 'work_id', 'work_shift_id', 'comment'];

    protected static function boot()
    {
        parent::boot();

        static::deleting(function ($course)
        {
            $course->files()->get()->each(function($row) {
                $row->delete();
            });
        });
    }

    public function files() {
        return $this->morphMany(File::class, 'model');
    }

    public function getFile($name) {
        $file = $this->files()->where('name', $name)->first();

        if (empty($file))
            return null;

        return $file->permanent_url;
    }

    public function worker()
    {
        return $this->belongsTo(Worker::class);
    }

    public function user()
    {
        return $this->belongsTo(User::class);
    }

    public function work()
    {
        return $this->belongsTo(Work::class);
    }

    public function workShift()
    {
        return $this->belongsTo(WorkShift::class);
    }
}
