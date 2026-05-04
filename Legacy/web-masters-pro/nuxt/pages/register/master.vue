<template lang="pug">
section.register.pd-header.pd-bottom.bg-screen.screen
    .container 
        form.register 
            TabViewer(:height="tabViewerHeight")
                Tab(name="citizenship" :activeTab="activeTab" v-model="tabViewerHeight")
                    TabSwitcher(:tabs="tabsCitizenship" v-model="worker.citizenship")
                    SendCodeForm(
                        v-model:phone="worker.phone" 
                        v-model:code="worker.code" 
                        @submit="confirmPhone" 
                        @codeSended="updateTabViewer(132)" 
                        submit-text="Подтвердить номер"
                        :errorsInputCode="errors.code"
                        v-model:agree1="worker.agree1"
                        v-model:agree2="worker.agree2"
                        mode="register"
                    )
                Tab(name="citizen" :activeTab="activeTab" v-model="tabViewerHeight")
                    Input(label="Ваше ФИО" placeholder="ФИО" v-model="worker.name" :errors="errors.name")
                    Input(label="Email" placeholder="example@mail.com" v-model="worker.email" type="email" :errors="errors.email")
                    Popover(text="Состоит из 12 цифр. Можно получить в электронном виде на сайте ФНС или на Госуслугах")
                        Input(label="ИНН" placeholder="ИНН" v-model="worker.inn" :errors="errors.inn" mask="############")
                    //Input(label="Текущий адрес или прописка, начиная с города" placeholder="Адрес" v-model="worker.address" :errors="errors.address")
                    Popover(image="/images/layout_elements/popup/ser.jpg" type="image")
                        Input(label="Серия" placeholder="1122"  mask="####" v-model="worker.passport_series" :errors="errors.passport_series")
                    Popover(image="/images/layout_elements/popup/num.jpg" type="image")
                        Input(label="Номер" placeholder="112233"  mask="######" v-model="worker.passport_number" :errors="errors.passport_number")
                    Popover(text="Полностью как указано в вашем паспорте.")
                        Input(label="Кем выдан" placeholder="Кем выдан" v-model="worker.passport_issued_by" :errors="errors.passport_issued_by")
                    Popover(
                        text="Если нет возможности отсканировать паспорт, сфотографируйте его так, чтобы в кадре отображались и были хорошо видны все данные:<br> <br>Надписи, цифры, отметки, оттиски. Печать органа, который выдал паспорт. Код подразделения должен хорошо читаться."
                        )
                        InputImage(label="Фото паспорта (основной разворот)" v-model="worker.passport_scan" name="passport_citizen" :errors="errors.passport_scan")
                    Popover(
                        text="Если нет возможности отсканировать паспорт, сфотографируйте его так, чтобы в кадре отображались и были хорошо видны все данные:<br> <br>Надписи, цифры, отметки, оттиски. Печать органа, который выдал паспорт. Код подразделения должен хорошо читаться."
                        )
                        InputImage(label="Фото паспорта (прописка)" v-model="worker.passport_reg_scan" name="passport_citizen_reg" :errors="errors.passport_reg_scan")
                    Popover(image="/images/layout_elements/popup/selfie.jpg" type="image")
                        InputImage(label="Ваше фото с паспортом (должны быть видны данные паспорта)" v-model="worker.passport_selfie" name="selfie_citizen" :errors="errors.passport_selfie")
                    Popover(text="СНИЛС – страховой номер индивидуального лицевого счёта – состоит из 11 цифр. <br> Где узнать СНИЛС: <br><br> - На Госуслугах. СНИЛС можно посмотреть в личном кабинете → Документы → Личные документы. ... <br><br> - На сайте СФР. Для входа в личный кабинет понадобится подтверждённая учётная запись. ... <br><br> - В отделении СФР или офисе МФЦ. Возьмите с собой паспорт.")
                        //InputImage(label="СНИЛС" v-model="worker.snils" name="snils_citizen" :errors="errors.snils")
                        Input(label="СНИЛС" placeholder="СНИЛС" v-model="worker.snils" :errors="errors.snils" mask="###-###-### ##")
                    .input-wrapper.requisites 
                        label Реквизиты банковского счета (для зачисления оплаты)
                        Popover(text="Подсказка о том, где искать название банка")
                            Input(placeholder="Название банка" v-model="worker.bank_name" :errors="errors.bank_name")
                        Popover(text="Подсказка о том, где искать номер счета")
                            Input(placeholder="Номер счета" v-model="worker.account_number" :errors="errors.account_number" :mask="'#########################'")
                        Popover(text="Подсказка о том, где искать корреспондентский счет")
                            Input(placeholder="Корреспондентский счет" v-model="worker.bank_cor_account" :errors="errors.bank_cor_account" :mask="'####################'")
                        Popover(text="Подсказка о том, где искать БИК")
                            Input(placeholder="БИК банка" v-model="worker.bank_bic" :errors="errors.bank_bic" :mask="'#########'")
                    .btn-wrapper.center.mt-10 
                        button.btn.primary(@click.prevent="checkForm") Далее
                Tab(name="foreigner" :activeTab="activeTab" v-model="tabViewerHeight")
                    TabSwitcher(:tabs="tabsForeigner" v-model="worker.citizenship")
                    TabViewer(:height="tabViewerForeignerHeight" @update-height="syncTabViewers")
                        Tab(name="eaeu" :activeTab="worker.citizenship" v-model="tabViewerForeignerHeight")
                            .input-wrapper 
                                label ЕАЭС: Белоруссия, Киргизия, Казахстан, Армения
                            Input(label="Ваше ФИО" placeholder="ФИО" v-model="worker.name" :errors="errors.name")
                            Input(label="Email" placeholder="example@mail.com" v-model="worker.email" type="email" :errors="errors.email")
                            Popover(text="Подсказка о том, где искать ИНН")
                                Input(label="ИНН" placeholder="ИНН" v-model="worker.inn" :errors="errors.inn" mask="############")
                            Popover(text="Подсказка о том, где искать серию")
                                Input.series(label="Серия" placeholder="1122"  mask="####" v-model="worker.passport_series" :errors="errors.passport_series")
                            Popover(text="Подсказка о том, где искать номер")
                                Input.number(label="Номер" placeholder="112233"  mask="######" v-model="worker.passport_number" :errors="errors.passport_number")
                            Popover(text="Подсказка о том, где искать кем выдано")
                                Input(label="Кем выдан" placeholder="Кем выдан" v-model="worker.passport_issued_by" :errors="errors.passport_issued_by")
                            Popover(image="/images/layout_elements/popover-image.jpg" type="image")
                                InputImage(label="Фото паспорта (основной разворот)" v-model="worker.passport_scan" name="passport_eaeu" :errors="errors.passport_scan")
                            Popover(
                                text="Если нет возможности отсканировать паспорт, сфотографируйте его так, чтобы в кадре отображались и были хорошо видны все данные:<br> <br>Надписи, цифры, отметки, оттиски. Печать органа, который выдал паспорт. Код подразделения должен хорошо читаться."
                                )
                                InputImage(label="Фото паспорта (прописка)" v-model="worker.passport_reg_scan" name="passport_eaeu_ref" :errors="errors.passport_reg_scan")
                            Popover(image="/images/layout_elements/popover-image.jpg" type="image")
                                InputImage(label="Ваше фото с паспортом (должны быть видны данные паспорта)" v-model="worker.passport_selfie" name="selfie_eaeu" :errors="errors.passport_selfie")
                            Popover(image="/images/layout_elements/popover-image.jpg" type="image")
                                InputImage(label="Миграционная карточка" v-model="worker.migration_card" name="migration_card_eaeu" :errors="errors.migration_card")
                            Popover(text="Подсказка о том, где искать СНИЛС")
                                //InputImage(label="СНИЛС" v-model="worker.snils" name="snils_eaeu" :errors="errors.snils")
                                Input(label="СНИЛС" placeholder="СНИЛС" v-model="worker.snils" :errors="errors.snils" mask="###-###-### ##")
                            .input-wrapper.requisites 
                                label Реквизиты банковского счета (для зачисления оплаты)
                                Popover(text="Подсказка о том, где искать название банка")
                                    Input(placeholder="Название банка" v-model="worker.bank_name" :errors="errors.bank_name")
                                Popover(text="Подсказка о том, где искать номер счета")
                                    Input(placeholder="Номер счета" v-model="worker.account_number" :errors="errors.account_number" :mask="'#########################'")
                                Popover(text="Подсказка о том, где искать корреспондентский счет")
                                    Input(placeholder="Корреспондентский счет" v-model="worker.bank_cor_account" :errors="errors.bank_cor_account" :mask="'####################'")
                                Popover(text="Подсказка о том, где искать БИК")
                                    Input(placeholder="БИК банка" v-model="worker.bank_bic" :errors="errors.bank_bic" :mask="'#########'")
                            .btn-wrapper.center.mt-10 
                                button.btn.primary(@click.prevent="checkForm") Далее
                        Tab(name="other" :activeTab="worker.citizenship" v-model="tabViewerForeignerHeight")
                            Input(label="Ваше ФИО" placeholder="ФИО" v-model="worker.name" :errors="errors.name")
                            Input(label="Email" placeholder="example@mail.com" v-model="worker.email" type="email" :errors="errors.email")
                            Popover(text="Подсказка о том, где искать ИНН")
                                Input(label="ИНН" placeholder="ИНН" v-model="worker.inn" :errors="errors.inn" mask="############")
                            //Popover(text="Подсказка о том, где искать адрес регистрации")
                                Input(label="Текущий адрес регистрации" placeholder="Адрес" v-model="worker.address" :errors="errors.address")
                            Popover(text="Подсказка о том, где искать серию")
                                Input.series(label="Серия" placeholder="1122"  mask="####" v-model="worker.passport_series" :errors="errors.passport_series")
                            Popover(text="Подсказка о том, где искать номер")
                                Input.number(label="Номер" placeholder="112233"  mask="######" v-model="worker.passport_number" :errors="errors.passport_number")
                            Popover(text="Подсказка о том, где искать кем выдано")
                                Input(label="Кем выдан" placeholder="Кем выдан" v-model="worker.passport_issued_by" :errors="errors.passport_issued_by")
                            Popover(image="/images/layout_elements/popover-image.jpg" type="image")
                                InputImage(label="Фото паспорта (основной разворот)" v-model="worker.passport_scan" name="passport_other" :errors="errors.passport_scan")
                            Popover(
                                text="Если нет возможности отсканировать паспорт, сфотографируйте его так, чтобы в кадре отображались и были хорошо видны все данные:<br> <br>Надписи, цифры, отметки, оттиски. Печать органа, который выдал паспорт. Код подразделения должен хорошо читаться."
                                )
                                InputImage(label="Фото паспорта (прописка)" v-model="worker.passport_reg_scan" name="passport_other_reg" :errors="errors.passport_reg_scan")
                            Popover(image="/images/layout_elements/popover-image.jpg" type="image")
                                InputImage(label="Ваше фото с паспортом (должны быть видны данные паспорта)" v-model="worker.passport_selfie" name="selfie_other" :errors="errors.passport_selfie")
                            Popover(image="/images/layout_elements/popover-image.jpg" type="image")
                                InputImage(label="Миграционная карточка" v-model="worker.migration_card" name="migration_card_other" :errors="errors.migration_card")
                            Popover(text="Подсказка о том, где искать СНИЛС")
                                //InputImage(label="СНИЛС" v-model="worker.snils" name="snils_other" :errors="errors.snils")
                                Input(label="СНИЛС" placeholder="СНИЛС" v-model="worker.snils" :errors="errors.snils" mask="###-###-### ##")
                            Popover(image="/images/layout_elements/popover-image.jpg" type="image")
                                InputImage(label="Патент (кроме Армении, Киргизии, Белоруси, Казахстана)" v-model="worker.patent" name="patent_other" :errors="errors.patent")
                            Popover(image="/images/layout_elements/popover-image.jpg" type="image")
                                InputImage(label="Чек об оплате патента" v-model="worker.patent_cheque" name="patent_cheque_other" :errors="errors.patent_cheque")
                            Popover(image="/images/layout_elements/popover-image.jpg" type="image")
                                InputImage(label="ДМС" v-model="worker.dms" name="dms_other" :errors="errors.dms")
                            .input-wrapper.requisites 
                                label Реквизиты банковского счета (для зачисления оплаты)
                                Popover(text="Подсказка о том, где искать название банка")
                                    Input(placeholder="Название банка" v-model="worker.bank_name" :errors="errors.bank_name")
                                Popover(text="Подсказка о том, где искать номер счета")
                                    Input(placeholder="Номер счета" v-model="worker.account_number" :errors="errors.account_number" :mask="'#########################'")
                                Popover(text="Подсказка о том, где искать корреспондентский счет")
                                    Input(placeholder="Корреспондентский счет" v-model="worker.bank_cor_account" :errors="errors.bank_cor_account" :mask="'####################'")
                                Popover(text="Подсказка о том, где искать БИК")
                                    Input(placeholder="БИК банка" v-model="worker.bank_bic" :errors="errors.bank_bic" :mask="'#########'")
                            .btn-wrapper.center.mt-10 
                                button.btn.primary(@click.prevent="checkForm") Далее
                Tab(name="jobs" :activeTab="activeTab" v-model="tabViewerHeight")
                    InputMap(label="Ваша локация" v-model:address="worker.location" v-model:lat="worker.location_lat" v-model:lng="worker.location_lng" :errors="errors.location")
                    InputProfessions(v-model="worker.professions" @update-height="updateTabViewer").mb-20
                    .btn-wrapper.center.mt-20 
                        button.btn.primary(@click.prevent="registerWorker") Далее
</template>

<script setup>
definePageMeta({
    middleware: ['sanctum:guest'],
});

const tabsCitizenship = ref([
    {
        title: 'Гражданин РФ',
        value: 'citizen'
    },
    {
        title: 'Иностранный гражданин',
        value: 'foreigner'
    }
])

const tabsForeigner = ref([
    {
        title: 'ЕАЭС',
        value: 'eaeu'
    },
    {
        title: 'Другие страны',
        value: 'other'
    }
])

const activeTab = ref('citizenship')
const tabViewerHeight = ref(250)
const tabViewerForeignerHeight = ref(0)

const errors = ref({})

const worker = ref({
    phone: '',
    code: '',
    citizenship: 'citizen',
    foreigner: 'eaeu',
    name: '',
    email: '',
    inn: '',
    address: '',
    passport_series: '',
    passport_number: '',
    passport_issued_by: '',
    passport_scan: null,
    passport_reg_scan: null,
    passport_selfie: null,
    snils: null,
    migration_card: null,
    patent: null,
    patent_cheque: null,
    dms: null,
    bank_name: '',
    account_number: '',
    bank_cor_account: '',
    bank_bic: '',
    location: '',
    location_lat: '',
    location_lng: '',
    professions: [{
        profession_id: '',
        profession_level_id: 1
    }]
})

async function sendSmsCode() {
    try {
        await $api.post(`/auth/sms/send`, {
            phone: worker.value.phone,
            agree1: worker.value.agree1,
            agree2: worker.value.agree2,
        })
        $notice.info('Код отправлен')
        tabViewerHeight.value += 132
        codeSended.value = true
        activeTimer.value = true
    }
    catch(error) {
        $notice.handleError(error)
    }
}

async function confirmPhone() {
    errors.value = {}
    try {
        const {status} = await $api.post(`/register/confirm/phone`, {
            phone: worker.value.phone,
            code: worker.value.code,
            agree1: worker.value.agree1,
            agree2: worker.value.agree2,
        })

        if (status === 'success') {
            $notice.success('Номер подтвержден')

            if (worker.value.citizenship === 'citizen') 
                activeTab.value = 'citizen'
            if (worker.value.citizenship === 'foreigner') {
                activeTab.value = 'foreigner'
                worker.value.citizenship = 'eaeu'
            }
        }
    } 
    catch (error) {
        errors.value = error?.response?._data?.errors ? {...error.response._data.errors} : {}
        $notice.handleError(error)
    }
}

async function checkForm() {
    errors.value = {}
    try {
        const {status} = await $api.post(`/register/worker/confirm/form`, $api.createFormData(worker.value))
        if (status === 'success') {
            activeTab.value = 'jobs'
        }
    }
    catch (error) {
        errors.value = error?.response?._data?.errors ? {...error.response._data.errors} : {}
        tabViewerHeight.value += 1000
        $notice.handleError(error)
    }
}

async function registerWorker() {
    errors.value = {}
    try {
        const {status} = await $api.post(`/register/worker/finish`, $api.createFormData(worker.value))
        if (status === 'success') {
            $notice.success('Регистрация прошла успешно')
            await useSanctumAuth().refreshIdentity()
            navigateTo('/worker/dashboard')
        }
    }
    catch (error) {
        errors.value = error?.response?._data?.errors ? {...error.response._data.errors} : {}
        tabViewerHeight.value += 500
        $notice.handleError(error)
    }
}

function updateTabViewer(height) {
    tabViewerHeight.value += height
}

function syncTabViewers() {
    console.log('asd')
    tabViewerHeight.value = tabViewerForeignerHeight.value + 70
}
</script>