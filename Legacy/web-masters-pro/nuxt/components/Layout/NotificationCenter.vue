<template lang="pug">
.notifications(v-click-outside="hideNotificationList")
    .icon(@click="active = !active" :class="{ active: active }")
        .counter {{ notifications.length }}
        img(src="/images/icons/notification.png")
    .notification-list(:class="{ active: active }" v-if="notifications.length")
        NuxtLink.notification-item(v-for="item in notifications" :key="item.id" :to="item.url")
            .date {{ item.date }}
            .title {{ item.title }}
            .text {{ item.message }}
</template>

<script setup>
const user = useSanctumUser()
const { refreshIdentity } = useSanctumAuth()
const active = ref(false)
const notifications = ref([])

async function hideNotificationList() {
    if (!active.value) return
    active.value = false

    try {
        await $api.delete('/user/notifications/')
        notifications.value = []
    }
    catch (error) {
        $notice.handleError(error)
    }
}

function listenNotifications() {
    $echo.private(`user-notification.${user.value.id}`)
        .listen('UserNotification', (e) => {
            notifications.value.push({
                title: e.title,
                message: e.message,
                url: e.url,
                date: e.date,
            })
        })
        .listen('UpdateBalance', (e) => {
            refreshIdentity()
        })
}

async function getNotifications() {
    try {
        notifications.value = await $api.get('/user/notifications')
    }
    catch (error) {
        $notice.handleError(error)
    }
}

onMounted(() => {
    if (user.value) {
        listenNotifications()
        getNotifications()
    }
})
onBeforeUnmount(() => {
    if (user.value) {
        $echo.leave(`user-notification.${user.value.id}`)
    }
})
</script>