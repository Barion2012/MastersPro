<template lang="pug">
.tab-switcher-wrapper
    .tab-switcher 
        .btn-tab(v-for="tab in tabs" :class="{active: model === tab.value}" @click="model = tab.value")
            span {{ tab.title }}
</template>

<script setup>
const props = defineProps(['tabs'])
const model = defineModel()
</script>