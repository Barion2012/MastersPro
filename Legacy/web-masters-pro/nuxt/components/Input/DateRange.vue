<template lang="pug">
.date-wrapper 
    label(v-if="props.label") {{ props.label }}
    ClientOnly
        VueDatePicker(
            v-model="model" 
            locale="ru-RU" 
            cancel-text="Отмена" 
            select-text="Выбрать" 
            month-name-format="long" 
            :format="format" 
            :placeholder="props.placeholder" 
            :range="true" 
            auto-apply
            :enable-time-picker="false"
        ) 
            template(v-slot:input-icon)
                img.icon(src="/images/icons/date.png")
</template>

<script setup>
const model = defineModel()
const props = defineProps({
    label: {
        type: String,
        default: ''
    },
    placeholder: {
        type: String,
        default: ''
    },
})

const format = (dates) => { 
    let result = [];
    for (let date of dates) {
        result.push(formatDate(date));
    }

    return result.join(' - ');
}

function formatDate(date) {
    console.log(date)
    const day = date.getDate();
    const month = date.getMonth() + 1;
    const year = date.getFullYear();

    return `${day}.${month}.${year}`;
}
</script>