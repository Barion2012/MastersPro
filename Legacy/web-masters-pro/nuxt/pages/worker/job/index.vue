<template lang="pug">
main 
    section.dashdoard.pd-header.bg-screen.screen
        WorkSearch(v-if="acceptWork.status === 'none'")
        WorkMatchDetail(v-else-if="acceptWork.status === 'matched'" :acceptWork="acceptWork" @accept="accept" @refuse="refuse")
        .container(v-if="acceptWork.status === 'pending'")
            WorkPendingDetail(v-if="!acceptWork.current_work_shift" :acceptWork="acceptWork" @refuse="refuse")
            WorkWaitingShift(v-else-if="!acceptWork?.current_work_shift?.is_active" :acceptWork="acceptWork" @refuse="refuse")
            WorkConfirmAddressDetail(v-else-if="!acceptWork.current_work_shift.confirm_address" :acceptWork="acceptWork" @refuse="refuse")
            WorkConfirmMeetingDetail(v-else-if="!acceptWork.current_work_shift.confirm_meeting" :acceptWork="acceptWork")
        .container(v-if="acceptWork.status === 'arrive'")
            WorkConfirmCustomerDetail(v-if="acceptWork.current_work_shift.confirm_address && acceptWork.current_work_shift.confirm_meeting" :acceptWork="acceptWork")
        .container(v-if="acceptWork.status === 'accepted'")
            WorkWaitingShift(v-if="!acceptWork?.current_work_shift?.is_active" :acceptWork="acceptWork" @refuse="refuse")
            WorkConfirmAddressDetail(v-else-if="!acceptWork.current_work_shift.confirm_address" :acceptWork="acceptWork" @refuse="refuse")
            WorkConfirmMeetingDetail(v-else-if="!acceptWork.current_work_shift.confirm_meeting" :acceptWork="acceptWork")
            WorkActiveDetail(v-else-if="acceptWork?.current_work_shift?.is_active" :acceptWork="acceptWork" )
    SupportWidget
</template>

<script setup>
definePageMeta({
    middleware: ['sanctum:auth', 'role:worker'],
});

const acceptWork = ref({
    status: 'none'
})
const user = useSanctumUser()
const channelName = ref('');

async function getAcceptWork() {
    try {
        acceptWork.value = await $api.get('/work/accept')
    }
    catch (error) {
        $notice.handleError(error)
    }
}

function listenNotifications() {
    channelName.value = `user-notification.${user.value.id}`
    $echo.private(channelName.value)
        .listen('UpdateAcceptWork', (e) => {
            getAcceptWork()
        })
}

async function refuse(workId) {
    try {
        await $api.post(`/work/${workId}/refuse`)
        $notice.success('Вы отказались от предложения работы')
    }
    catch (error) {
        $notice.handleError(error)
    }
}

async function accept(workId) {
    try {
        await $api.post(`/work/${workId}/accept`)
        $notice.success('Вы приняли предложение работы')
    }
    catch (error) {
        $notice.handleError(error)
    }
}

onMounted(() => {
    getAcceptWork()
    listenNotifications()
})

</script>