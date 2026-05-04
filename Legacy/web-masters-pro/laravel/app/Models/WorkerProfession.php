<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class WorkerProfession extends Model
{
    protected $fillable = ['worker_id', 'profession_id', 'profession_level_id'];

    public function worker()
    {
        return $this->belongsTo(Worker::class);
    }

    public function profession()
    {
        return $this->belongsTo(Profession::class);
    }

    public function professionLevel()
    {
        return $this->belongsTo(ProfessionLevel::class);
    }
}
