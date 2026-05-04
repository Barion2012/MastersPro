<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class Ticket extends Model
{
    protected $fillable = [
        'title',
        'description',
        'user_id',
        'report_id'
    ];

    public function user()
    {
        return $this->belongsTo(User::class);
    }

    public function report()
    {
        return $this->belongsTo(Report::class);
    }

    public function messages()
    {
        return $this->hasMany(TicketMessage::class);
    }

    public function files() {
        return $this->morphMany(File::class, 'model');
    }
}
