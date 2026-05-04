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
        Schema::create('orders', function (Blueprint $table) {
            $table->id();
            $table->timestamps();
            $table->foreignId('user_id')->constrained()->onDelete('cascade');
            $table->foreignId('customer_id')->constrained();
            $table->string('address_lat');
            $table->string('address_lng');
            $table->string('address');
            $table->string('meeting_point');
            $table->string('meeting_point_lat');
            $table->string('meeting_point_lng');
            $table->longText('info')->nullable();
            $table->decimal('total_price', 10, 2);
            $table->string('currency')->default('RUB');
            $table->string('status')->default('search');
        });
    }

    /**
     * Reverse the migrations.
     */
    public function down(): void
    {
        Schema::dropIfExists('orders');
    }
};
