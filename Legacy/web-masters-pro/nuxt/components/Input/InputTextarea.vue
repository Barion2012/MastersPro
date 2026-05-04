<template lang="pug">
.input-wrapper(:class="{ error: props.errors?.length > 0 }")
    label {{ props.label }}
    .input-errors(:class="{active: props.errors?.length > 0}")
        span(v-for="item in props.errors") {{ item }}
    .input(:class="props.type")
        textarea(:placeholder="props.placeholder" v-model="model" ref="input" :rows="props.rows" :cols="props.cols")
</template>

<script setup>
const props = defineProps({
    placeholder: {
        type: String,
        default: ''
    },
    label: {
        type: String,
        default: ''
    },
    errors: {
        type: Array,
        default: () => []
    },
    rows: {
        type: [Number, String],
        default: 3
    },
    cols: {
        type: [Number, String],
        default: 20
    }
})
const model = defineModel()
</script>