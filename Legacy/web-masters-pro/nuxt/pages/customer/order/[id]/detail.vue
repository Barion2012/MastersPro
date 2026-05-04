<template lang="pug">
main 
    section.order-page.pd-header.bg-screen.screen
        OrderDetail(:orderId="route.params.id")
</template>

<script setup>
definePageMeta({
    middleware: ['sanctum:auth', 'role:customer'],
});

const route = useRoute()
</script>