import { defineConfig } from 'vitepress'

// https://vitepress.dev/reference/site-config
export default defineConfig({
  base: '/docs/',
  vite: {
    server: {
      port: 5174,
      host: '0.0.0.0',
      allowedHosts: true
    },
  },
  cleanUrls: true,
  title: "Docker Laravel Nuxt App",
  description: "Шаблон для разработки Docker + Laravel + Nuxt приложения",
  themeConfig: {
    // https://vitepress.dev/reference/default-theme-config
    nav: [
      { text: 'Главная', link: '/' },
      { text: 'Examples', link: '/markdown-examples' }
    ],

    sidebar: [
      {
        text: 'Введение',
        items: [
          { text: 'О проекте', link: '/intro/about' },
          { text: 'Установка', link: '/intro/setup' },
          { text: 'Файловая структура', link: '/intro/structure' },
        ]
      },
      {
        text: 'Laravel',
        items: [
          { text: 'О контейнере', link: '/laravel/about' },
          { text: 'Контроллеры', link: '/laravel/controllers' },
          { text: 'Модели', link: '/laravel/models' },
          { text: 'Система прав', link: '/laravel/permissions' },
          { text: 'Роуты', link: '/laravel/routes' },
        ]
      },
      {
        text: 'Nuxt',
        items: [
          { text: 'О контейнере', link: '/nuxt/about' },
          { text: 'Шаблоны', link: '/nuxt/layouts' },
          { text: 'Компоненты', link: '/nuxt/components' },
          { text: 'Страницы', link: '/nuxt/pages' },
          { text: 'Плагины', link: '/nuxt/plugins' },
          { text: 'Хранилище', link: '/nuxt/storage' },
        ]
      },
      {
        text: 'Примеры',
        items: [
          { text: 'Runtime API Examples', link: '/api-examples' },
          { text: 'Runtime API Examples 2', link: '/markdown-examples' }
        ]
      }
    ],

    socialLinks: [
      { icon: 'git', link: 'https://gitverse.ru/igorkrosh/digital-dealer' }
    ]
  }
})
