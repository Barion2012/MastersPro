<template lang="pug">
.popover-wrapper
    .popover.text(v-if="props.type === 'text'")
        .text(v-html="props.text")
    .popover.image(v-if="props.type === 'image'")
        .image 
            img(:src="props.image")
    slot
</template>

<script setup>
const props = defineProps({
    text: {
        type: String,
        default: ''
    },
    type: {
        type: String,
        default: 'text'
    },
    image: {
        type: String,
        default: ''
    }
})
</script>