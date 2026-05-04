<template lang="pug">
    .work-item
        ClientOnly
            .info-wrapper(v-if="order.place || order.order_files.place_file")
                .label Как добраться
                .info(v-html="order.place" v-if="order.place")
                .filelist
                    a(:href="order.order_files.place_file" target="_blank").file
                        span Скачать файл
            .info-wrapper(v-if="order.quality || order.order_files.quality_file")
                .label Критерии качества
                .info(v-html="order.quality" v-if="order.quality")
                .filelist(v-if="order.order_files.quality_file")
                    a(:href="order.order_files.quality_file" target="_blank").file
                        span Скачать файл
            .info-wrapper(v-if="order.info || order.order_files.info_file")
                .label Дополнительная информация
                .info(v-html="order.info" v-if="order.info")
                .filelist(v-if="order.order_files.info_file")
                    a(:href="order.order_files.info_file" target="_blank").file
                        span Скачать файл
        .grid
            .col 
                .param 
                    .label Вид работ 
                    .value {{ order.work.profession }}
            .col
                .param 
                    .label Адрес объекта 
                    .value {{ order.address }}
            .col
                .param 
                    .label Место встречи
                    .value {{ order.meeting_point }}
            .col
                .param 
                    .label Дата смены
                    .value {{ order.current_work_shift.date}}
            .col
                .param 
                    .label Оплата за смену
                    .value {{ parseInt(order.current_work_shift.price) }}
            .col 
                .btn-wrapper.right
                    slot
</template>

<script setup>
const props = defineProps({
    order: {
        type: Object,
        required: true
    }
})
</script>