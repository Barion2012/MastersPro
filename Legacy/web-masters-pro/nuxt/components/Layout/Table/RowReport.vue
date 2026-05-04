<template lang="pug">
.report.row
    .index Номер смены: {{ report.index }}
    .date {{ report.date }}
    .worker 
        span.name {{ report.name }}
        span {{ report.profession }}
        span {{ report.professionLevel }}
    .status
        .tag.blue(v-if="report.status == 'pending'") Ожидает проверки
        .tag.green(v-if="report.status == 'accepted'") Принят
        .tag.red(v-if="report.status == 'refused'") Отклонен
    .comment 
        span {{ report.comment }}
    .show 
        BtnIcon(icon="/images/icons/btn-icon/show.png" @click="showModal = !showModal")
    Modal(v-model="showModal").user-modal
        .info
            b ФИО:
            span {{ report.name }}
        .info 
            b Профессия:
            span {{ report.profession }}
        .info 
            b Уровень професии:
            span {{ report.professionLevel }}
        .photos.mt-20.mb-20
            a(:href="report.photo_1" target="_black" v-if="report.photo_1")
                img(:src="report.photo_1")
            a(:href="report.photo_2" target="_black" v-if="report.photo_2")
                img(:src="report.photo_2")
            a(:href="report.photo_3" target="_black" v-if="report.photo_3")
                img(:src="report.photo_3")
            a(:href="report.photo_4" target="_black" v-if="report.photo_4")
                img(:src="report.photo_4")
        .accept-wrapper(v-if="report.status != 'accepted' && canEdit")
            InputTextarea(
                v-model="comment"
                label="Комментарий к отчету"
                placeholder="Введите комментарий к отчету"
            )
            InputFile(name="files" label="Прикрепить файлы" :multiple="true" accept="image/*,.pdf" v-model="filelist")
            .btn-wrapper.center.mt-20
                button.btn.sm.mr-20.secondary(@click="AcceptReport") Принять отчет
                button.btn.sm.danger(@click="RefuseReport") Отклонить отчет

</template>

<script setup>
import InputTextarea from '~/components/Input/InputTextarea.vue'

const props = defineProps(['report', 'canEdit'])
const showModal = ref(false)
const emits = defineEmits(['update'])
const comment = ref('')
const filelist = ref([])

async function AcceptReport() {
    try {
        await $api.post(`/work/report/${props.report.id}/confirm/accept`, $api.createFormData({
            comment: comment.value,
            filelist: filelist.value
        }))
        $notice.success('Отчет принят')
        showModal.value = false
        emits('update')
    }
    catch (error) {
        $notice.handleError(error)
    }
}

async function RefuseReport() {
    try {
        await $api.post(`/work/report/${props.report.id}/confirm/refuse`, $api.createFormData({
            comment: comment.value,
            filelist: filelist.value
        }))
        $notice.success('Отчет отклонен')
        showModal.value = false
        emits('update')
    }
    catch (error) {
        $notice.handleError(error)
    }
}
</script>