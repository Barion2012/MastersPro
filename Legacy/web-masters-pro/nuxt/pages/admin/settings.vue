<template lang="pug">
main 
    section.dashdoard.pd-header.bg-screen.screen
        .container
            Pagetitle(:title="`Управление платформой`")
            .content-card
                .table.settings(v-if="settings.length > 0")
                    .row.head 
                        span Название
                        span Значение
                        span 
                    .row(v-for="item in settings" :key="item?.id")
                        span {{ settingLabels[item.key] }}
                        Input(v-model="item.value" type="number" min="0" :errors="errors.commission")
                        .btn-wrapper
                            button.btn.sm.secondary(@click="saveSetting(item.key, item.value)") Сохранить
</template>

<script setup>
definePageMeta({
    middleware: ['sanctum:auth', 'role:administration'],
});

const commission = ref(0)
const errors = ref({})
const settings = ref([])
const settingLabels = ref({
    'commission': "Процент комисии"
})

async function saveSetting(key, value) {
    try {
        await $api.post(`/setting/save`, {
            key: key,
            value: value
        })

        $notice.success('Изменения успешно сохранены')
    }
    catch (error) {
        $notice.handleError(error)
    }
}

async function loadSettingsList() {
    try {
        settings.value = await $api.get(`/setting/list`)
    }
    catch (error) {
        $notice.handleError(error)
    }
}

onMounted(() => {
    loadSettingsList()
})
</script>