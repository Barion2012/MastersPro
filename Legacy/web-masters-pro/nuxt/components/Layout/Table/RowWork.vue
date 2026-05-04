<template lang="pug">
    .row.work 
        .col
            span {{ index }}
        .col 
            span {{ work.profession }}
        .col 
            span {{ work.count }}
        .col
            span {{ work.found }}
        .col 
            span {{ work.refused }}
        .col
            span {{ work.price_shift }}
        .col 
            span.tag.blue(v-if="work.status === 'search'") В поиске
            span.tag.cyan(v-else-if="work.status === 'work'") В работе
            span.tag.green(v-else-if="work.status === 'done'") Поиск завершен
            span.tag.red(v-else-if="work.status === 'canceled'") Отмен
</template>

<script setup>
const props = defineProps(['index', 'work'])
</script>