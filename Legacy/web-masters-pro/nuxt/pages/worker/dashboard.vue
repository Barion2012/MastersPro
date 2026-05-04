<template lang="pug">
main 
    section.dashboard.pd-header.bg-screen.screen
        .container
            Pagetitle(:title="`Добро пожаловать! 👋`")
            .app-menu 
                NuxtLink.menu-item(v-for="item in menu" :to="item.link")
                    .icon 
                        img(:src="item.icon")
                    span {{ item.title }}
            .btn-wrapper.right.mb-10.mt-auto
                button.btn.sm.danger(@click="logout") Выйти
</template>

<script setup>
definePageMeta({
    middleware: ['sanctum:auth', 'role:worker'],
});

const menu = ref([
    {
        title: 'Работа',
        icon: '/images/icons/menu/3.png',
        link: '/worker/job'
    },
    {
        title: 'Профиль',
        icon: '/images/icons/menu/10.png',
        link: '/worker/profile'
    },
    {
        title: 'Поддержка',
        icon: '/images/icons/menu/8.png',
        access: true,
        link: '/support'
    },
]);

async function logout() {
    try {
        await $api.logout()
        navigateTo('/')
    }
    catch(error) {
        $api.handleError(error)
    }
}
</script>