<template lang="pug">
header.app-header
    .container 
        NuxtLink(to="/").logo
            img(src="/images/logo.png")
        .user-info(v-if="user?.id")
            .balance-wrapper(v-if="user?.role === 'customer'")
                .balance
                    span Баланс:
                    span.green {{ user?.customer?.balance }} р.
                small(v-if="user?.customer?.hold_balance") Удержано {{ user?.customer?.hold_balance }} р.
                a.link(@click.prevent="appStore.showModalBalance = true") Пополнить

            .balance-wrapper(v-else-if="user?.role === 'worker'")
                //.balance
                    span Баланс:
                    span.green {{ user?.worker?.balance }} р.
            NuxtLink(to="/customer/order/create" v-if="user?.role === 'customer'").cart
                span Мой заказ
                .icon
                    .counter {{ appStore.orderCounter }}
                    img(src="/images/icons/cart.png")
        .menu(v-else)
        NotificationCenter(v-if="user?.id")
        .btn-wrapper 
            BtnLogin
        MobileMenu
        ModalBalance
</template>

<script setup>
import { useAppStore } from '~/store/app'

const user = useSanctumUser()
const appStore = useAppStore()
</script>