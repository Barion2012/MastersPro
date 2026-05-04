<template lang="pug">
.work-item
    button.remove-card(v-if="props.index > 0" @click="emits('delete')")
    .grid
        .col.works
            .param 
                .label Вид работ
                Select(:options="appStore.professions" v-model="model.profession_id" @change="autoSelectFirstLevel")
            .param
                InputCounter(label="Количество" v-model="model.count")
        //.col 
            //.param 
                .label Уровень мастера
                Select(:options="appStore.professionsLevels[model.profession_id]" v-model="model.profession_level_id")
            .param 
                Input(label="Дата начала" v-model="model.start_date" type="datetime-local")
        //.col 
            .param 
                Input(label="Дата окончания" v-model="model.end_date" type="datetime-local")
        .col.dates 
            .param 
                Input(label="Дата начала" v-model="model.start_date" type="date")
            .param 
                Input(label="Дата окончания" v-model="model.end_date" type="date")
        .col.dates 
            .param 
                Input(label="Время встречи" v-model="model.meet_time" type="time")
</template>

<script setup>
import { useAppStore } from '~/store/app'

const appStore = useAppStore()
const model = defineModel()
const props = defineProps({
    index: {
        type: Number,
        required: true
    }
})
const emits = defineEmits(['delete'])

function autoSelectFirstLevel() {
    //console.log(appStore.professionsLevels[model.value.profession_id][0])
    model.value.profession_level_id = appStore.professionsLevels[model.value.profession_id][0]['value']
}
</script>