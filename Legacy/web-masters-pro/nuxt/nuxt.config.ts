// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
    ssr: true,
    compatibilityDate: '2024-11-01',
    devtools: {enabled: true},
    experimental: {appManifest: false},
    vite: {
        server: {
            hmr: {
                host: process.env.APP_DOMAIN,
                protocol: "wss",
                clientPort: 443
            },
            strictPort: true,
            watch: {
                usePolling: true,
            },
            allowedHosts: true
        },
    },
    nitro: {
        routeRules: {
            '/api/**': {
                proxy: process.env.APP_URL + '/api/**',
            },
        },
    },
    css: [
        '~/assets/scss/style.scss',
        '~/assets/scss/response.scss',
        '~/assets/scss/fonts.scss',
        '~/assets/scss/animations.scss',
        'vue-final-modal/style.css'
    ],
    modules: ['@pinia/nuxt', 'nuxt-auth-sanctum', 'vue-yandex-maps/nuxt'],
    app: {
        pageTransition: { name: 'fade', mode: 'out-in' },
        layoutTransition: { name: 'fade', mode: 'out-in' }
    },
    sanctum: {
        baseUrl: process.server ? 'https://nginx' : process.env.APP_URL,
        mode: 'cookie',
        endpoints: {
            csrf: '/api/csrf-cookie',
            login: '/api/auth/login',
            logout: '/api/auth/logout',
            user: '/api/user',
        },
        redirect: {
            onAuthOnly: '/',
        },
        client: {
            initialRequest: true
        },
        logLevel: 1
    },
    yandexMaps: {
        apikey: process.env.YANDEX_MAPS_API_KEY,
    },
    components: [
        {
            path: '~/components',
            pathPrefix: false
        }
    ],
    routeRules: {
    },
    runtimeConfig: {
        public: {
            APP_URL: process.env.APP_URL,
            APP_DOMAIN: process.env.APP_DOMAIN,
            YANDEX_MAPS_API_KEY: process.env.YANDEX_MAPS_API_KEY
        }
    }
})