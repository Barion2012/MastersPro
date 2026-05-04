<template lang="pug">
    Modal 
        .warning-modal 
            h2.modal-title {{ text }}
            .btn-wrapper.evenly
                button.btn.sm.danger(@click="emit('confirm')") Подтвердить
                button.btn.sm(@click="emit('cancel')") Отмена
    </template>
    
    <script setup>
    const { text } = defineProps(['text'])
    const emit = defineEmits(['confirm', 'cancel'])
    </script>