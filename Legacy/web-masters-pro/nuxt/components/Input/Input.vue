<template lang="pug">
.input-wrapper(:class="{ error: props.errors?.length > 0 }")
    label {{ props.label }}
    .input-errors(:class="{active: props.errors?.length > 0}")
        span(v-for="item in props.errors") {{ item }}
    .input(:class="props.type" @click="input.showPicker()")
        input(:placeholder="props.placeholder" v-model="model" :type="props.type" v-mask="mask" ref="input" :step="props.step" :min="props.min" v-if="props.type !== 'datetime-local' && props.type !== 'date' && props.type !== 'time'")
        input(:placeholder="props.placeholder" v-model="model" :type="props.type" ref="input" v-else)  
</template>

<script setup>
const input = useTemplateRef('input')
const props = defineProps({
    placeholder: {
        type: String,
        default: ''
    },
    type: {
        type: String,
        default: 'text'
    },
    label: {
        type: String,
        default: ''
    },
    mask: {
        type: String,
        default: ''
    },
    errors: {
        type: Array,
        default: []
    },
    step: {
        type: [Number, String],
        default: 1
    },
    min: {
        type: [Number, String],
        default: 0
    },
})
const model = defineModel()

</script>

<style>

</style>