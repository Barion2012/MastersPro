<?php 

namespace App\Services;

use Illuminate\Support\Facades\Http;
use Illuminate\Support\Facades\Log;

/**
 * Сервис отправки SMS и голосовых сообщений через Notisend.
 *
 * Поддерживает:
 *  - SMS-сообщения (текст)
 *  - Голосовые вызовы (озвучивание кода)
 *
 * Используется в:
 *  - AuthController@sendSmsCode()
 *  - RegisterController@sendSmsCode()
 *
 * Настройки берутся из config/notisend.php
 *
 * @see config/notisend.php
 */

class NotisendService {
    /**
     * Генерирует подпись (sign) для запроса к Notisend API.
     *
     * Алгоритм:
     *  1. Сортировка параметров по ключу (ksort)
     *  2. Склейка значений через ';'
     *  3. Добавление секретного ключа в конец
     *  4. sha1 -> md5
     *
     * @param array  $data Массив параметров запроса (без sign)
     * @param string $key  Секретный api ключ
     * @return string Подпись в виде md5-хеша
     */
    private function signRequest($data, $key) {
        ksort($data);
        $values = implode(';', $data);
        $values = $values . ';' . $key;
        $sign = md5(sha1($values));
        return $sign;
    }

    /**
     * Выполняет POST-запрос к Notisend API.
     *
     * Особенности:
     *  - Отправка как form-data (asForm())
     *  - Принимает только JSON-ответ
     *  - Логирует ошибки при неудачном запросе
     *  - Бросает исключение при HTTP-ошибке
     *
     * @param string $url    Часть пути (например, /message/send)
     * @param array  $data   Параметры запроса
     * @return \Illuminate\Http\Client\Response
     * @throws \Exception При любой ошибке API
     */
    private function post($url, $data) {
        $response = Http::asForm()
            ->accept('application/json')
            ->post(config('notisend.api_url') . $url, $data);

        // Логирование ошибок для отладки и мониторинга
        if (!$response->successful()) {
            Log::error('NotiSend SMS error', [
                'data'    => $data,
                'response' => $response->body(),
                'status'   => $response->status(),
            ]);

            throw new \Exception('NotiSend API error: ' . $response->body());
        }

        return $response;
    }

    /**
     * Отправляет SMS-сообщение через Notisend.
     *
     * Форматирует номер: убирает все нецифровые символы.
     *
     * @param string $phone   Номер телефона (любой формат)
     * @param string $message Текст сообщения (или код)
     * @return \Illuminate\Http\Client\Response
     */
    public function sendSMS($phone, $message) {
        $data = [
            'project' => config('notisend.sms_project'),
            'recipients' => preg_replace('/\D/', '', $phone),
            'message' => $message,
        ];

        // Добавляем подпись
        $data['sign'] = $this->signRequest($data, config('notisend.sms_key'));

        return $this->post('/message/send', $data);
    }

    /**
     * Отправляет голосовое сообщение (код озвучивается).
     *
     * Использует отдельный проект и ключ (voice_project, voice_key).
     *
     * @param string $phone Номер телефона
     * @param string $code  Код для озвучивания (например, "1111")
     * @return \Illuminate\Http\Client\Response
     */
    public function sendVoice($phone, $code) {
        $data = [
            'project' => config('notisend.voice_project'),
            'recipients' => preg_replace('/\D/', '', $phone),
            'message' => $code,
        ];

        // Добавляем подпись
        $data['sign'] = $this->signRequest($data, config('notisend.voice_key'));

        return $this->post('/message/send', $data);
    }

    /**
     * Отправляет код авторизации по двум каналам.
     *
     * Поведение:
     *  - Если notisend.sms_disable = false -> отправка SMS
     *  - Если notisend.voice_disable = false -> отправка голосового
     *
     * Полезно для:
     *  - Доставки в условиях блокировок SMS
     *  - Повышения надёжности (резервный канал)
     *
     * @param string $phone Номер телефона
     * @param string $code  Код (4 символа)
     * @return void
     */
    public function sendAuthCode($phone, $code) {
        // Отправляем SMS, если не отключено
        if (!config('notisend.sms_disable')) {
            $this->sendSMS($phone, $code);
        }
        // Отправляем голосовой звонок, если не отключено
        if (!config('notisend.voice_disable')) {
            $this->sendVoice($phone, $code);
        }
    }
}