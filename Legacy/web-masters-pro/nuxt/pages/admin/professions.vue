<template lang="pug">
main 
    section.professions.pd-header.pd-bottom.bg-screen.screen
        .container 
            Pagetitle(title="Профессии" :goback="true")
            .content-card
                .table.professions
                    .row.head
                        .col
                            span Профессия
                        .col
                            span Доступные уровни
                        .col
                            span Оплата за смену
                        .col
                    RowProfession(
                        v-for="(profession, index) in professions" 
                        :key="index" 
                        :profession="profession" 
                        @show-modal="showSaveProfessionLevelModalHandler"
                        @select-profession="selectedProfession = profession; showSaveProfessionModal = true"
                        @delete-profession="showWarningDeleteProfessionModal"
                        @delete-level="showWarningDeleteProfessionLevelModal"
                    )
                Paginator(:lastPage="lastPage" :currentPage="currentPage" @change-page="")
                .btn-wrapper.right.mt-10
                    button.btn.primary.sm(@click="showSaveProfessionModal = true") Добавить профессию
            SaveProfessionModal(v-model="showSaveProfessionModal" @save="loadProfessionListDetaited(currentPage)" :profession="selectedProfession")
            SaveProfessionLevelModal(v-model="showSaveProfessionLevelModal" :level="selectedLevel" @save="loadProfessionListDetaited(currentPage)")

</template>

<script setup>
import { useModal } from 'vue-final-modal'
import WarningModal from '~/components/Modal/WarningModal.vue'

definePageMeta({
    middleware: ['sanctum:auth', 'role:admin'],
});

const professions = ref([])
const currentPage = ref(1)
const lastPage = ref(1)
const showSaveProfessionModal = ref(false)
const showSaveProfessionLevelModal = ref(false)
const selectedLevel = ref({})
const selectedProfession = ref({})

const professionDeleteModal = useModal({
    component: WarningModal,
    attrs: {
        text: 'Вы уверены, что хотите удалить профессию?',
        onCancel() {
            professionDeleteModal.close()
        },
        onConfirm() {
            deleteProfession(selectedProfession.value.id)
            professionDeleteModal.close()
        }
    },
})

const professionLevelDeleteModal = useModal({
    component: WarningModal,
    attrs: {
        text: 'Вы уверены, что хотите удалить уровень профессии?',
        onCancel() {
            professionLevelDeleteModal.close()
        },
        onConfirm() {
            deleteProfessionLevel(selectedLevel.value.id)
            professionLevelDeleteModal.close()
        }
    },
})

async function loadProfessionListDetaited(pageIndex) {
    try {
        const response = await $api.get(`/profession/list/detailed?list=${pageIndex}`)
        professions.value = response.data
        currentPage.value = response.current_page
        lastPage.value = response.last_page
    }
    catch (error) {
        $notice.handleError(error)
    }
}

async function deleteProfession(professionId) {
    try {
        await $api.post(`/profession/delete`, {
            id: professionId
        })
        $notice.success('Профессия успешно удалена')
        loadProfessionListDetaited(currentPage.value)
    }
    catch (error) {
        $notice.handleError(error)
    }
}

async function deleteProfessionLevel(levelId) {
    try {
        await $api.post(`/profession/level/delete`, {
            id: levelId
        })
        $notice.success('Уровень профессии успешно удален')
        loadProfessionListDetaited(currentPage.value)
    }
    catch (error) {
        $notice.handleError(error)
    }
}

function showSaveProfessionLevelModalHandler(level) {
    selectedLevel.value = level
    showSaveProfessionLevelModal.value = true
}

function showWarningDeleteProfessionModal(profession) {
    selectedProfession.value = profession
    professionDeleteModal.open()
}

function showWarningDeleteProfessionLevelModal(level) {
    selectedLevel.value = level
    professionLevelDeleteModal.open()
}

loadProfessionListDetaited(1)
</script>