<template lang="pug">
    .work-item(v-if="order.current_work_shift")
        .report(v-if="order.current_work_shift.report")
            .report-info
                h3 Отчет загружен 
                p(v-if="order.current_work_shift.report.status == 'pending'") Ваш отчет находится на рассмотрении
                p(v-if="order.current_work_shift.report.status == 'accepted'") Ваш отчет принят
                p(v-if="order.current_work_shift.report.status == 'refused'") Ваш отчет отклонен
        .report(v-else)
            .work-shift
                .row
                    b Текущая смена: {{ order.current_work_shift.index }}
                .row
                    b Выработанная смен: 
                    .shift-progress 
                        span {{ order.current_work_shift.index }}/{{ order.work_shifts.length }}
                        .bg(:style="{ width: `${(order.current_work_shift.index / order.work_shifts.length) * 100}%` }")
            .photo-report 
                b Загрузите от 1 до 4 фотографий сегодняшней работы: 
                .photos 
                    InputImageSm(v-model="report.photo_1" name="photo_1")
                    InputImageSm(v-model="report.photo_2" name="photo_2")
                    InputImageSm(v-model="report.photo_3" name="photo_3")
                    InputImageSm(v-model="report.photo_4" name="photo_4")
            .btn-wrapper 
                button.btn.primary.sm(@click="sendReport") Отправить
</template>

<script setup>
const props = defineProps({
    order: {
        type: Object,
        required: true
    }
})

const emit = defineEmits(['send-report'])

const report = ref({
    'photo_1': null,
    'photo_2': null,
    'photo_3': null,
    'photo_4': null,
})

async function sendReport() {
    try {
        await $api.post(`/work/${props.order.work.id}/shift/${props.order.current_work_shift.id}/report`, $api.createFormData(report.value))
        $notice.success('Отчет успешно отправлен')
    }
    catch (error) {
        $notice.handleError(error)
    }
}
</script>