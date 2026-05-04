<template lang="pug">
    .active-work
        .app-title-wrapper
            h1.app-title Активные объекты
            .app-subtitle После окончания смены вам необходимо сделать фото своей работы и загрузить их
        .works-list
            WorkReport(:order="acceptWork")
            WorkShift(:order="acceptWork")
                button.btn.secondary.sm(@click="showModalDoc = true") Показать документы
            Modal(v-model="showModalDoc").user-modal
                span.modal-title Документы
                .files
                    .info(v-if="acceptWork.files.passport_scan")
                        b Паспорт 
                        a(:href="`/api/order/${acceptWork.order.id}/worker/${user.worker.id}/file/${acceptWork.files.passport_scan}/get`" target="_blank").link Скачать
                    .info(v-if="acceptWork.files.snils")
                        b СНИЛС 
                        a(:href="`/api/order/${acceptWork.order.id}/worker/${user.worker.id}/file/${acceptWork.files.snils}/get`" target="_blank").link Скачать
                    .info(v-if="acceptWork.files.migration_card")
                        b Миграционная карта 
                        a(:href="`/api/order/${acceptWork.order.id}/worker/${user.worker.id}/file/${acceptWork.files.migration_card}/get`" target="_blank").link Скачать
                    .info(v-if="acceptWork.files.patent")
                        b Патент
                        a(:href="`/api/order/${acceptWork.order.id}/worker/${user.worker.id}/file/${acceptWork.files.patent}/get`" target="_blank").link Скачать
                    .info(v-if="acceptWork.files.dms")
                        b ДМС
                        a(:href="`/api/order/${acceptWork.order.id}/worker/${user.worker.id}/file/${acceptWork.files.dms}/get`" target="_blank").link Скачать
</template>

<script setup>
const user = useSanctumUser()

const props = defineProps({
    acceptWork: {
        type: Object,
        default: {}
    }
})
const showModalDoc = ref(false)
</script>