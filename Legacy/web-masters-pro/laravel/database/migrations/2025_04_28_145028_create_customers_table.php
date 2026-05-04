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
        Schema::create('customers', function (Blueprint $table) {
            $table->id();
            $table->timestamps();
            $table->foreignId('user_id')->constrained()->onDelete('cascade');
            $table->string('beneficiary_id');
            $table->string('type')->default('ip');
            $table->json('info')->nullable();
            $table->decimal('balance', 15, 2)->default(0);
            $table->decimal('hold_balance', 10, 2)->default(0);
            $table->string('balance_currency')->default('RUB');
        });
    }

    /**
     * Reverse the migrations.
     */
    public function down(): void
    {
        Schema::dropIfExists('customers');
    }
};
