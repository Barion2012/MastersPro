<?php

use Illuminate\Foundation\Console\ClosureCommand;
use Illuminate\Foundation\Inspiring;
use Illuminate\Support\Facades\Artisan;
use Illuminate\Support\Facades\Schedule;

Artisan::command('inspire', function () {
    /** @var ClosureCommand $this */
    $this->comment(Inspiring::quote());
})->purpose('Display an inspiring quote');

Schedule::command('orders:notify')
    ->everyMinute();

Schedule::command('payment:check')
    ->everyMinute();

Schedule::command('work-shift:check')
    ->everyMinute();

Schedule::command('work:match-workers')
    ->everyMinute();

Schedule::command('work:check-match')
    ->everyMinute();
Schedule::command('work:check-shifts')
    ->everyMinute();