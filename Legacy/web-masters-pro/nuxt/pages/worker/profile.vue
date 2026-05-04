<template lang="pug">
    section.profile.pd-header.bg-screen.screen
        .container 
            .card.user-modal
                span.card-title Профиль
                .info-wrapper
                    h4 Общая информация
                    .info
                        b ФИО:
                        span {{ user.name }}
                    .info 
                        b Гражданство:
                        div 
                            span.tag.green(v-if="user.worker.citizenship === 'citizen'") Гражданин РФ
                            span.tag.blue(v-else-if="user.worker.citizenship === 'eaeu'") ЕАЭС
                            span.tag(v-else) Прочее 
                .info-wrapper
                    h4 Профессии
                    .profession(v-for="(profession, index) in user.worker.profession")
                        .info 
                            b Профессия:
                            span {{ appStore.professions.find(item => item.value === profession.profession_id).text }}
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
                        button.btn.primary.sm(@click="editPaymentRequisites") Изменить реквизиты
</template>

<script setup>
import { useAppStore } from '~/store/app'

const user = useSanctumUser()
const appStore = useAppStore()
const paymentRequisites = ref({
    account_number: '',
    bank_bik: '',
    bank_cor_account: '',
    bank_name: ''
})

async function editPaymentRequisites() {
    try {
        await $api.post(`/user/edit/payment-requisites`, paymentRequisites.value)
        $notice.success('Платежные данные обновлены')
    }
    catch (error) {
        $notice.handleError(error)
    }
}

onMounted(() => {
    paymentRequisites.value.account_number = user.value.worker.account_number
    paymentRequisites.value.bank_bik = user.value.worker.bank_bic
    paymentRequisites.value.bank_cor_account = user.value.worker.bank_cor_account
    paymentRequisites.value.bank_name = user.value.worker.bank_name
})
</script>