<template lang="pug">
main 
    section.order-search-page.pd-header.bg-screen.screen
        .container(v-if="order.id")
            .search-title(v-if="workers.length == 0")
                h1 Ожидаем отклик мастеров... 
                h2 Приблизительное время ожидания: 
                    span 20 мин
            .founded-masters(v-else)
                h1 Откликнувшиеся мастера в вашем заказе:
                .content-card.mt-10
                    .table.workers
                        .row.head
                            .col 
                                span №
                            .col 
                                span ФИО
                            .col 
                                span Профессия
                            .col 
                                span Гражданство 
                            .col 
                                span Статус
                            .col 
                                span Подробнее
                            .col 
                                span Встретил мастера
                        RowWorker(v-for="(item, index) in workers" :worker="item" :index="index + 1")
                .btn-wrapper.center.mt-20
                    NuxtLink.btn.primary.mr-20(:to="`/customer/order/${order.id}/detail`") Перейти в заказ
                    a.btn.secondary(:href="`/api/order/${order.id}/download/excel`" target="_blank") Скачать .xlsx (Excel)
            .buy-equipment.indev
                h2 Рекомендуем ознакомится с предложениями наших партнеров: 
                .grid
                    .content-card
                        .row
                            .col
                                span.name Спецодежда
                                span.price Стоимость аренды в сутки: 
                                    .green 1000 р.
                            .col
                                InputCounter(label="Количество" )
                            .col 
                                DateRange(label="Дата" )
                        .btn-wrapper.between
                            button.btn.primary.sm Добавить к заказу
                            .total-price Общая стоимость: 
                                .green 1000 р.
                    .content-card
                        .row
                            .col
                                span.name Спецодежда
                                span.price Стоимость аренды в сутки: 
                                    .green 1000 р.
                            .col
                                InputCounter(label="Количество" )
                            .col 
                                DateRange(label="Дата" )
                        .btn-wrapper.between
                            button.btn.primary.sm Добавить к заказу
                            .total-price Общая стоимость: 
                                .green 1000 р.
                    .content-card
                        .row
                            .col
                                span.name Спецодежда
                                span.price Стоимость аренды в сутки: 
                                    .green 1000 р.
                            .col
                                InputCounter(label="Количество" )
                            .col 
                                DateRange(label="Дата" )
                        .btn-wrapper.between
                            button.btn.primary.sm Добавить к заказу
                            .total-price Общая стоимость: 
                                .green 1000 р.
                    .content-card
                        .row
                            .col
                                span.name Спецодежда
                                span.price Стоимость аренды в сутки: 
                                    .green 1000 р.
                            .col
                                InputCounter(label="Количество" )
                            .col 
                                DateRange(label="Дата" )
                        .btn-wrapper.between
                            button.btn.primary.sm Добавить к заказу
                            .total-price Общая стоимость: 
                                .green 1000 р.
                    
</template>

<script setup>
definePageMeta({
    middleware: ['sanctum:auth', 'role:customer'],
});

const route = useRoute()
const order = ref({})
const workers = ref({})

async function getOrder() {
    try {
        const response = await $api.get(`/order/${route.params.id}/get`)
        order.value = response.order
        workers.value = response.workers
    }
    catch (error) {
        $notice.handleError(error)
    }
}

function listenNotifications() {
    $echo.channel(`order-notification.${route.params.id}`)
        .listen('UpdateOrder', (e) => {
            getOrder()
        })
}

onMounted(() => {
    listenNotifications()
})

onBeforeUnmount(() => {
    $echo.leave(`order-notification.${route.params.id}`);
})

getOrder()
</script>