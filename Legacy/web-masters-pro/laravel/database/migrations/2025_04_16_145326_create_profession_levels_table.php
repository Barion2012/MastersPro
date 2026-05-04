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
        Schema::create('profession_levels', function (Blueprint $table) {
            $table->id();
            $table->timestamps();
            $table->foreignId('profession_id')->constrained()->onDelete('cascade');
            $table->unsignedInteger('level');
            $table->string('desc');
            $table->decimal('price', 10, 2);
            $table->string('currency')->default('RUB');
        });
    }

    /**
     * Reverse the migrations.
     */
    public function down(): void
    {
        Schema::dropIfExists('profession_levels');
    }
};
