<template lang="pug">
    .row.order 
        .col
            span {{ order.id }}
        .col 
            span {{ order.created_at }}
        .col 
            span {{ order.address }}
        .col 
            span {{ order.total_price }} р.
        .col 
            span.tag.blue(v-if="order.status === 'search'") Поиск мастеров
            span.tag.green(v-if="order.status === 'done'") Завершен
            span.tag.red(v-if="order.status === 'canceled'") Отменен
            span.tag.cyan(v-if="order.status === 'work'") В работе
        .col
            NuxtLink(:to="`/customer/order/${order.id}/detail`")
                BtnIcon(icon="/images/icons/btn-icon/show.png")
            //NuxtLink(:to="`/customer/order/${order.id}/detail`" v-if="order.status === 'canceled' || order.status === 'completed' || order.status === 'work'")
                BtnIcon(icon="/images/icons/btn-icon/show.png" @click.stop="")
            //NuxtLink(:to="`/customer/order/${order.id}/search`" v-if="order.status === 'search'")
                BtnIcon(icon="/images/icons/btn-icon/show.png" @click.stop="")
</template>

<script setup>
const props = defineProps({
    order: {
        type: Object,
        required: true
    }
})
</script>