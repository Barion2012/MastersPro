<template lang="pug">
header
    .container 
        NuxtLink(to="/").logo
            img(src="/images/logo.png")
        .menu
            Anchor(href="#how-it-works") Как это работает?
            Anchor(href="#faq") Часто задаваемые вопросы
            //a Стать исполнителем 
        .btn-wrapper
            BtnLogin
        MobileMenu(:menu="menu")
</template>

<script setup>
const menu = ref([
    { text: 'Как это работает?', href: '#how-it-works', type: 'anchor' },
    { text: 'Часто задаваемые вопросы', href: '#faq', type: 'anchor' },
])
</script>