<template lang="pug">
.mobile-menu
    .btn-menu(@click="isOpen = true")
        span 
    .menu-wrapper(:class="{ 'active': isOpen }")
        .btn-close(@click="isOpen = false")
        BtnLogin(@click="isOpen = false")
        .menu-item(v-for="item in menu" :key="item.text")
            Anchor(:href="item.href" v-if="item.type === 'anchor'" @click="isOpen = false") {{ item.text }}
            NuxtLink(:to="item.href" v-else)
                span(@click="isOpen = false") {{ item.text }}
</template>

<script setup>
const isOpen = ref(false)
const props = defineProps(['menu'])


</script>