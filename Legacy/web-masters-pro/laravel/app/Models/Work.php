<?php

namespace App\Models;

use Carbon\Carbon;
use Illuminate\Database\Eloquent\Model;

class Work extends Model
{
    public $fillable = ['order_id', 'profession_id', 'profession_level_id', 'count', 'found', 'status', 'start_date', 'end_date', 'price_shift'];

    public function order() {
        return $this->belongsTo(Order::class);
    }

    public function profession() {
        return $this->belongsTo(Profession::class);
    }

    public function professionLevel() {
        return $this->belongsTo(ProfessionLevel::class);
    }

    public function workShifts() {
        return $this->hasMany(WorkShift::class);
    }

    public function getCurrentWorkShiftAttribute() {
        $currentWorkShift = $this->workShifts()
            ->get()
            ->filter(function($shift) {
            if (empty($shift->date)) return false;
                try {
                    $start = Carbon::createFromFormat('d.m.Y H:i', $shift->date);
                } catch (\Exception $e) {
                    return false;
                }
                $hoursLeft = Carbon::now()->diffInHours($start, false);
                return $hoursLeft >= 0 && $hoursLeft <= 20;
            })
            ->sortBy('index')
            ->first();

        if (empty($currentWorkShift))
            return null;

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
            'enable' => Carbon::parse($currentWorkShift->date)->addMinutes(20)->isPast()
        ];
    }

    public function serialize() {
        $startDate = Carbon::parse($this->start_date)->startOfDay();
        $endDate = Carbon::parse($this->end_date)->startOfDay();
        $days = $startDate->diffInDays($endDate, false);

        return [
            'id' => $this->id,
            'created_at' => Carbon::parse($this->created_at)->format('d.m.Y'),
            'profession_id' => $this->profession_id,
            'profession' => $this->profession->name,
            'profession_level_id' => $this->profession_level_id,
            'profession_level' => $this->professionLevel->desc,
            'count' => $this->count,
            'found' => $this->found,
            'start_date' => Carbon::parse($this->start_date)->format('d.m.Y H:i'),
            'end_date' => Carbon::parse($this->end_date)->format('d.m.Y H:i'),
            'price' => $this->price_shift * ($days + 1),
            'price_shift' => $this->price_shift,
        ];
    }
}
