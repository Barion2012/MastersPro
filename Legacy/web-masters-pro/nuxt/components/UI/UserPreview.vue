<template lang="pug">
.user-preview 
    span.name {{ name }}
    RoleTag(:role="role")
    a.phone(:href="`tel:${phone}`" v-if="checkPermission('User.view-contacts')")
        BtnIcon(icon="/images/icons/btn-icon/phone.png")
</template>

<script setup>
const { checkPermission } = $api;
const props = defineProps({
    name: {
        type: String,
        default: ''
    },
    role: {
        type: String,
        default: ''
    },
    phone: {
        type: String,
        default: ''
    }
})
</script>