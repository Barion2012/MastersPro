<template lang="pug">
    .row.worker
        .col
            span {{ index }}
        .col 
            span {{ worker.name }}
        .col 
            span {{ worker.profession }}
        //.col 
            span {{ worker.professionLevel }}
        .col 
            span.tag.green(v-if="worker.citizenship === 'citizen'") Гражданин РФ
            span.tag.blue(v-else-if="worker.citizenship === 'eaeu'") ЕАЭС
            span.tag(v-else) Прочее
        .col
            span.tag(v-if="worker.status === 'pending'") На подтверждении
            span.tag.yellow(v-else-if="worker.status === 'confirmed'") Выход подтвержден
            span.tag.blue(v-else-if="worker.status === 'arrive'") На месте
            span.tag.green(v-else-if="worker.status === 'accepted'") Утвержден
            span.tag.red(v-else-if="worker.status === 'refused'") Отказ
        .col 
            BtnIcon(icon="/images/icons/btn-icon/show.png" @click.stop="showModal = !showModal")
        .col
            .btn-wrapper
                BtnIcon(
                    v-if="worker.status == 'arrive'"
                    icon="/images/icons/btn-icon/accept.png" 
                    @click="acceptWorker"
                ).success.mr-10 
                BtnIcon(
                    v-if="worker.status == 'arrive' || (worker.status == 'accepted' && !worker.current_work_shift.three_hours_passed)"
                    icon="/images/icons/btn-icon/refuse.png" 
                    @click="refuseWorker" 
                ).danger
    Modal(v-model="showModal").user-modal
        span.modal-title Информация о мастере
        h4 Общая информация
        .info
            b ФИО:
            span {{ worker.name }}
        .info 
            b Профессия:
            span {{ worker.profession }}
        .info 
            b Уровень професии:
            span {{ worker.professionLevel }} 
        .info 
            b Гражданство:
            div 
                span.tag.green(v-if="worker.citizenship === 'citizen'") Гражданин РФ
                span.tag.blue(v-else-if="worker.citizenship === 'eaeu'") ЕАЭС
                span.tag(v-else) Прочее 
        h4 Файлы
        .files
            .info 
                b Договор: 
                span(
                    @click.prevent="downloadFile(`/order/${route.params.id}/worker/${worker.worker_id}/contract/get`, `Договор_${worker.name}`)"
                ).link Скачать
            .info(v-if="worker.files.passport_scan")
                b Паспорт 
                span(
                    @click.prevent="downloadFile(`/order/${route.params.id}/worker/${worker.worker_id}/file/${worker.files.passport_scan}/get`, `Паспорт_${worker.name}`)"
                ).link Скачать
            .info(v-if="worker.files.snils")
                b СНИЛС 
                span(
                    @click.prevent="downloadFile(`/order/${route.params.id}/worker/${worker.worker_id}/file/${worker.files.snils}/get`, `СНИЛС_${worker.name}`)"
                ).link Скачать
            .info(v-if="worker.files.migration_card")
                b Миграционная карта 
                span(
                    @click.prevent="downloadFile(`/order/${route.params.id}/worker/${worker.worker_id}/file/${worker.files.migration_card}/get`, `Миграционная_карта_${worker.name}`)"
                ).link Скачать
            .info(v-if="worker.files.patent")
                b Патент
                span(
                    @click.prevent="downloadFile(`/order/${route.params.id}/worker/${worker.worker_id}/file/${worker.files.patent}/get`, `Патент_${worker.name}`)"
                ).link Скачать
            .info(v-if="worker.files.dms")
                b ДМС
                span(
                    @click.prevent="downloadFile(`/order/${route.params.id}/worker/${worker.worker_id}/file/${worker.files.dms}/get`, `ДМС_${worker.name}`)"
                ).link Скачать
        //.btn-wrapper.center.mt-20(v-if="canEdit")
            button.btn.sm.mr-20.secondary(v-if="worker.status === 'arrive'" @click="acceptWorker") Встретил мастера
            button.btn.sm.danger(@click="refuseWorker") Отказаться от мастера


</template>

<script setup>
const props = defineProps(['index', 'worker', 'canEdit'])
const showModal = ref(false)
const route = useRoute()

async function acceptWorker() {
    try {
        await $api.post(`/order/${route.params.id}/worker/accept`, {
            worker_id: props.worker.worker_id
        })
        $notice.success('Вы приняли мастера')
        showModal.value = false
    }
    catch (error) {
        $notice.handleError(error)
    }
}

async function refuseWorker() {
    try {
        await $api.post(`/order/${route.params.id}/worker/refuse`, {
            worker_id: props.worker.worker_id
        })
        $notice.success('Вы отказались от мастера')
        showModal.value = false
    }
    catch (error) {
        $notice.handleError(error)
    }
}

async function downloadFile(url, fileName) {
    try {
        const response = await $api.get(url)

        const blob = response;
        const fileUrl = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = fileUrl;
        a.download = fileName; // имя файла
        document.body.appendChild(a);
        a.click();

        // Очистка
        window.URL.revokeObjectURL(fileUrl);
        document.body.removeChild(a);
    }
    catch (error) {
        $notice.handleError(error)
    }
}
</script>