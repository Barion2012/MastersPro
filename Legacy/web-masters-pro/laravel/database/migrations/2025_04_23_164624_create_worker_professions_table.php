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
        Schema::create('worker_professions', function (Blueprint $table) {
            $table->id();
            $table->timestamps();
            $table->foreignId('worker_id')->constrained()->onDelete('cascade');
            $table->foreignId('profession_id')->constrained()->onDelete('cascade');
            $table->foreignId('profession_level_id')->constrained()->onDelete('cascade');
        });
    }

    /**
     * Reverse the migrations.
     */
    public function down(): void
    {
        Schema::dropIfExists('worker_professions');
    }
};
