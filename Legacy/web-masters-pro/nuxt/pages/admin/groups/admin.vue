<template lang="pug">
main 
    section.users-group.pd-header.bg-screen.screen
        .container 
            Pagetitle(title="Администраторы" :goback="true")
            .content-card
                .table.admin
                    .row.head
                        .col
                            span №
                        .col
                            span ID
                        .col
                            span ФИО
                        .col
                            span Роль
                        .col
                            span Телефон
                        .col
                            span E-mail
                        .col
                            span Подробнее
                    .row(v-for="(item, index) in users")
                        .col
                            span {{ item.index }}
                        .col
                            span {{ item.id }}
                        .col
                            span {{ item.name }}
                        .col
                            RoleTag(:role="item.role")
                        .col
                            span {{ item.phone }}
                        .col
                            span {{ item.email }}
                        .col
                            BtnIcon(icon="/images/icons/btn-icon/show.png" @click="editAdmin(index)").mr-10
                            BtnIcon(icon="/images/icons/btn-icon/bin.png" @click="showWarningModal(index)").danger
                Paginator(:lastPage="lastPage" :currentPage="currentPage" @change-page="loadUsersList")
                .btn-wrapper.right.mt-10
                    button.btn.primary.sm(@click="createAdmin") Создать администратора
                EditAdminModal(v-model="showEditAdminModal" :user="selectedUser" @save="loadUsersList(currentPage)")
</template>

<script setup>
import { useModal } from 'vue-final-modal'
import WarningModal from '~/components/Modal/WarningModal.vue'
import EditAdminModal from '~/components/Modal/EditAdminModal.vue'

definePageMeta({
    middleware: ['sanctum:auth', 'role:admin'],
});

const users = ref([])
const currentPage = ref(0)
const lastPage = ref(0)
const selectedUser = ref({
    name: '',
    phone: '',
    email: ''
})
const showEditAdminModal = ref(false)

const adminDeleteModal = useModal({
    component: WarningModal,
    attrs: {
        text: 'Вы уверены, что хотите удалить администратора?',
        onCancel() {
            adminDeleteModal.close()
        },
        onConfirm() {
            deleteUser(selectedUser.value.id)
            adminDeleteModal.close()
        }
    },
})

async function loadUsersList(pageIndex) {
    try {
        const response = await $api.post(`/user/list?page=${pageIndex}`, {
            filter: {
                roles: ['admin', 'moderator', 'manager']
            }
        })
        
        users.value = response.data
        currentPage.value = response.current_page
        lastPage.value = response.last_page
    }
    catch (error) {
        $notice.handleError(error)
    }
}

function editAdmin(userIndex) {
    selectedUser.value = users.value[userIndex]
    showEditAdminModal.value = true
}

function createAdmin() {
    selectedUser.value = {
        name: '',
        phone: '',
        email: '',
        role: 'admin'
    }
    showEditAdminModal.value = true
}

async function deleteUser(userId) {
    try {
        await $api.delete(`/user/${userId}/delete`)
        $notice.success('Пользователь успешно удален')
        loadUsersList(currentPage.value)
    }
    catch (error) {
        $notice.handleError(error)
    }
}

function showWarningModal(userIndex) {
    selectedUser.value = users.value[userIndex]
    adminDeleteModal.open()
}

loadUsersList(1)
</script>