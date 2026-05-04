import Echo from 'laravel-echo';
import Pusher from 'pusher-js';

export default defineNuxtPlugin(() => {
    window.Pusher = Pusher;
    
    globalThis.$echo = window.Echo = new Echo({
        broadcaster: 'reverb',
        key: '0FTGVdISJr',
        wsHost: window.location.host,
        wsPort: 80,
        wssPort: 443,
        forceTLS: false,
        enabledTransports: ['ws', 'wss'],
    });
    
})