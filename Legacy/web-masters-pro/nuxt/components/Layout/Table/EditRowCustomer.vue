<template lang="pug">
    .row.customer.edit
        .col
            span {{ index }}
        .col 
            span {{ user.id }}
        .col 
            span {{ user.name }}
        .col 
            span {{ user.email }}
        .col 
            span {{ user.customer.balance }} р.
        .col 
            .tag.blue(v-if="user.customer.type === 'ip'") ИП
            .tag.purple(v-else-if="user.customer.type === 'ooo'") ООО
            .tag.yellow(v-else-if="user.customer.type === 'pao'") ПАО
            .tag.orange(v-else-if="user.customer.type === 'ao'") АО
        .col 
            BtnIcon(icon="/images/icons/btn-icon/show.png" @click.stop="showEditModal")
    Modal(v-model="showModal").user-modal
        span.modal-title Информация о заказчике
        .info-wrapper
            h4 Общая информация
            .info
                b Название:
                span {{ user.name }}
            .info 
                b Тип заказчика:
                div
                    span.tag.blue(v-if="user.customer.type === 'ip'") ИП
                    span.tag.purple(v-else-if="user.customer.type === 'ooo'") ООО
            .info 
                b Email:
                span {{ user.email }}
            .info 
                b Телефон:
                span {{ user.phone }}
            .info 
                b Баланс:
                span {{ user.customer.balance }} ₽
            div(v-if="user.customer.type === 'ip'")
                .info 
                    b Фамилия:
                    span {{ user.customer.info['surname'] }}
                .info 
                    b Имя:
                    span {{ user.customer.info['name'] }}
                .info 
                    b Отчество:
                    span {{ user.customer.info['patronymic'] }}
                .info 
                    b Дата рождения:
                    span {{ user.customer.info['birthDate'] }}
                .info 
                    b Место рождения:
                    span {{ user.customer.info['birthPlace'] }}
                .info 
                    b Юридический адрес:
                    span {{ user.customer.info['regAddress'] }}
                .info 
                    b ИНН:
                    span {{ user.customer.info['inn'] }}
                .info 
                    b ОГРНИП:
                    span {{ user.customer.info['ogrnip'] }}
                .info 
                    b Серия документа:
                    span {{ user.customer.info['seria'] }}
                .info 
                    b Номер документа:
                    span {{ user.customer.info['number'] }}
                .info 
                    b Наименование органа, выдавшего документ:
                    span {{ user.customer.info['issuer'] }}
                .info 
                    b Дата выдачи паспорта:
                    span {{ user.customer.info['issueDate'] }}
                .info 
                    b Код подразделения:
                    span {{ user.customer.info['issuerCode'] }}
            div(v-else-if="user.customer.type === 'ooo'")
                .info
                    b ОГРН:
                    span {{ user.customer.info['ogrn'] }}
                .info
                    b ИНН:
                    span {{ user.customer.info['inn'] }}
                .info
                    b Код причины постановки на учет:
                    span {{ user.customer.info['kpp'] }}
                .info
                    b Наименование организации:
                    span {{ user.customer.info['orgName'] }}
                .info 
                    b Юридический адрес:
                    span {{ user.customer.info['legalAddress'] }}
            //.info
                b Форма налогообложения:
                span(v-if="user.customer.taxation_system === 'osn'") Общая система налогообложения (ОСН)
                span(v-else-if="user.customer.taxation_system === 'usn'") Упрощенная система налогообложения (УСН)
                span(v-else-if="user.customer.taxation_system === 'envd'") Единый налог на вмененный доход (ЕНВД)
                span(v-else-if="user.customer.taxation_system === 'patent'") Патентная система налогообложения (ПСН)
        .info-wrapper
            h4 Реквизиты банковского счета 
            .info 
                b Номер расчетного счета:
                span {{ user.customer.info['accountNumber'] }}
            .info
                b Номер корреспондентского счета:
                span {{ user.customer.info['bankCorAccount'] }}
            .info
                b БИК:
                span {{ user.customer.info['bankBIC'] }}
            .info
                b Наименование банка:
                span {{ user.customer.info['bankName'] }}
        .info-wrapper
            h4 Документы
            .info(v-if="user?.files?.passport_scan")
                b Фото паспорта (основной разворот и прописка):
                a(:href="`/api/user/${user.id}/file/${user?.files?.passport_scan}`" target="_blank").link Скачать
            .info(v-if="user?.files?.passport_selfi")
                b Фото с паспортом:
                a(:href="`/api/user/${user.id}/file/${user?.files?.passport_selfi}`" target="_blank").link Скачать
            .info(v-if="user?.files?.certificate")
                b Свидетельство о постановке на учет ИП:
                a(:href="`/api/user/${user.id}/file/${user?.files?.certificate}`" target="_blank").link Скачать
        .btn-wrapper.mt-20.center
            button.btn.primary.sm.mr-20(@click="editCustomer") Сохранить
            button.btn.danger.sm.mr-20(@click="showModal = false") Отмена
</template>

<script setup>
const showModal = ref(false)
const emits = defineEmits(['update'])
const props = defineProps({
    user: {
        type: Object,
        required: true
    },
    index: {
        type: Number,
        required: true
    }
})
const balance = ref(0)

function showEditModal() {
    showModal.value = true
    balance.value = props.user.customer.balance
}

async function editCustomer() {
    try {
        await $api.post(`/user/${props.user.id}/customer/edit`, {
            balance: balance.value
        })
        showModal.value = false
        emits('update')
    } catch (error) {
        $notice.handleError(error)
    }
}
</script>