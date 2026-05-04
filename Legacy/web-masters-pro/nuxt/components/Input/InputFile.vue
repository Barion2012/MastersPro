<template lang="pug">
    .input-file
        label.add-file(:for="props.name")
            img(:src="props.icon" v-if="props.icon")
            span {{ props.label }}
            input(type="file" :id="props.name" :ref="props.name" :multiple="props.multiple" :accept="props.accept" @change="handleInputChange")
        .filelist
            .file(v-for="(file, index) in filelist" @click="handleDeleteFile(index)")
                span {{ file.name }}
</template>
    
<script setup>
const emit = defineEmits(['update:modelValue'])

const props = defineProps({
    icon: {
        type: String,
        default: ''
    },
    label: {
        type: String,
        default: ''
    },
    multiple: {
        type: Boolean,
        default: false
    },
    accept: {
        type: String,
        default: ''
    },
    name: {
        type: String,
        default: 'input-file'
    }
})

const filelist = reactive([])
const input = useTemplateRef(props.name)

function handleInputChange() {
    if (!props.multiple)
        filelist.length = 0;

    for (const file of input.value.files)
        filelist.push(file)

    emit("update:modelValue", filelist)
}

function handleDeleteFile(index) {
    filelist.splice(index, 1)
    emit("update:modelValue", filelist)
}
</script>

<style lang="scss">
</style>