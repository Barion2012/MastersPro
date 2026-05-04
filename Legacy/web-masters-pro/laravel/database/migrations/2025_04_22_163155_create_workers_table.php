<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    /**
     * Run the migrations.
     */
    public function up(): void
    {
        Schema::create('workers', function (Blueprint $table) {
            $table->id();
            $table->timestamps();
            $table->foreignId('user_id')->constrained()->onDelete('cascade');
            $table->string('status')->default('check');
            $table->string('citizenship');
            $table->string('inn');
            $table->string('snils');
            $table->string('address');
            $table->string('passport_series');
            $table->string('passport_number');
            $table->string('passport_issued_by');
            $table->string('location');
            $table->string('location_lat');
            $table->string('location_lng');
            $table->string('account_number');
            $table->string('bank_bic');
            $table->string('bank_cor_account');
            $table->string('bank_name');
            //$table->decimal('balance', 10, 2)->default(0); 
            //$table->string('currency')->default('RUB');
        });
    }

    /**
     * Reverse the migrations.
     */
    public function down(): void
    {
        Schema::dropIfExists('workers');
    }
};
