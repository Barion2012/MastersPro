<template lang="pug">
    .row.worker.edit
        .col
            span {{ index }}
        .col
            span {{ worker.id }}
        .col 
            span {{ worker.name }}
        .col 
            div(v-for="profession in worker.profession") {{ appStore.professions.find(item => item.value === profession.profession_id)?.text }} - {{ appStore.professionsLevels[profession.profession_id].find(item => item.value === profession.profession_level_id)?.text }}
        .col 
            span.tag.green(v-if="worker.citizenship === 'citizen'") Гражданин РФ
            span.tag.blue(v-else-if="worker.citizenship === 'eaeu'") ЕАЭС
            span.tag(v-else) Прочее
        .col 
            BtnIcon(icon="/images/icons/btn-icon/show.png" @click.stop="showEditModal")
    Modal(v-model="showModal").user-modal
        span.modal-title Информация о мастере
        .info-wrapper
            h4 Общая информация
            .info
                b ФИО:
                span {{ worker.name }}
            .info 
                b Гражданство:
                div 
                    span.tag.green(v-if="worker.citizenship === 'citizen'") Гражданин РФ
                    span.tag.blue(v-else-if="worker.citizenship === 'eaeu'") ЕАЭС
                    span.tag(v-else) Прочее 
        .info-wrapper
            h4 Профессии
            .profession(v-for="(profession, index) in workerProfessions")
                .info 
                    b Профессия:
                    span {{ appStore.professions.find(item => item.value === profession.profession_id).text }}
                //.info 
                    b Уровень профессии: 
                    Select(v-model="workerProfessions[index].profession_level_id" :options="appStore.professionsLevels[profession.profession_id]")
        .info-wrapper
            h4 Личные данные
            .info
                b ИНН:
                Input(v-model="personalData.inn" mask="############")
            .info
                b СНИЛС:
                Input(v-model="personalData.snils" mask="###-###-### ##")
            .info
                b Адрес:
                Input(v-model="personalData.location")
            .btn-wrapper.mt-20.center 
                button.btn.primary.sm(@click="editWorkerPersonalData") Изменить данные
        .info-wrapper
            h4 Паспортные данные
            .info
                b Серия:
                span {{ worker.worker.passport_series }}
            .info
                b Номер:
                span {{ worker.worker.passport_number }}
            .info
                b Кем выдан:
                span {{ worker.worker.passport_issued_by }}
        .info-wrapper
            h4 Файлы
            .files
                .info(v-if="worker.files.passport_scan")
                    b Паспорт (основной разворот)
                    a(:href="`/api/order/${route.params.id}/worker/${worker.worker_id}/file/${worker.files.passport_scan}/get`" target="_blank").link Скачать
                .info(v-if="worker.files.passport_reg_scan")
                    b Паспорт (прописка)
                    a(:href="`/api/order/${route.params.id}/worker/${worker.worker_id}/file/${worker.files.passport_reg_scan}/get`" target="_blank").link Скачать
                .info(v-if="worker.files.snils")
                    b СНИЛС 
                    a(:href="`/api/order/${route.params.id}/worker/${worker.worker_id}/file/${worker.files.snils}/get`" target="_blank").link Скачать
                .info(v-if="worker.files.migration_card")
                    b Миграционная карта 
                    a(:href="`/api/order/${route.params.id}/worker/${worker.worker_id}/file/${worker.files.migration_card}/get`" target="_blank").link Скачать
                .info(v-if="worker.files.patent")
                    b Патент
                    a(:href="`/api/order/${route.params.id}/worker/${worker.worker_id}/file/${worker.files.patent}/get`" target="_blank").link Скачать
                .info(v-if="worker.files.dms")
                    b ДМС
                    a(:href="`/api/order/${route.params.id}/worker/${worker.worker_id}/file/${worker.files.dms}/get`" target="_blank").link Скачать
        .info-wrapper
            h4 Реквизиты
            .info
                b Название банка:
                Input(v-model="paymentRequisites.bank_name")
            .info
                b Номер расчетного счета:
                Input(v-model="paymentRequisites.account_number" :mask="'#########################'")
            .info
                b Номер кореспондентского счета:
                Input(v-model="paymentRequisites.bank_cor_account" :mask="'####################'")
            .info
                b БИК банка:
                Input(v-model="paymentRequisites.bank_bik" :mask="'#########'")
            .btn-wrapper.mt-20.center 
                button.btn.primary.sm(@click="editWorkerPaymentRequisites") Изменить реквизиты
        .info-wrapper
        //.btn-wrapper.mt-20.center
            button.btn.primary.sm.mr-20(@click="editWorkerProfessions") Сохранить
            button.btn.danger.sm.mr-20(@click="showModal = false") Отмена


</template>

<script setup>
import { useAppStore } from '~/store/app'

const props = defineProps(['index', 'worker'])
const showModal = ref(false)
const route = useRoute()
const appStore = useAppStore()
const workerProfessions = ref([])
const paymentRequisites = ref({
    account_number: '',
    bank_bik: '',
    bank_cor_account: '',
    bank_name: ''
})
const personalData = ref({
    inn: '',
    snils: '',
    location: ''
})
const emits = defineEmits(['update'])

function showEditModal() {
    workerProfessions.value = JSON.parse(JSON.stringify(props.worker.profession))

    paymentRequisites.value.account_number = props.worker.worker.account_number
    paymentRequisites.value.bank_bik = props.worker.worker.bank_bic
    paymentRequisites.value.bank_cor_account = props.worker.worker.bank_cor_account
    paymentRequisites.value.bank_name = props.worker.worker.bank_name

    personalData.value.inn = props.worker.worker.inn
    personalData.value.snils = props.worker.worker.snils
    personalData.value.location = props.worker.worker.location

    showModal.value = true
}

async function editWorkerProfessions() {
    try {
        await $api.post(`/user/worker/${props.worker.worker_id}/edit/profession`, {
            professions: workerProfessions.value
        })
        showModal.value = false
        emits('update')
    }
    catch (error) {
        $notice.handleError(error)
    }
}

async function editWorkerPaymentRequisites() {
    try {
        await $api.post(`/user/worker/${props.worker.worker_id}/edit/payment-requisites`, paymentRequisites.value)
        $notice.success('Платежные данные обновлены')
    }
    catch (error) {
        $notice.handleError(error)
    }
}

async function editWorkerPersonalData() {
    try {
        await $api.post(`/user/worker/${props.worker.worker_id}/edit/personal-data`, personalData.value)
        $notice.success('Личные данные обновлены')
    }
    catch (error) {
        $notice.handleError(error)
    }
}
</script>

<style lang="scss" scoped>
.info {
    .input-wrapper {
        margin-bottom: 0;
        label {
            display: none;
        }
    }
}
</style>