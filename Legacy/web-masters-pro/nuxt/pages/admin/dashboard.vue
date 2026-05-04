<template lang="pug">
main 
    section.dashdoard.pd-header.bg-screen.screen
        .container
            Pagetitle(:title="`Добро пожаловать! 👋`")
            .app-menu 
                NuxtLink.menu-item(v-for="item in filteredMenu" :to="item.link")
                    .icon 
                        img(:src="item.icon")
                    span {{ item.title }}
            .btn-wrapper.right.mb-10.mt-auto
                button.btn.sm.danger(@click="logout") Выйти
</template>

<script setup>
definePageMeta({
    middleware: ['sanctum:auth', 'role:administration'],
});

const { checkPermission } = $api;
const user = useSanctumUser()
const menu = ref([
    {
        title: 'Статистика',
        icon: '/images/icons/menu/1.png',
        access: checkPermission('App.stats'),
        link: '/admin/stats'
    },
    {
        title: 'Заказы',
        icon: '/images/icons/menu/2.png',
        access: checkPermission('Order.read'),
        link: '/admin/orders'
    },
    {
        title: 'Мастера',
        icon: '/images/icons/menu/3.png',
        access: checkPermission('Worker.save'),
        link: '/admin/groups/workers'
    },
    {
        title: 'Заказчики',
        icon: '/images/icons/menu/4.png',
        access: checkPermission('Customer.save'),
        link: '/admin/groups/customers'
    },
    {
        title: 'Поставщики',
        icon: '/images/icons/menu/5.png',
        access: false,
        link: '#'
    },
    {
        title: 'Профессии',
        icon: '/images/icons/menu/6.png',
        access: checkPermission('Profession.save'),
        link: '/admin/professions'
    },
    {
        title: 'Оборудование',
        icon: '/images/icons/menu/7.png',
        access: false,
        link: '#'
    },
    {
        title: 'Поддержка',
        icon: '/images/icons/menu/8.png',
        access: checkPermission('Ticket.handle'),
        link: '/support'
    },
    {
        title: 'Модерация',
        icon: '/images/icons/menu/9.png',
        access: false,
        link: '#'
    },
    {
        title: 'Администраторы',
        icon: '/images/icons/menu/10.png',
        access: checkPermission('User.save'),
        link: '/admin/groups/admin'
    },
    {
        title: 'Платежи',
        icon: '/images/icons/menu/2.png',
        access: checkPermission('PaymentRequest.read'),
        link: '/admin/payments'
    },
    {
        title: 'Управление платформой',
        icon: '/images/icons/menu/11.png',
        access: true,
        link: '/admin/settings'
    },
]);

const filteredMenu = computed(() => {
    return menu.value.filter(item => item.access);
})

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