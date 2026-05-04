<template lang="pug">
.tab(:class="{ active: props.activeTab === props.name }")
    .tab-content(ref="tab-content")
        slot
</template>

<script setup>
const props = defineProps({
    name: {
        type: String,
        default: ''
    },
    activeTab: {
        type: String,
        default: ''
    }
})

const model = defineModel()

const tabContent = useTemplateRef('tab-content')
const tabHeight = ref(0)

watchEffect(() => {
    if (props.activeTab === props.name)
        showTab()
}, {flush: 'post'})

function showTab() {
    // Ensure the template ref is available before reading layout properties.
    // Use a microtask to wait for DOM updates and guard against null refs.
    Promise.resolve().then(() => {
        if (tabContent && tabContent.value)
            model.value = tabContent.value.clientHeight
        else
            model.value = 0
    })
}

function checkTab() {
    if (props.activeTab === props.name) 
        showTab()
}

onMounted(() => {
    checkTab()
})
</script>