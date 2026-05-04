<template lang="pug">
.layout
    SiteHeader(v-if="!isAuthenticated")
    AppHeader(v-else)
    slot
    Footer
</template>

<script setup>
const { isAuthenticated } = useSanctumAuth()
</script>

<style scoped>

</style>