<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class Order extends Model
{
    public $fillable = ['user_id', 'customer_id', 'address_lat', 'address_lng', 'address', 'meeting_point', 'meeting_point_lat', 'meeting_point_lng', 'info', 'total_price', 'place', 'quality'];

    public function user() {
        return $this->belongsTo(User::class);
    }

    public function customer() {
        return $this->belongsTo(Customer::class);
    }

    public function works() {
        return $this->hasMany(Work::class);
    }

    public function reports() {
        return $this->hasMany(Report::class);
    }

    public function acceptWorks() {
        return $this->hasMany(AcceptWork::class);
    }

    public function files() {
        return $this->morphMany(File::class, 'model');
    }

    public function getFile($name) {
        $file = $this->files()->where('name', $name)->first();

        if (empty($file))
            return null;

        return $file->permanent_url;
    }
}
