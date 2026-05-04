<template lang="pug">
.select-wrapper(:class="{error: props.errors?.length > 0}")
    label(v-if="props.label") {{ props.label }}
    .input-errors(:class="{active: props.errors?.length > 0}")
        span(v-for="item in props.errors") {{ item }}
    .select
        select(v-model="model")
            option(value="") Выберите
            option(v-for="option in options" :value="option.value") {{ option.text }}
</template>

<script setup>
const props = defineProps({
    label: {
        type: String,
        default: ''
    },
    options: {
        type: Array,
        default: () => []
    },
    errors: {
        type: Array,
        default: []
    }
})
const model = defineModel()
</script>