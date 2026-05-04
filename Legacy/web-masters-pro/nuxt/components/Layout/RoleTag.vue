<template lang="pug">
.tag.red(v-if="role === 'admin' || role === 'superuser'") Администратор
.tag.yellow(v-else-if="role === 'moderator'") Модератор
.tag.purple(v-else-if="role === 'manager'") Менеджер
.tag.blue(v-else-if="role === 'customer'") Заказчик
.tag.green(v-else-if="role === 'worker'") Рабочий
    
</template>

<script setup>
const props = defineProps(['role'])    
</script>