<template lang="pug">
.layout
    AppHeader 
    .content
        slot
    Footer
</template>

<script>
export default {
    name: "default"
}
</script>

<style scoped>

</style>