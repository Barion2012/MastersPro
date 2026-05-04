<template lang="pug">
    Modal(v-model="appStore.showModalBalance").balance-modal
        span.modal-title Пополнение баланса
        .balace-wrapper 
            span К оплате
            label Введите сумму для пополнения
            .input-wrapper
                input(v-model="money" type="number" min="0")
                img(src="/images/icons/001-wallet.png").icon
            small Минимальная сумма пополнения 1 000 р.
        .btn-wrapper.center
            button.btn.secondary Перейти к оплате
</template>

<script setup>
import { useAppStore } from '~/store/app'

const appStore = useAppStore()
const money = ref(1000)
</script>

<style lang="scss">
.balance-modal {
    .balace-wrapper {
        span {
            font-family: "Montserrat";
            margin-bottom: 5px;
            font-weight: 600;
            display: block;
        }

        label {
            font-weight: 400;
            font-size: 15px;
            margin-bottom: 10px;
            display: block;
        }

        input {
            border: none;
            padding-left: 40px;
        }

        .input-wrapper {
            border: 1px solid #c5c5c5;
            border-radius: 5px;
            margin-bottom: 10px;
            position: relative;
            

            .icon {
                position: absolute;
                left: 10px;
                top: 0;
                bottom: 0;
                margin: auto;
            }
        }

        small {
            margin-bottom: 20px;
            display: block;
        }
    }
}
</style>
