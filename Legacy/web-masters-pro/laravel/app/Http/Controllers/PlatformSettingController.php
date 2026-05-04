<?php

namespace App\Http\Controllers;

use App\Models\PlatformSetting;
use Illuminate\Http\Request;

class PlatformSettingController extends Controller
{
    public function saveSetting(Request $request) {
        $setting = PlatformSetting::where('key', $request->key)->first();

        if (empty($setting))
            return $this->createValidationErrorResponse('Параметр не найден');

        $setting->value = $request->value;
        $setting->save();

        return;
    }

    public function getSettingsList() {
        return PlatformSetting::all();
    }
}
