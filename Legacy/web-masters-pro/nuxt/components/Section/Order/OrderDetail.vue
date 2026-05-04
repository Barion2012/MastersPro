<template lang="pug">
    .order-section 
        .container
            Pagetitle(:title="`Заказ №${order.id} от ${order.created_at}`" :goback="true")
            TabSwitcher(:tabs="tabsOrder" v-model="activeTab")
            TabViewer(:height="tabViewerHeight")
                Tab(name="info" :activeTab="activeTab" v-model="tabViewerHeight")
                    .work-item
                        .info-wrapper
                            .label Как добраться
                            .info(v-html="order.place")
                        .info-wrapper
                            .label Критерии качества
                            .info(v-html="order.quality")
                        .info-wrapper(v-if="order?.order_files?.quality_file")
                            .filelist
                                a(:href="order.order_files.quality_file" target="_blank").file
                                    span Скачать файл
                        .info-wrapper
                            .label Дополнительная информация
                            .info(v-html="order.info")
                        .info-wrapper(v-if="order?.order_files?.info_file")
                            .filelist
                                a(:href="order.order_files.info_file" target="_blank").file
                                    span Скачать файл
                        .grid
                            .col
                                .param 
                                    .label Адрес объекта 
                                    .value {{ order.address }}
                            .col
                                .param 
                                    .label Место встречи
                                    .value {{ order.meeting_point }}
                Tab(name="works" :activeTab="activeTab" v-model="tabViewerHeight")
                    .content-card.mt-10
                        .table.works
                            .row.head
                                .col 
                                    span №
                                .col 
                                    span Вид работы
                                .col 
                                    span Кол-во
                                .col 
                                    span Найдено
                                .col 
                                    span Отказались
                                .col
                                    span Цена за смену
                                .col 
                                    span Статус
                            RowWork(v-for="(item, index) in works" :work="item" :index="index + 1")
                Tab(name="masters" :activeTab="activeTab" v-model="tabViewerHeight")
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
                            RowWorker(v-for="(item, index) in workers" :worker="item" :index="index + 1" :canEdit="order.user_id == user.id")
                    .btn-wrapper.right.mt-20
                        a.btn.secondary(:href="`/api/order/${order.id}/download/excel`" target="_blank") Скачать .xlsx (Excel)
                Tab(name="reports" :activeTab="activeTab" v-model="tabViewerHeight")
                    .reports-wrapper
                        .content-card.mt-10.mb-20
                            .table.reports
                                .report.row.head
                                    .col 
                                        span Смена
                                    .col 
                                        span Дата
                                    .col 
                                        span Автор отчета
                                    .col 
                                        span Статус
                                    .col 
                                        span Комментарий
                                    .col
                                        span Отчет
                                RowReport(v-for="(item, index) in reports" :report="item" :index="index + 1" :canEdit="order.user_id == user.id" @update="getOrder")
            
</template>

<script setup>
const props = defineProps(['orderId'])
const user = useSanctumUser()

const order = ref({})
const reports = ref({})
const workers = ref({})
const works = ref({})

const tabViewerHeight = ref(450)
const activeTab = ref('info')
const tabsOrder = ref([
    {
        title: 'Инфо',
        value: 'info'
    },
    {
        title: 'Виды работ',
        value: 'works'
    },
    {
        title: 'Мастера',
        value: 'masters'
    },
    {
        title: 'Отчеты',
        value: 'reports'
    }
])

async function getOrder() {
    try {
        const response = await $api.get(`/order/${props.orderId}/get`)
        order.value = response.order
        reports.value = response.reports
        workers.value = response.workers
        works.value = response.works
    }
    catch (error) {
        $notice.handleError(error)
    }
}

function listenNotifications() {
    $echo.channel(`order-notification.${props.orderId}`)
        .listen('UpdateOrder', (e) => {
            getOrder()
        })
}

onMounted(() => {
    listenNotifications()
})

onBeforeUnmount(() => {
    $echo.leave(`order-notification.${props.orderId}`);
})

getOrder()
</script>