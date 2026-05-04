<template lang="pug">
VueFinalModal(class="modal" content-class="modal-content" overlay-transition="vfm-fade" content-transition="vfm-fade" v-model="model")
    slot
</template>

<script setup>
import { VueFinalModal } from 'vue-final-modal'

const model = defineModel()
</script>

<style lang="scss">
</style>