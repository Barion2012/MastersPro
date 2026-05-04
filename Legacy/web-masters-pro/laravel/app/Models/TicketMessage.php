<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class TicketMessage extends Model
{
    public $fillable = ['user_id', 'message'];

    public function user()
    {
        return $this->belongsTo(User::class);
    }

    public function files() {
        return $this->morphMany(File::class, 'model');
    }
}
