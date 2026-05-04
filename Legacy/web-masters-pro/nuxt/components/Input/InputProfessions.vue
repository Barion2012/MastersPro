<template lang="pug">
.professions-wrapper 
    .profession(v-for="(item, index) in model")
        button.remove-card(v-if="index > 0" @click="removeProfession(index)")
        Select(v-model="item.profession_id" label="Профессия" :options="professions" @input="emits('update-height', 400)")
        p.profession-desc(v-html="desc[item.profession_id]")
        //Select(v-model="item.profession_level_id" label="Уровень" :options="levels[item.profession_id]")
    .btn-wrapper.left
        button.btn.primary.sm(@click.prevent="addProfession") Добавить профессию
</template>

<script setup>
const model = defineModel()
const emits = defineEmits('update-height')
const professions = ref([])
const levels = ref([])
const desc = ref([])

function addProfession() {
    model.value.push({
        profession_id: '',
        profession_level_id: 1
    })
    emits('update-height', 240)
}

function removeProfession(index) {
    model.value.splice(index, 1)
}

onMounted(async () => {
    try {
        const response = await $api.get(`/profession/list/select`)
        console.log(professions)
        professions.value = response.professions
        levels.value = response.levels
        desc.value = response.desc
    }
    catch (error) {
        $notice.handleError(error)
    }
})
</script>