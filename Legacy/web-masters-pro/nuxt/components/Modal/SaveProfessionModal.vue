<template lang="pug">
Modal(v-model="model")
    span.modal-title Создание профессии 
    Input(label="Название" v-model="professionName")
    TextEditor(label="Описание" v-model="professionDesc").mb-10 
    .btn-wrapper.center.mt-20 
        button.btn.primary.mr-10.sm(@click="saveProfession") Сохранить
        button.btn.danger.sm(@click="model = false") Отмена

</template>

<script setup>

const model = defineModel()
const props = defineProps(['profession'])
const emits = defineEmits(['save'])

const professionName = ref('')
const professionDesc = ref('')

watch(() => props.profession, () => {
    professionName.value = props.profession.name
    professionDesc.value = props.profession.desc
})

async function saveProfession() {
    try {
        await $api.post(`/profession/save`, {
            id: props.profession?.id,
            name: professionName.value,
            desc: professionDesc.value
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