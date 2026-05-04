<template lang="pug">
.paginator(v-if="lastPage != 1" )
    .wrapper
        button.item(v-for="item in pages" :class="{current: item == currentPage}" @click="emit('change-page', item)") {{item}}
        .item.disable(v-if="(currentPage + 2 < lastPage) && lastPage > 5") ...
        button.item(v-if="currentPage + 2 < lastPage && lastPage > 5" @click="emit('change-page', lastPage)") {{lastPage}}
</template>

<script setup>
const { lastPage, currentPage } = defineProps(['lastPage', 'currentPage'])
const emit = defineEmits(['change-page'])
const pages = ref([])

watch(() => lastPage, () => calcPages())
watch(() => currentPage, () => calcPages())

function calcPages() {
    pages.value = Array.from({length: lastPage}, (_, i) => i + 1);

    let startIndex = currentPage;

    if (lastPage < 5)
        return;

    if (currentPage - 1 > 0)
        startIndex = currentPage - 1;
    if (currentPage - 2 > 0)
        startIndex = currentPage - 2;
    if (currentPage + 1 > lastPage)
        startIndex = startIndex - 1;
    if (currentPage + 2 > lastPage)
        startIndex = startIndex - 1;

    pages.value = pages.value .slice(startIndex - 1, startIndex + 4);
}

calcPages()
</script>

<style lang="scss">

</style>