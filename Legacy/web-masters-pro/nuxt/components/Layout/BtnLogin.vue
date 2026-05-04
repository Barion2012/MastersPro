<template lang="pug">
    NuxtLink(to="/login" v-if="!isAuthenticated").btn Авторизация
    NuxtLink(to="/admin/dashboard" v-else-if="user.role === 'admin'").btn Панель
    NuxtLink(to="/admin/dashboard" v-else-if="user.role === 'superuser'").btn Панель
    NuxtLink(to="/admin/dashboard" v-else-if="user.role === 'manager'").btn Панель
    NuxtLink(to="/login" v-else-if="user.role === 'user'").btn Личный кабинет
    NuxtLink(to="/worker/dashboard" v-else-if="user.role === 'worker'").btn Личный кабинет
    NuxtLink(to="/customer/dashboard" v-else-if="user.role === 'customer'").btn Личный кабинет
</template>

<script setup>
const { isAuthenticated } = useSanctumAuth()
const user = useSanctumUser()
</script>