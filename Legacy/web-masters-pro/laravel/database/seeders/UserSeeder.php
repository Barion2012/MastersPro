<?php

namespace Database\Seeders;

use Carbon\Carbon;
use Illuminate\Database\Console\Seeds\WithoutModelEvents;
use Illuminate\Database\Seeder;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Hash;

class UserSeeder extends Seeder
{
    /**
     * Run the database seeds.
     */
    public function run(): void
    {
        DB::table('users')->insert([
            'name' => 'superuser',
            'email' => config('app.superuser_email'),
            'password' => Hash::make(config('app.superuser_password')),
            'role' => 'superuser',
            'phone' => '+79081696981',
            'phone_verified_at' => now()
        ]);

        DB::table('users')->insert([
            'name' => 'admin',
            'email' => 'admin'.'@admin.com',
            'password' => Hash::make('password'),
            'role' => 'admin',
            'phone' => '+79999999999',
            'phone_verified_at' => now()
        ]);
    }
}
