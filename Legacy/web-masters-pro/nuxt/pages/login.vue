<template lang="pug">
main 
    section.pd-header.bg-screen.screen
        .container.center
            form.form-auth(@submit.prevent="login")
                SendCodeForm(v-model:phone="phone" v-model:code="code" @submit="login")
</template>

<script setup>
definePageMeta({
    middleware: ['sanctum:guest'],
});

const phone = ref('')
const code = ref('')

async function login() {
    try {
        await $api.loginBySmsCode({
            phone: phone.value,
            code: code.value
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