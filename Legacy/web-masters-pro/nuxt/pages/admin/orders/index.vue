<template lang="pug">
section.orders-page.pd-header.bg-screen.screen
    .container
        Pagetitle(:title="`Заказы`" :goback="true")
        .content-card.mt-10
            .table.admin-orders
                .row.head
                    .col
                        span №
                    .col 
                        span ID
                    .col
                        span Заказчик
                    .col
                        span Дата
                    .col
                        span Общая стоимость
                    .col
                        span Статус
                    .col
                        span Подробнее
                .row(v-for="item in orders")
                    .col
                        span {{ item.index }}
                    .col
                        span {{ item.id }}
                    .col 
                        span {{ item.customer }}
                    .col 
                        span {{ item.created_at }}
                    .col 
                        span {{ item.total_price }} р.
                    .col 
                        span.tag.blue(v-if="item.status === 'search'") Поиск мастеров
                        span.tag.green(v-if="item.status === 'done'") Завершен
                        span.tag.red(v-if="item.status === 'canceled'") Отменен
                        span.tag.cyan(v-if="item.status === 'work'") В работе
                    .col
                        NuxtLink(:to="`/admin/orders/${item.id}`")
                            BtnIcon(icon="/images/icons/btn-icon/show.png")
            Paginator(:lastPage="lastPage" :currentPage="currentPage" @change-page="getOrdersList")
</template>

<script setup>
definePageMeta({
    middleware: ['sanctum:auth', 'role:administration'],
});

const orders = ref([])
const currentPage = ref(0)
const lastPage = ref(0)

async function getOrdersList(pageIndex) {
    try {
        let response = await $api.get(`/order/list?page=${pageIndex}`)
        orders.value = response.data
        currentPage.value = response.current_page
        lastPage.value = response.last_page
    }
    catch (error) {
        $notice.handleError(error)
    }
}

getOrdersList(1)
</script>