<template lang="pug">
    .row.db-profession-header(@click="toggleDropdown" :class="{active: active}")
        .col 
            span {{ profession.name }}
        .col
            span Добавлено уровней: {{ profession.levels.length }}
        .col
        .col
            .btn-wrapper.right
                BtnIcon(icon="/images/icons/btn-icon/show.png" @click.stop="emits('select-profession', profession)").mr-10
                BtnIcon(icon="/images/icons/btn-icon/bin.png" @click.stop="emits('delete-profession', profession)").danger
    .db-profession-wrapper(:style="{height: `${height}px`}")
        .db-profession-content(ref="nodeContent" :class="{active: active}")
            .row(v-for="item in profession.levels")
                .col 
                .col 
                    span {{ item.desc }}
                .col 
                    span {{ item.price }} ₽
                .col
                    .btn-wrapper.right
                        BtnIcon(icon="/images/icons/btn-icon/show.png" @click="emits('show-modal', item)").mr-10
                        BtnIcon(icon="/images/icons/btn-icon/bin.png" @click="emits('delete-level', item)").danger
            .row 
                .col 
                .col
                .col
                .col 
                    .btn-wrapper.right
                        button.btn.sm(@click="emits('show-modal', {profession_id: profession.id})") Добавить уровень
         

</template>

<script setup>
const emits = defineEmits(['show-modal', 'delete-profession', 'delete-level', 'select-profession'])
const active = ref(false)
const height = ref(0)
const nodeContent = useTemplateRef('nodeContent')

const props = defineProps({
    profession: {
        type: Object,
        required: true
    }
})

watch(() => props.profession.levels, async (newLevels, oldLevels) => {
    if (newLevels.length !== oldLevels.length) {
        await nextTick(); // Wait for DOM updates
        height.value = nodeContent.value.offsetHeight;
    }
}, { deep: true });

function toggleDropdown() {
    if (active.value) {
        height.value = 0
        active.value = false
        return;
    }

    height.value = nodeContent.value.offsetHeight
    active.value = true     
}

</script>