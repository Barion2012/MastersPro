<template lang="pug">
.row
    .col
        span {{ ticket.id }}
    .col 
        UserPreview(:name="ticket.user.name" :role="ticket.user.role" :phone="ticket.user.phone")
    .col 
        span {{ ticket.title }}
    .col 
        span {{ ticket.created }}
    .col 
        span.tag.blue(v-if="ticket.status === 'open'") Открыт
        span.tag.green(v-if="ticket.status === 'closed'") Закрыт
    .col
        NuxtLink(:to="`/support/ticket/${ticket.id}`")
            BtnIcon(icon="/images/icons/btn-icon/show.png")
</template>

<script setup>
const props = defineProps(['ticket', 'link'])
</script>