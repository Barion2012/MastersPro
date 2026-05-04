<template lang="pug">
.input-wrapper 
    label {{ props.label }}
    .counter 
        button.minus(@click="minus")
        input(placeholder="0" v-model="model" type="number" min="0")
        button.plus(@click="plus")
</template>

<script setup>
const props = defineProps({
    label: {
        type: String,
        default: ''
    }
})

const model = defineModel()

function plus() {
    model.value++
}

function minus() {
    if (model.value > 0) model.value--
}
</script>