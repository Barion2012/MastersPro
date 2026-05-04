<template lang="pug">
section.register.pd-header.pd-bottom.bg-screen.screen
    .container 
        form.register 
            TabViewer(:height="tabViewerHeight")
                Tab(name="phone" :activeTab="activeTab" v-model="tabViewerHeight")
                    TabSwitcher(:tabs="tabsCustomer" v-model="customer.type")
                    SendCodeForm(
                        v-model:phone="customer.phone" 
                        v-model:code="customer.code" 
                        @submit="confirmPhone" 
                        @codeSended="updateTabViewer(132)" 
                        submit-text="Подтвердить номер"
                        :errorsInputCode="errors.code"
                        v-model:agree1="customer.agree1"
                        v-model:agree2="customer.agree2"
                        v-model:agree3="customer.agree3"
                        mode="register"
                    )
                Tab(name="ip" :activeTab="activeTab" v-model="tabViewerHeight")
                    Input(label="Фамилия" placeholder="Иванов" v-model="customer.surname" :errors="errors.surname")
                    Input(label="Имя" placeholder="Иван" v-model="customer.name" :errors="errors.name")
                    Input(label="Отчество" placeholder="Иванович" v-model="customer.patronymic" :errors="errors.patronymic")
                    Input(label="Email" placeholder="example@mail.com" v-model="customer.email" :errors="errors.email")
                    Date(label="Дата рождения" placeholder="01.01.2001" v-model="customer.birthDate" :errors="errors.birthDate" )
                    InputImage(label="Фото паспорта (основной разворот и прописка)" v-model="customer.passport_scan" name="passport" :errors="errors.passport_scan")
                    InputImage(label="Ваше фото с паспортом (должны быть видны данные паспорта)" v-model="customer.passport_selfie" name="selfie_citizen" :errors="errors.passport_selfie")
                    Input(label="Серия" placeholder="Серия" v-model="customer.seria" :errors="errors.seria" :mask="'####'")
                    Input(label="Номер" placeholder="Номер" v-model="customer.number" :errors="errors.number" :mask="'######'")
                    Input(label="Кем выдан" placeholder="Кем выдан" v-model="customer.issuer" :errors="errors.issuer")
                    Date(label="Дата выдачи паспорта" placeholder="01.01.2001" v-model="customer.issueDate" :errors="errors.issueDate") 
                    Input(label="Код подразделения" placeholder="Код подразделения" v-model="customer.issuerCode" :errors="errors.issuerCode" :mask="'###-###'")
                    Input(label="ИНН" placeholder="ИНН" v-model="customer.inn" type="text" :errors="errors.inn" name="ip-inn" :mask="'############'")
                    Input(label="Р./сч." placeholder="Номер расчетного счета" v-model="customer.accountNumber" :errors="errors.accountNumber" :mask="'#########################'")
                    Input(label="БИК" placeholder="БИК" v-model="customer.bankBIC" :errors="errors.bankBIC" :mask="'#########'")
                    Input(label="К./сч." placeholder="Номер корреспондентского счёта" v-model="customer.bankCorAccount" :errors="errors.bankCorAccount" :mask="'####################'")
                    Input(label="Наименование банка" placeholder="Наименование банка" v-model="customer.bankName" :errors="errors.bankName")
                    Input(label="ОГРНИП" placeholder="ОГРНИП" v-model="customer.ogrnip" type="text" :errors="errors.ogrnip" :mask="'3##############'")
                    Input(label="Юр. Адрес" placeholder="Адрес" v-model="customer.postAddress" :errors="errors.postAddress")
                    //Select(label="Форма налогооблажения" v-model="customer.taxation_system" :options="taxationSystems" placeholder="Выберите систему налогообложения" :errors="errors.taxation_system")
                    //InputImage(label="Свидетельство о постановке на учет ИП" v-model="customer.certificate " name="certificate" :errors="errors.certificate")
                    .btn-wrapper.center.mt-10 
                        button.btn.primary(@click.prevent="registerCustomer") Отправить
                Tab(name="ooo" :activeTab="activeTab" v-model="tabViewerHeight")
                    Input(label="Наименование организации" placeholder="OOO Иванов Иван Иванович" v-model="customer.orgName" :errors="errors.orgName")
                    Input(label="Email" placeholder="example@mail.com" v-model="customer.email" :errors="errors.email")
                    Input(label="ОГРН" placeholder="ОГРН" v-model="customer.ogrn" type="text" :errors="errors.ogrn" :mask="'#############'")
                    Input(label="ИНН" placeholder="ИНН" v-model="customer.inn" type="text" :errors="errors.inn" name="ooo-inn" :mask="'############'")
                    Input(label="КПП" placeholder="КПП" v-model="customer.kpp" type="text" :errors="errors.kpp" :mask="'#########'")
                    Input(label="Р./сч." placeholder="Номер расчетного счета" v-model="customer.accountNumber" :errors="errors.accountNumber" :mask="'#########################'")
                    Input(label="БИК" placeholder="БИК" v-model="customer.bankBIC" :errors="errors.bankBIC" :mask="'#########'")
                    Input(label="К./сч." placeholder="Номер корреспондентского счёта" v-model="customer.bankCorAccount" :errors="errors.bankCorAccount" :mask="'####################'")
                    Input(label="Наименование банка" placeholder="Наименование банка" v-model="customer.bankName" :errors="errors.bankName")
                    Input(label="Юр. Адрес" placeholder="Адрес" v-model="customer.legalAddress" :errors="errors.legalAddress")
                    .btn-wrapper.center.mt-10 
                        button.btn.primary(@click.prevent="registerCustomer") Отправить
                Tab(name="ao" :activeTab="activeTab" v-model="tabViewerHeight")
                    Input(label="Наименование организации" placeholder="OOO Иванов Иван Иванович" v-model="customer.orgName" :errors="errors.orgName")
                    Input(label="Email" placeholder="example@mail.com" v-model="customer.email" :errors="errors.email")
                    Input(label="ОГРН" placeholder="ОГРН" v-model="customer.ogrn" type="text" :errors="errors.ogrn" :mask="'#############'")
                    Input(label="ИНН" placeholder="ИНН" v-model="customer.inn" type="text" :errors="errors.inn" name="ooo-inn" :mask="'############'")
                    Input(label="КПП" placeholder="КПП" v-model="customer.kpp" type="text" :errors="errors.kpp" :mask="'#########'")
                    Input(label="Р./сч." placeholder="Номер расчетного счета" v-model="customer.accountNumber" :errors="errors.accountNumber" :mask="'#########################'")
                    Input(label="БИК" placeholder="БИК" v-model="customer.bankBIC" :errors="errors.bankBIC" :mask="'#########'")
                    Input(label="К./сч." placeholder="Номер корреспондентского счёта" v-model="customer.bankCorAccount" :errors="errors.bankCorAccount" :mask="'####################'")
                    Input(label="Наименование банка" placeholder="Наименование банка" v-model="customer.bankName" :errors="errors.bankName")
                    Input(label="Юр. Адрес" placeholder="Адрес" v-model="customer.legalAddress" :errors="errors.legalAddress")
                    .btn-wrapper.center.mt-10 
                        button.btn.primary(@click.prevent="registerCustomer") Отправить
                Tab(name="pao" :activeTab="activeTab" v-model="tabViewerHeight")
                    Input(label="Наименование организации" placeholder="OOO Иванов Иван Иванович" v-model="customer.orgName" :errors="errors.orgName")
                    Input(label="Email" placeholder="example@mail.com" v-model="customer.email" :errors="errors.email")
                    Input(label="ОГРН" placeholder="ОГРН" v-model="customer.ogrn" type="text" :errors="errors.ogrn" :mask="'#############'")
                    Input(label="ИНН" placeholder="ИНН" v-model="customer.inn" type="text" :errors="errors.inn" name="ooo-inn" :mask="'############'")
                    Input(label="КПП" placeholder="КПП" v-model="customer.kpp" type="text" :errors="errors.kpp" :mask="'#########'")
                    Input(label="Р./сч." placeholder="Номер расчетного счета" v-model="customer.accountNumber" :errors="errors.accountNumber" :mask="'#########################'")
                    Input(label="БИК" placeholder="БИК" v-model="customer.bankBIC" :errors="errors.bankBIC" :mask="'#########'")
                    Input(label="К./сч." placeholder="Номер корреспондентского счёта" v-model="customer.bankCorAccount" :errors="errors.bankCorAccount" :mask="'####################'")
                    Input(label="Наименование банка" placeholder="Наименование банка" v-model="customer.bankName" :errors="errors.bankName")
                    Input(label="Юр. Адрес" placeholder="Адрес" v-model="customer.legalAddress" :errors="errors.legalAddress")
                    .btn-wrapper.center.mt-10 
                        button.btn.primary(@click.prevent="registerCustomer") Отправить
</template>

<script setup>
definePageMeta({
    middleware: ['sanctum:guest'],
});

const tabViewerHeight = ref(0)
const activeTab = ref('phone')
const tabsCustomer = ref([
    {
        title: 'ИП',
        value: 'ip'
    },
    {
        title: 'ООО',
        value: 'ooo'
    },
    {
        title: 'АО',
        value: 'ao'
    },
    {
        title: 'ПАО',
        value: 'pao'
    }
])
const customer = ref({
    phone: '',
    code: '',
    surname: '',
    name: '',
    patronymic: '',
    email: '',
    birthDate: '',
    seria: '',
    number: '',
    issuer: '',
    issueDate: '',
    issuerCode: '',
    inn: '',
    accountNumber: '',
    bankBIC: '',
    bankCorAccount: '',
    bankName: '',
    ogrnip: '',
    postAddress: '',
    orgName: '',
    ogrn: '',
    kpp: '',
    taxation_system: 'osn',
    type: 'ip',
    legalAddress: '',
})
const taxationSystems = [
    {value: 'osn', text: 'Общая система налогообложения (ОСН)'},
    {value: 'usn', text: 'Упрощенная система налогообложения (УСН)'},
    {value: 'envd', text: 'Единый налог на вмененный доход (ЕНВД)'},
    {value: 'patent', text: 'Патентная система налогообложения (ПСН)'}
]
const errors = ref([])

async function confirmPhone() {
    errors.value = {}
    try {
        const {status} = await $api.post(`/register/confirm/phone`, {
            phone: customer.value.phone,
            code: customer.value.code,
            agree1: customer.value.agree1,
            agree2: customer.value.agree2,
            agree3: customer.value.agree3,
        })

        if (status === 'success') {
            $notice.success('Номер подтвержден')
            activeTab.value = customer.value.type
        }
    } 
    catch (error) {
        errors.value = error?.response?._data?.errors ? {...error.response._data.errors} : {}
        $notice.handleError(error)
    }
}

async function registerCustomer() {
    errors.value = {}
    try {
        const {status} = await $api.post(`/register/customer`, $api.createFormData(customer.value))
        if (status === 'success') {
            $notice.success('Регистрация прошла успешно')
            await useSanctumAuth().refreshIdentity()
            setTimeout(() => {
                navigateTo('/')
            }, 2000)
            
        }
    }
    catch (error) {
        errors.value = error?.response?._data?.errors ? {...error.response._data.errors} : {}
        tabViewerHeight.value += 500
        $notice.handleError(error)
    }
}

</script>