<template lang="pug">
.text-editor
    label(v-if="label") {{ label }}
    QuillEditor(:options="options" ref="editor" v-model:content="model" contentType="html") 
</template>

<script setup>

const model = defineModel()
const props = defineProps({
    label: {
        type: String,
        default: ''
    }
})

const options = {
    theme: 'snow',
    modules: {
        toolbar: {
            container: [
                ['bold', 'italic', 'underline'],
            ],
        },
    },
    placeholder: '',
    readOnly: false
}
</script>