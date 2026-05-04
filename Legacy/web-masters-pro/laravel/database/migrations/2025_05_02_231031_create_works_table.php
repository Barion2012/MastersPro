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
        Schema::create('works', function (Blueprint $table) {
            $table->id();
            $table->timestamps();
            $table->foreignId('order_id')->constrained()->onDelete('cascade');
            $table->foreignId('profession_id')->constrained();
            $table->foreignId('profession_level_id')->constrained();
            $table->integer('count');
            $table->integer('found')->default(0);
            $table->integer('refused')->default(0);
            $table->timestamp('start_date');
            $table->timestamp('end_date');
            $table->string('status')->default('search');
            $table->decimal('price_shift', 10, 2);
            $table->string('currency')->default('RUB');
        });
    }

    /**
     * Reverse the migrations.
     */
    public function down(): void
    {
        Schema::dropIfExists('works');
    }
};
