<template lang="pug">
Modal(v-model="model")
    span.modal-title Добавление уровня профессии
    Input(label="Название" v-model="levelDesc")
    Input(label="Цена" v-model="levelPrice" type="number")
    .btn-wrapper.center.mt-10 
        button.btn.primary.mr-10.sm(@click="saveProfessionLevel") Сохранить
        button.btn.danger.sm(@click="model = false") Отмена

</template>

<script setup>
const model = defineModel()
const props = defineProps(['level'])
const emits = defineEmits(['save'])

const levelDesc = ref('')
const levelPrice = ref('')

watch(() => props.level, () => {
    levelDesc.value = props.level.desc
    levelPrice.value = props.level.price
})

async function saveProfessionLevel() {
    try {
        await $api.post(`/profession/level/save`, {
            id: props.level?.id,
            profession_id: props.level?.profession_id,
            desc: levelDesc.value,
            price: levelPrice.value
        })
        model.value = false
        emits('save')
        $notice.success('Данные успешно сохранены')
    }
    catch(error) {
        $notice.handleError(error)
    }
}
</script>