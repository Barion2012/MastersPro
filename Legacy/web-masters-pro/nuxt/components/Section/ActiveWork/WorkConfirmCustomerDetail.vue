<template lang="pug">
.active-work 
    .app-title-wrapper
        h1.app-title Ждем потверждения от заказчика
        .app-subtitle После подтверждения вы сможете приступить к работе
    .works-list
        WorkShift(:order="acceptWork")
            button.btn.primary.sm.mr-10(@click="showModalMeet = true" v-if="acceptWork.current_work_shift.not_met") Меня не встретили
    Modal(v-model="showModalMeet").user-modal
        span.modal-title Вы уверены, что вас не встретили? 
        .btn-wrapper.center
            button.btn.danger.sm.mr-10(@click="workerNotMet") Меня не встретили
            button.btn.primary.sm(@click="showModalMeet = false") Отмена
</template>

<script setup>
const props = defineProps({
    acceptWork: {
        type: Object,
        default: {}
    }
})
const showModalMeet = ref(false)

async function workerNotMet() {
    try {
        await $api.post(`/work/${props.acceptWork.work.id}/not-met`)
        $notice.success('Вы сообщили, что вас не встретили')

        setTimeout(() => {
            navigateTo('/worker/dashboard')
        }, 400)
        
    }
    catch (error) {
        $notice.handleError(error)
    }
}
</script>