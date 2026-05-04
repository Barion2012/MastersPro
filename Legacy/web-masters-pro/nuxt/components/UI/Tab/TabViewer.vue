<template lang="pug">
.tab-viewer(:style="{ 'max-height': `${props.height}px` }")
    slot
</template>

<script setup>
const props = defineProps(['height'])
const emits = defineEmits(['update-height'])

const init = ref(false)

watchEffect(() => emits('update-height', props.height))
</script>