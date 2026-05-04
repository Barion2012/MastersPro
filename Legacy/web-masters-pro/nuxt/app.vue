<template lang="pug">
    NuxtLoadingIndicator
    NuxtLayout
        NuxtPage
    ModalsContainer
</template>

<script setup>
import { useAppStore } from '~/store/app'
import { ModalsContainer } from 'vue-final-modal'

const appStore = useAppStore()
appStore.initStore()
</script>

<style lang="scss">
</style>
