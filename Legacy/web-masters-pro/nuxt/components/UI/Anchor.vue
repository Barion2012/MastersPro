<template lang="pug">
    a(:href="href" @click.prevent="anchor").anchor
        slot
</template>

<script setup>
const props = defineProps({
    href: {
        type: String,
        default: '#'
    }
})

function anchor() {
    const anchor = document.querySelector(props.href)
    if (anchor) {
        anchor.scrollIntoView({
            behavior: 'smooth',
            block: 'start'
        })
    }
}
</script>