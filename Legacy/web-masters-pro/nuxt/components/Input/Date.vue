<template lang="pug">
.date-wrapper 
    label(v-if="props.label") {{ props.label }}
    .input-errors(:class="{active: props.errors?.length > 0}")
        span(v-for="item in props.errors") {{ item }}
    ClientOnly
        VueDatePicker(
            v-model="input" 
            locale="ru-RU" 
            cancel-text="Отмена" 
            select-text="Выбрать" 
            month-name-format="long" 
            :format="formatDate" 
            :placeholder="props.placeholder" 
            :range="false" 
            auto-apply
            :enable-time-picker="false"
        ) 
            template(v-slot:input-icon)
                img.icon(src="/images/icons/date.png")
</template>

<script setup>
const model = defineModel()
const input = ref('')
const props = defineProps({
    label: {
        type: String,
        default: ''
    },
    placeholder: {
        type: String,
        default: ''
    },
    errors: {
        type: Array,
        default: []
    },
})

watch(() => input.value, () => model.value = formatDate(input.value))

function formatDate(date) {
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();

    return `${day}.${month}.${year}`;
}
</script>