<template lang="pug">
    button.btn.sm.danger(@click="logout") Выйти
</template>

<script setup>
async function logout() {
    try {
        await $api.logout()
    }
    catch(error) {
        $notice.handleError(error)
    }
}
</script>