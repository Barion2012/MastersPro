<template lang="pug">
    .wait-wrapper
        .app-title-wrapper
            h1.app-title Принятое предложение
            .app-subtitle Ожидание начала смены. Дата начала смены: <span class="secondary">{{ acceptWork.work.start_date }}</span>
        .jobs-list
            WorkItem(:order="acceptWork")
                button.btn.primary.sm.danger(@click="$emit('refuse', acceptWork.work.id)") Отказаться от работы
</template>

<script setup>
defineProps({
    acceptWork: {
        type: Object,
        default: {}
    }
})

defineEmits(['refuse'])
</script>