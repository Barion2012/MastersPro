<template lang="pug">
.input-image
    .label {{ props.label }}
    .input-errors(:class="{active: props.errors?.length > 0}")
        span(v-for="item in props.errors") {{ item }}
    label(
        :for="props.name"
        @dragover.prevent
        @dragenter.prevent="isDragging = true"
        @dragleave.prevent="isDragging = false"
        @drop.prevent="onDrop"
        :class="{'drag': isDragging}"
    )
        .preview 
            img(src="/images/layout_elements/file_loaded.jpg" :class="{'active': fileLoaded}")
            img(src="/images/layout_elements/no-image-preview.png" :class="{'active': !fileLoaded}")
        .text 
            .label Добавьте или перенесите файл в эту область
            span В формате PDF, JPG, PNG, вес не более 20 МБ
        .ondrag
            .drag-text Перетащите файл сюда
    input(type="file" :id="props.name" :ref="props.name" :accept="props.accept" @change="handleInputChange")
    
</template>

<script setup> 
const model = defineModel()
const props = defineProps({
    label: {
        type: String,
        default: ''
    },
    name: {
        type: String,
        default: 'input-image'
    },
    accept: {
        type: String,
        default: 'image/jpeg,image/png,application/pdf'
    },
    errors: {
        type: Array,
        default: []
    }
})
const input = useTemplateRef(props.name)

const fileLoaded = ref(false)
const isDragging = ref(false)

function handleInputChange() {
    const file = input.value.files[0]
    if (file) {
        fileLoaded.value = true
        model.value = file
    } else {
        fileLoaded.value = false
        model.value = ''
    }
}

function onDrop(event) {
    isDragging.value = false
    const file = event.dataTransfer.files[0]
    if (file) {
        fileLoaded.value = true
        model.value = file
    } else {
        fileLoaded.value = false
        model.value = ''
    }
}
</script>