<template lang="pug">
    .dropdown(:class="{active: active}")
        .d-header(@click="toggleDropdown") {{ props.header }}
        .d-wrapper(:style="{height: `${height}px`}")
            .d-content(ref="nodeContent") {{ props.content }}
</template>

<script setup>
const props = defineProps(['header', 'content'])
const active = ref(false)
const height = ref(0)
const nodeContent = useTemplateRef('nodeContent')

function toggleDropdown() {
    if (active.value) {
        height.value = 0
        active.value = false
        return;
    }

    height.value = nodeContent.value.offsetHeight
    active.value = true     
}
</script>