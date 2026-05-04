<template lang="pug">
    main 
        section.users-group.pd-header.bg-screen.screen
            .container 
                Pagetitle(title="Заказчики" :goback="true")
                .content-card.mt-10
                    .table.customers
                        .row.edit.head
                            .col 
                                span №
                            .col 
                                span ID
                            .col 
                                span Название
                            .col 
                                span Email
                            .col 
                                span Баланс 
                            .col 
                                span Тип заказчика
                            .col 
                                span Подробнее
                        EditRowCustomer(v-for="(item, index) in customers" :user="item" :index="index + 1" @update="loadCustomerList(currentPage)")
                    Paginator(:lastPage="lastPage" :currentPage="currentPage" @change-page="loadCustomerList")
</template>

<script setup>
definePageMeta({
    middleware: ['sanctum:auth', 'role:admin'],
});

const customers = ref([])
const currentPage = ref(0)
const lastPage = ref(0)

async function loadCustomerList(pageIndex) {
    try {
        const response = await $api.get(`/user/list/customer?page=${pageIndex}`)
        customers.value = response.data
        currentPage.value = response.current_page
        lastPage.value = response.last_page
    }
    catch (error) {
        $notice.handleError(error)
    }
}
loadCustomerList(1)
</script>