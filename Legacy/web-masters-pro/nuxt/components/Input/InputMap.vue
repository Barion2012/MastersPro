<template lang="pug">
.input-map-wrapper(:class="{error: props.errors?.length > 0}")
    label {{ props.label }}
    .input-errors(:class="{active: props.errors?.length > 0}")
        span(v-for="item in props.errors") {{ item }}
    button.input-map(@click.prevent="showModal = true")
        span(v-if="modelAddress") {{ modelAddress }}
        span(v-else) Нажмите, чтобы выбрать ваше местоположение
    InputMapModal(
        v-model:show="showModal"
        v-model:lat="modelLat"
        v-model:lng="modelLng"
        v-model:address="modelAddress"
    )
</template>

<script setup>
const props = defineProps({
    label: {
        type: String,
        default: ''
    },
    errors: {
        type: Array,
        default: []
    }
})
const modelLat = defineModel('lat')
const modelLng = defineModel('lng')
const modelAddress = defineModel('address')
const showModal = ref(false)

function showMapModal() {
    showModal.value = true
}
</script>