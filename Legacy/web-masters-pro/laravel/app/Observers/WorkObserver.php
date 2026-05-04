<?php

namespace App\Observers;

use App\Events\WorkStatusUpdated;
use App\Models\Work;
use Illuminate\Support\Facades\Log;

class WorkObserver
{
    public function created(Work $work)
    {
        //event(new WorkStatusUpdated($work));
    }

    public function updated(Work $work)
    {
        if ($work->isDirty(['count', 'found'])) {
            event(new WorkStatusUpdated($work));
        }
    }
}
