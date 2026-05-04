<template lang="pug">
section.support-page.pd-header.bg-screen.screen
    .container
        Pagetitle(:title="`Поддержка`" :goback="true")
        .content-card.mt-10(v-if="supportTickets.length > 0")
            .table.support
                .row.head
                    .col
                        span ID
                    .col 
                        span Автор
                    .col
                        span Тема
                    .col
                        span Дата создания
                    .col
                        span Статус
                    .col
                        span Подробнее
                RowTicket(v-for="item in supportTickets" :ticket="item")
            Paginator(:lastPage="lastPage" :currentPage="currentPage" @change-page="getSupportTicketsList")
        .btn-wrapper.right.mt-20(v-if="checkPermission('Ticket.create')")
            NuxtLink.btn.sm.primary(@click="showModal = true") Создать обращение
    Modal(v-model="showModal").ticket-modal 
        span.modal-title Создать обращение
        Input(label="Тема обращения" v-model="newTicket.title")
        TextEditor(label="Описание" v-model="newTicket.description")
        InputFile(name="files" label="Прикрепить файлы" :multiple="true" accept="image/*,.pdf" v-model="newTicket.filelist")
        .btn-wrapper.mt-20.center
            button.btn.sm.primary.mr-20(@click="createTicket") Отправить
            button.btn.sm.danger(@click="showModal = false") Отмена

</template>

<script setup>
const supportTickets = ref([])
const currentPage = ref(0)
const lastPage = ref(0)
const showModal = ref(false)
const newTicket = ref({})
const user = useSanctumUser()
const { checkPermission } = $api;

async function getSupportTicketsList(pageIndex) {
    try {
        let response = {}
        if ($api.checkPermission('Ticket.handle'))
            response = await $api.get(`/support/ticket/list?page=${pageIndex}`)
        else 
            response = await $api.get(`/user/tickets?page=${pageIndex}`)

        supportTickets.value = response.data
        currentPage.value = response.current_page
        lastPage.value = response.last_page
    } catch (error) {
        $notice.handleError(error)
    }
}

async function createTicket() {
    try {
        await $api.post(`/support/ticket/create`, $api.createFormData(newTicket.value))
        showModal.value = false
        getSupportTicketsList(currentPage.value)
        $notice.success('Обращение успешно создано')
    }
    catch (error) {
        $notice.handleError(error)
    }
}

getSupportTicketsList(1)
</script>