<template lang="pug">
    main 
        section.users-group.pd-header.bg-screen.screen
            .container 
                Pagetitle(title="Мастера" :goback="true")
                .content-card.mt-10
                    .table.workers
                        .row.edit.head
                            .col 
                                span №
                            .col 
                                span ID
                            .col 
                                span ФИО
                            .col 
                                span Профессия
                            .col 
                                span Гражданство 
                            .col 
                                span Подробнее
                        EditRowWorker(v-for="(item, index) in workers" :worker="item" :index="index + 1" @update="loadWorkersList(currentPage)")
                    Paginator(:lastPage="lastPage" :currentPage="currentPage" @change-page="loadWorkersList")
</template>

<script setup>
definePageMeta({
    middleware: ['sanctum:auth', 'role:admin'],
});

const workers = ref([])
const currentPage = ref(0)
const lastPage = ref(0)

async function loadWorkersList(pageIndex) {
    try {
        const response = await $api.get(`/user/list/worker?page=${pageIndex}`)
        workers.value = response.data
        currentPage.value = response.current_page
        lastPage.value = response.last_page
    }
    catch (error) {
        $notice.handleError(error)
    }
}

loadWorkersList(1)
</script>