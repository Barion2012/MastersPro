<?php

namespace App\Models;

use Illuminate\Database\Eloquent\Model;
use Illuminate\Support\Facades\Storage;

class File extends Model
{
    protected static function boot()
    {
        parent::boot();

        static::deleting(function ($file)
        {
            Storage::disk($file->storage)->delete(str_replace('/storage/', '', $file->path));
        });
    }

    public function model() {
        return $this->morphTo();
    }
    
    public function getTemporaryUrlAttribute() {
        $url = Storage::disk($this->storage)->temporaryUrl(
            $this->path,
            now()->addMinutes(5)
        );

        return $url;
    }

    public function getPermanentUrlAttribute() {
        return Storage::disk($this->storage)->url('/'.config('filesystems.disks.tws3_public.bucket').$this->path);
    }
}
