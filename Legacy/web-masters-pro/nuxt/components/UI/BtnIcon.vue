<template lang="pug">
    button.btn-icon(@click="(event) => emit('click', event)")
        .icon 
            img(:src="props.icon")
</template>

<script setup>
const props = defineProps(['icon'])
const emit = defineEmits(['click'])
</script>