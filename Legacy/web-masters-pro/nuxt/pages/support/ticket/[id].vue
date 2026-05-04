<template lang="pug">
section.support-page.pd-header.bg-screen.screen
    .container(v-if="ticket.id")
        Pagetitle(:title="ticket.title" :goback="true")
        .content-card.report-data.mb-20
            .table.ticket-info 
                .row.head 
                    .col 
                        span ID
                    .col 
                        span Автор тикета
                    .col 
                        span Дата создани
                    .col 
                        span Мастер
                    .col 
                        span Отчёт 
                .row 
                    .col 
                        span {{ ticket.id }}
                    .col 
                        UserPreview(:name="ticket.user.name" :role="ticket.user.role" :phone="ticket.user.phone")
                    .col 
                        span {{ ticket.created }}
                    .col 
                        UserPreview(:name="ticket.report.user.name" :role="ticket.report.user.role" :phone="ticket.report.user.phone" v-if="ticket?.report?.user")
                        span(v-else) - 
                    .col 
                        .photos.mt-20.mb-20(v-if="ticket?.report?.files?.length > 0")
                            a(v-for="file in ticket.report.files" :href="file.permanent_url" target="_black")
                                img(:src="file.permanent_url")
                        span(v-else) - 
            .info
                b Описание тикета:
                .text(v-html="ticket.description")
            .filelist 
                a.file(v-for="file in ticket.files" :href="file.permanent_url" target="_black") {{ file.name }}
            .btn-wrapper.right.mt-20
                button.btn.danger.sm(@click="closeTicket") Закрыть тикет

            //.content-card.report-data.mb-20
                .info 
                    b Номер тикета:
                    span {{ ticket.id }}
                .info
                    b Автор тикета:
                    span {{ ticket.user.name }}
                .info
                    b Описание тикета:
                    .text(v-html="ticket.description")
                .info 
                    b Дата создания:
                    span {{ ticket.created }}
                .info
                    b Мастер:
                    span {{ ticket?.report?.user?.name || 'Не назначен' }} (ID: {{ ticket?.report?.user?.id || '' }})
                .photos.mt-20.mb-20(v-if="ticket.files")
                    a(v-for="file in ticket.files" :href="file.permanent_url" target="_black")
                        img(:src="file.permanent_url")
                .btn-wrapper.right.mt-20
                    button.btn.danger.sm(@click="closeTicket") Закрыть тикет
        .message-history
            .message.content-card(v-for="item in ticket.messages" :class="{right: item.user.id == user.id}")
                .author {{ item.user.name }}
                .text(v-html="item.message")
                .date {{ item.created }}
                .filelist 
                    a.file(v-for="file in item.files" :href="file.permanent_url" target="_black") {{ file.name }}
        .content-card.mt-20.new-message(:key="reRender")
            TextEditor(label="Новое сообщение" v-model="newMessage.message")
            InputFile(name="files" label="Прикрепить файлы" :multiple="true" accept="image/*,.pdf" v-model="newMessage.filelist")
            .btn-wrapper.right.mt-20
                button.btn.primary.sm(@click="sendTicketMessage") Отправить
</template>

<script setup>
import UserPreview from '~/components/UI/UserPreview.vue'

const ticket = ref({})
const route = useRoute()
const user = useSanctumUser()
const newMessage = ref({
    message: '',
    filelist: []
})
const filelist = ref([])
const reRender = ref(0)

async function getSupportTicketById() {
    try {
        ticket.value = await $api.get(`/support/ticket/${route.params.id}/get`)
    }
    catch (error) {
        $notice.handleError(error)
    }
}

async function sendTicketMessage() {
    try {
        await $api.post(`/support/ticket/${route.params.id}/message/send`, $api.createFormData(newMessage.value))
        newMessage.value.message = ''
        newMessage.value.filelist = []
        reRender.value++
    }
    catch (error) {
        $notice.handleError(error)
    }
}

async function closeTicket() {
    try {
        await $api.post(`/support/ticket/${route.params.id}/status/closed`)
        $notice.success('Тикет успешно закрыт')
        navigateTo('/support')
    }
    catch (error) {
        $notice.handleError(error)
    }
}

function listenNotifications() {
    $echo.private(`ticket-notification.${route.params.id}`)
        .listen('SendTicketMessage', (e) => {
            getSupportTicketById()
        })
}

onMounted(() => {
    listenNotifications()
})

onBeforeUnmount(() => {
    $echo.leave(`ticket-notification.${route.params.id}`)
})

getSupportTicketById()
</script>