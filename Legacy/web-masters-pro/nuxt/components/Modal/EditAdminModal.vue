<template lang="pug">
Modal(v-model="model")
    span.modal-title(v-if="props.user.id") Редактирование пользователя
    span.modal-title(v-else) Добавление пользователя
    Input(label="ФИО" v-model="name")
    Input(label="Телефон" v-model="phone" mask="+7(###) ###-##-##")
    Input(label="Email" v-model="email")
    Select(label="Роль" v-model="role" :options="[{text: 'Администратор', value: 'admin'}, {text: 'Менеджер', value: 'manager'}, {text: 'Пользователь', value: 'user'}]")
    .btn-wrapper.center.mt-10
        button.btn.primary(@click="saveUser") Сохранить
</template>

<script setup>
const model = defineModel()
const props = defineProps(['user'])
const emits = defineEmits(['save'])

const name = ref('')
const phone = ref('')
const email = ref('')
const role = ref('')

watch(() => props.user, () => {
    name.value = props.user.name
    phone.value = props.user.phone
    email.value = props.user.email
    role.value = props.user.role
})

async function saveUser() {
    try {
        await $api.post('/user/save', {
            id: props.user.id,
            role: role.value,
            name: name.value,
            phone: phone.value,
            email: email.value,
        })
        model.value = false
        emits('save')
        $notice.success('Данные успешно сохранены')
    }
    catch(error) {
        $notice.handleError(error)
    }
}
</script>