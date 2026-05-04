<?php

use Illuminate\Support\Facades\Route;
use App\Http\Middleware\EnsurePermissionIsValid;

Route::get('/', function () {
    return view('welcome');
});
