<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class ProfessionLevel extends Model
{
    protected $fillable = [
        'profession_id',
        'level',
        'description',
        'price',
    ];

    public function profession()
    {
        return $this->belongsTo(Profession::class);
    }
}
