<?php

namespace App\Console\Commands;

use App\Events\WorkShiftUpdate;
use App\Models\AcceptWork;
use App\Models\WorkShift;
use Carbon\Carbon;
use Illuminate\Console\Command;

class CheckWorkShift extends Command
{
    /**
     * The name and signature of the console command.
     *
     * @var string
     */
    protected $signature = 'work-shift:check';

    /**
     * The console command description.
     *
     * @var string
     */
    protected $description = 'Check work shit logic';

    /**
     * Execute the console command.
     */
    public function handle()
    {
        $workShifts = WorkShift::where('date', Carbon::today()->format('d.m.Y'))->get();
        $now = Carbon::now();
        

        foreach ($workShifts as $workShift) {
            $timeStart = Carbon::parse($workShift->time_start);

            if ($timeStart->diffInMinutes($now) >= 15 && $workShift->not_met == false) {
                $workShift->not_met = true;
                $workShift->save();
                broadcast(new WorkShiftUpdate($workShift->user_id));
            }
        }
    }
}
