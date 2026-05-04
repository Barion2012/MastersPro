<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;

class Customer extends Model
{
    protected $casts = [
        'info' => 'array', 
    ];

    public function user()
    {
        return $this->belongsTo(User::class);
    }

    protected static function boot()
    {
        parent::boot();

        static::deleting(function ($record)
        {
            $record->files()->get()->each(function($row) {
                $row->delete();
            });
        });
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

    public function getFileId($name) {
        $file = $this->files()->where('name', $name)->first();

        if (empty($file))
            return null;

        return $file->id;
    }

    public function getFileTemporary($name) {
        $file = $this->files()->where('name', $name)->first();

        if (empty($file))
            return null;

        return $file->temporary_url;
    }
}
