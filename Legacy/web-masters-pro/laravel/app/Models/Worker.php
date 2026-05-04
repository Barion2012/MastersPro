<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Support\Carbon;
use Illuminate\Support\Facades\Storage;

class Worker extends Model
{
    protected static function boot()
    {
        parent::boot();

        static::deleting(function ($record)
        {
            $record->files()->get()->each(function($row) {
                $row->delete();
            });
        });
    }

    public function user() {
        return $this->belongsTo(User::class);
    }

    public function workerProfession() {
        return $this->hasMany(WorkerProfession::class)->select(['profession_id', 'profession_level_id']);
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

    public function getFileId($name) {
        $file = $this->files()->where('name', $name)->first();

        if (empty($file))
            return null;

        return $file->id;
    }

    public function getFileTemporary($name) {
        $file = $this->files()->where('name', $name)->first();

        if (empty($file))
            return null;

        return $file->temporary_url;
    }

    public function getCurrentWorkShiftAttribute() {
        $acceptedWorks = AcceptWork::where('user_id', $this->user_id)
            ->where('status', 'completed')
            ->pluck('order_id')
            ->unique()
            ->values();
        $currentWorkShift = WorkShift::where('user_id', $this->user_id)
            ->whereNotIn('order_id', $acceptedWorks)
            ->get()
            ->filter(function($shift) {
                if (empty($shift->date)) return false;
                try {
                    $start = Carbon::createFromFormat('d.m.Y H:i', $shift->date);
                } catch (\Exception $e) {
                    return false;
                }
                $hoursDiff = Carbon::now()->diffInHours($start, false);
                return $hoursDiff >= -9 && $hoursDiff <= 20;
            })
            ->sortBy('index')
            ->first();

        $isActive = true;

        if (empty($currentWorkShift)) {
            $currentWorkShift = WorkShift::where('user_id', $this->user_id)
                ->where('date', '>', Carbon::now()->format('d.m.Y H:i'))
                ->orderBy('index', 'asc')
                ->first();

            $isActive = false;

            if (empty($currentWorkShift))
                return null;
        }

        return [
            'id' => $currentWorkShift->id,
            'user_id' => $currentWorkShift->user_id,
            'order_id' => $currentWorkShift->order_id,
            'work_id' => $currentWorkShift->work_id,
            'index' => $currentWorkShift->index,
            'date' => $currentWorkShift->date,
            'report' => $currentWorkShift->report,
            'time_start' => $currentWorkShift->time_start,
            'time_end' => $currentWorkShift->time_end,
            'confirm_meeting' => $currentWorkShift->confirm_meeting,
            'confirm_address' => $currentWorkShift->confirm_address,
            'not_met' => $currentWorkShift->not_met,
            'price' => $currentWorkShift->price,
            'enable' => Carbon::parse($currentWorkShift->date)->addMinutes(20)->isPast(),
            'is_active' => $isActive,
            'three_hours_passed' => Carbon::parse($currentWorkShift->date)->addHours(3)->isPast(),
            'hours_diff' => Carbon::now()->diffInHours(Carbon::createFromFormat('d.m.Y H:i', $currentWorkShift->date), false),
        ];
    }
}
