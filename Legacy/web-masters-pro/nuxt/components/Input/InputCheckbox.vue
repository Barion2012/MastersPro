<template lang="pug">
.input-checkbox 
    .input-errors(:class="{active: props.errors?.length > 0}")
        span(v-for="item in props.errors") {{ item }}
    label(:for="props.name")
        input(type="checkbox" v-model="model" :name="props.name" :id="props.name")
        span.checkmark
        span 
            slot
</template>

<script setup>
const model = defineModel()
const props = defineProps({
    errors: {
        type: Array,
        default: []
    },
    name: {
        type: String,
        default: 'checkbox'
    }
})
</script>