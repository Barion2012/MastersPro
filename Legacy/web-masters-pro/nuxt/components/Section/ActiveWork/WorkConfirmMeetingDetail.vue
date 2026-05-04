<template lang="pug">
    .confirm-meeting
        .app-title-wrapper
            h1.app-title Сообщите, что вы достигли адреса встречи
            .app-subtitle В случае опоздания более чем на <span class="secondary">15 минут</span>, то произойдет отказ от работы
        .works-list 
            WorkShift(:order="acceptWork")
                button.btn.primary.sm(@click="acceptWorkShiftMeeting") Я на месте
</template>

<script setup> 
const props = defineProps({
    acceptWork: {
        type: Object,
        default: {}
    }
})

async function acceptWorkShiftMeeting() {
    try {
        await $api.post(`/work/shift/${props.acceptWork.current_work_shift.id}/meeting/accept`)
        $notice.success('Вы подтвердили достижение адреса встречи')
    }
    catch (error) {
        $notice.handleError(error)
    }
}
</script>