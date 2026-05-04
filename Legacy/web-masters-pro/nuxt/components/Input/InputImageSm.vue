<template lang="pug">
.input-image.sm
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
            img(src="/images/icons/load-picture.png" :class="{'active': !fileLoaded}")
        .ondrag
            .drag-text +
    input(type="file" :id="props.name" :ref="props.name" :accept="props.accept" @change="handleInputChange")
    
</template>

<script setup> 
const model = defineModel()
const props = defineProps({
    name: {
        type: String,
        default: 'input-image'
    },
    accept: {
        type: String,
        default: 'image/jpeg,image/png,application/pdf'
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