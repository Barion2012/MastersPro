<template lang="pug">
.app-title-wrapper
    h1.app-title {{ props.title }}
    button.goback(v-if="goback" @click="router.back()") << Вернуться назад
</template>

<script setup>
const router = useRouter()

const props = defineProps({
    title: {
        default: '',
        type: String
    },
    goback: {
        default: false,
        type: Boolean 
    }
})
</script>