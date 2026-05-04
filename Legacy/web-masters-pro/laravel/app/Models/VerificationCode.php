<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class VerificationCode extends Model
{
    public $timestamps = false;
    public $fillable = ['code', 'phone', 'code_sent_at'];
}
