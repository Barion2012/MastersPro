<template lang="pug">
main
    section.payments.pd-header.bg-screen.screen
        .container 
            Pagetitle(title="Платежи" :goback="true")
            .content-card
                .table.payments 
                    .row.head
                        .col
                            span ID
                        .col
                            span Заказчик
                        .col
                            span Исполнитель
                        .col
                            span Сумма
                        .col
                            span Статус
                        .col
                            span Дата
                        .col
                            span Ответ SberAPI
                    .row(v-for="payment in payments")
                        .col 
                            span {{ payment.id }}
                        .col
                            span {{ payment.customer }}
                        .col
                            span {{ payment.worker }}
                        .col
                            span {{ payment.amount }} ₽
                        .col
                            .tag.red(v-if="payment.status === 'failed'") Ошибка
                            .tag.green(v-else-if="payment.status === 'completed'") Выполнен
                            .tag.yellow(v-else-if="payment.status === 'created'") Создан
                            .tag.blue(v-else-if="payment.status === 'pending'") В ожидании
                            .tag.gray(v-else) Неизвестно
                        .col
                            span {{ payment.payed_at }}
                        .col
                            BtnIcon(icon="/images/icons/btn-icon/show.png" @click="showResponse(payment.response)" v-if="payment.response")
                            span(v-else) Нет ответа
                Paginator(:lastPage="lastPage" :currentPage="currentPage" @change-page="loadPaymentsList")
                Modal(v-model="showModal")
                    span.modal-title Ответ SberAPI
                    pre {{ response }}
</template>

<script setup>
definePageMeta({
    middleware: ['sanctum:auth', 'role:administration'],
});

const payments = ref([]);
const currentPage = ref(0)
const lastPage = ref(0)
const response = ref({});
const showModal = ref(false);

async function loadPaymentsList(pageIndex) {
    try {
        const response = await $api.get(`/payment/list?page=${pageIndex}`);
        payments.value = response.data;
        currentPage.value = response.current_page
        lastPage.value = response.last_page
    } catch (error) {
        $notice.handleError(error)
    }
}

function showResponse(json) {
    response.value = json;
    showModal.value = true;
}

onMounted(() => {
    loadPaymentsList(1);
});
</script>