<?php

namespace App\Http\Controllers;
use Illuminate\Validation\ValidationException;
use Illuminate\Support\Facades\Validator;
abstract class Controller
{
    public function createValidationErrorResponse($message) {
        $validator = Validator::make([], []);
        $validator->errors()->add('message', $message);
        throw new ValidationException($validator);
    }

    public function replaceQuotes($text) {
        $search = ['«', '»', '„', '“'];
        $replace = ['"', '"', '"', '"'];
        return str_replace($search, $replace, $text);
    }
}
