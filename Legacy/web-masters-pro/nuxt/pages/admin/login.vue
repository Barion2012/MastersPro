<template lang="pug">
main 
    section.pd-header.bg-screen.screen
        .container.center
            form.form-auth(@submit.prevent="login" style="max-width: 400px; width: 100%")
                Input(label="Введите ваш email" placeholder="example@mail.com" v-model="email" :errors="errors.email")
                Input(label="Введите ваш пароль" placeholder="********" v-model="password" :errors="errors.password" type="password")
                .btn-wrapper.center.mt-20
                     button.btn.primary.mr-10.btn-submit.sm(@click.prevent="login") Войти
</template>

<script setup>
definePageMeta({
    middleware: ['sanctum:guest'],
});

const email = ref('')
const password = ref('')
const errors = ref({})

async function login() {
    try {
        await $api.login({
            email: email.value,
            password: password.value
        })

        redirectToDashboard()
    }
    catch(error) {
        $notice.handleError(error)
    }
}

function redirectToDashboard() {
    const user = useSanctumUser()

    switch (user.value.role) {
        case 'admin':
            navigateTo('/admin/dashboard')
            break;
        case 'superuser':
            navigateTo('/admin/dashboard')
            break;
        case 'worker':
            navigateTo('/worker/dashboard')
            break;
        case 'manager':
            navigateTo('/admin/dashboard')
            break;
        case 'customer':
            navigateTo('/')
            break;
        default:
            navigateTo('/')
            break;
    }
}
</script>

<style scoped>

</style>