<template lang="pug">
    .confirm-address
        .app-title-wrapper
            h1.app-title Необходимо подтвердить выход на объект
            .app-subtitle Если вы не подтвердите выход в течении <span class="secondary">20 часов</span> до начала смены, то произойдет отказ от работы
        .jobs-list 
            WorkShift(:order="acceptWork")
                button.btn.primary.sm.danger.mr-10(@click="$emit('refuse', acceptWork.work.id)") Отказаться от работы
                button.btn.primary.sm(@click="acceptWorkShiftAddress") Подтверждаю
</template>

<script setup>
const props = defineProps({
    acceptWork: {
        type: Object,
        default: {}
    }
})

defineEmits(['refuse'])

async function acceptWorkShiftAddress() {
    try {
        await $api.post(`/work/shift/${props.acceptWork.current_work_shift.id}/address/accept`)
        $notice.success('Вы подтвердили выход на объект')
    }
    catch (error) {
        $notice.handleError(error)
    }
}

</script>