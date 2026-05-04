<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class Profession extends Model
{
    public function levels()
    {
        return $this->hasMany(ProfessionLevel::class);
    }

    public function getLevelListAttribute()
    {
        return $this->levels()->orderBy('id', 'ASC')->get()->map(function ($level) {
            return [
                'id' => $level->id,
                'desc' => $level->desc,
                'price' => $level->price,
                'level' => $level->level,
                'profession_id' => $level->profession_id,
            ];
        });
    }
}
