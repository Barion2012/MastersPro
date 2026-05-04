<template lang="pug">
    .container
        .app-title-wrapper
            h1.app-title Предложение работы
            .app-subtitle Если вы не подтвердите в течении <span class="secondary">20 минут</span>, то произойдет отказ от работы
        .order
            WorkItem(:order="acceptWork")
                .btn-wrapper.right
                    button.btn.primary.sm.danger.mr-10(@click="$emit('refuse', acceptWork.work.id)") Отказаться от работы
                    button.btn.sm.primary(@click="showModalContract = true") Принять работу
        Modal(v-model="showModalContract").user-modal
            span.modal-title(style="margin-bottom: 15px") Подтверждая выход на объект, вы соглашаетесь с условиями договора
            p(style="margin-bottom: 35px; text-align: center;") Ознакомиться с договором можно по 
                a(:href="`/api/order/${acceptWork.order.id}/worker/${user.worker.id}/contract/get`" target="_blank" style="font-size: 16px;").link ссылке
            .btn-wrapper.center
                button.btn.primary.sm(@click="$emit('accept', acceptWork.work.id); showModalContract = false") Подтверждаю
</template>

<script setup>
defineProps({
    acceptWork: {
        type: Object,
        default: {}
    }
})

defineEmits(['accept', 'refuse'])

const user = useSanctumUser()
const showModalContract = ref(false)
</script>