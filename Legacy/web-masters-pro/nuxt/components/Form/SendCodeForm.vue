<template lang="pug">
.send-code-form
    Input(label="Введите ваш номер телефона" placeholder="+7 (___) ___-__-__" v-model="phone" mask="+7(###) ###-##-##" :errors="errors.phone")
    .sms-wrapper(:class="{active: codeSended}")
        Input(label="Введите код подтверждения из SMS" placeholder="" v-model="code" mask="####" :errors="errorsInputCode")
        span.timer(:class="{active: activeTimer}") Запросить код повторно можно через: {{ seconds }} сек.
    .btn-wrapper.center
        button.btn.primary.mr-10.btn-submit(:class="{active: codeSended}" @click.prevent="emit('submit')") {{ props.submitText }}
        button.btn(@click.prevent="sendSmsCode") Запросить код
    InputCheckbox(v-if="mode === 'register'" name="agree-1" :errors="errors.agree1" v-model="agree1").mt-20  Я подтверждаю, что ознакомлен(а) с <a href="/files/docs/personal_data_agreement.pdf" target="_blank">Пользовательским соглашением</a> и <a href="/files/docs/blank.pdf" target="_blank">Офертой платформы</a> и принимаю условия их использования. 
    InputCheckbox(v-if="mode === 'register'" name="agree-2" :errors="errors.agree2" v-model="agree2").mt-20 Я даю согласие на обработку своих персональных данных в соответствии с <a href="/files/docs/personal_data_processing_policy.pdf" target="_blank">Политикой конфиденциальности</a> и требованиями Федерального закона № 152-ФЗ. 
    InputCheckbox(v-if="mode === 'register'" name="agree-3" :errors="errors.agree3" v-model="agree3").mt-20 Я согласен(а) получать уведомления и рассылки (SMS, email, push) от платформы. 
</template>

<script setup>
const phone = defineModel('phone')
const code = defineModel('code')
const agree1 = defineModel('agree1')
const agree2 = defineModel('agree2')
const agree3 = defineModel('agree3')

const props = defineProps({
    submitText: {
        type: String,
        default: 'Войти'
    },
    errorsInputCode: {
        type: Array,
        default: []
    },
    mode: {
        type: String,
        default: 'login' // 'register' or 'login'
    }
})

const emit = defineEmits(['submit', 'codeSended'])

const codeSended = ref(false)
const activeTimer = ref(false)

const seconds = ref(0)
const intervalId = ref('')

const errors = ref({})

watch(() => activeTimer.value, (newVal) => {
    if (newVal) {
        seconds.value = 60
        intervalId.value = setInterval(() => {
            if (seconds.value > 0) {
                seconds.value--
            } else {
                clearInterval(intervalId.value)
                activeTimer.value = false
            }
        }, 1000)
    } else {
        clearInterval(intervalId.value)
        activeTimer.value = false
    }
})

async function sendSmsCode() {
    errors.value = {}
    try {
        switch (props.mode) {
            case 'register':
                await $api.post(`/register/sms/send`, {
                    phone: phone.value,
                    agree1: agree1.value,
                    agree2: agree2.value,
                    agree3: agree3.value
                })
                break
            case 'login':
                await $api.post(`/auth/sms/send`, {
                    phone: phone.value
                })
                break
            default:
                $notice.error('Неизвестный режим отправки кода')
                return
        }
        
        $notice.info('Код отправлен')

        if (codeSended.value === false) {
            codeSended.value = true
            emit('codeSended')
        }
        
        activeTimer.value = true
    }
    catch(error) {
        errors.value = error?.response?._data?.errors ? {...error.response._data.errors} : {}
        console.log(error.response._data.errors)
        $notice.handleError(error)
    }
}
</script>