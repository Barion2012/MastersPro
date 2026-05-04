<template lang="pug">
main 
    section.orders-page.pd-header.bg-screen.screen
        .container
            Pagetitle(:title="`Мои заказы`")
            .content-card
                .table.orders
                    .row.head
                        .col
                            span №
                        .col
                            span Дата
                        .col
                            span Адрес
                        .col
                            span Общая стоимость
                        .col
                            span Статус
                        .col
                            span Подробнее
                    RowOrder(v-for="item in orders" :order="item")
                Paginator(:lastPage="lastPage" :currentPage="currentPage" @change-page="getUserOrdersList")
</template>

<script setup>
definePageMeta({
    middleware: ['sanctum:auth', 'role:customer'],
});

const orders = ref([])
const currentPage = ref(0)
const lastPage = ref(0)
const user = useSanctumUser()

async function getUserOrdersList(pageIndex) {
    try {
        let response = await $api.get(`/order/user/${user.value.id}/list?page=${pageIndex}`)
        orders.value = response.data
        currentPage.value = response.current_page
        lastPage.value = response.last_page
    }
    catch (error) {
        $notice.handleError(error)
    }
}

getUserOrdersList(1)
</script>