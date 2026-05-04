<template lang="pug">
main 
    section.dashdoard.pd-header.bg-screen.screen
        .container
            Pagetitle(:title="`Предложения работы`")
            .orders(v-if="orders.length > 0")
                WorkItem(v-for="order in orders" :order="order")
                    .btn-wrapper.right
                        button.btn.primary.sm.danger.mr-10(@click="refuseWorkAddress(order.workId)") Отказаться от работы
                        button.btn.sm.primary(@click="acceptWorkOrder(order.workId)") Принять работу
            .orders(v-else)
                h2 Предложения от работодателей появятся здесь в ближайшее время
            .btn-wrapper.right.mb-10.mt-auto
                BtnLogout
        SupportWidget
</template>

<script setup>
definePageMeta({
    middleware: ['sanctum:auth', 'role:worker'],
});

const user = useSanctumUser()
const orders = ref([])
const channelName = ref('');

function listenNotifications() {
    channelName.value = `user-notification.${user.value.id}`
    $echo.private(channelName.value)
        .listen('WorkAvailable', (e) => {
            const isDuplicate = orders.value.some(obj => obj.workId === e.workId);
            console.log(e)

            if (!isDuplicate)
                orders.value.push(e)
        })
    $echo.channel(`search-channel`)
        .listen('WorkUnavailable', (e) => {
            const index = orders.value.findIndex(item => item.workId === e.workId);
            if (index !== -1) 
                orders.value.splice(index, 1);
        })
}

async function acceptWorkOrder(workId) {
    try {
        await $api.post(`/work/${workId}/accept`)
        navigateTo('/worker/job/active')
    }
    catch (error) {
        $notice.handleError(error)
    }
}

async function checkAcceptWork() {
    try {
        const {workId} = await $api.get(`/work/check/accept`)
        if (workId)
            navigateTo('/worker/job/active')
    }
    catch (error) {
        $notice.handleError(error)
    }
}

async function refuseWorkAddress(workId) {
    try {
        await $api.post(`/work/${workId}/refsuse`)
        const index = orders.value.findIndex(item => item.workId === workId);
        if (index !== -1) 
            orders.value.splice(index, 1);
    }
    catch (error) {
        $notice.handleError(error)
    }
}

onMounted(() => {
    checkAcceptWork()
    listenNotifications()
})

onBeforeUnmount(() => {
    $echo.leave(channelName.value);
})
</script>