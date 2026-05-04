<template lang="pug">
    section.profile.pd-header.bg-screen.screen
        .container 
            .card.user-modal
                span.card-title Профиль
                .info-wrapper
                    h4 Общая информация
                    .info
                        b Заказчик:
                        span {{ user.name }}
                    .info
                        b Почта:
                        span {{ user.email }}
                    .info
                        b Телефон:
                        span {{ user.phone }}
                    .info 
                        b Тип:
                        div 
                            span.tag.green(v-if="user.customer.type === 'ip'") ИП
                            span.tag.blue(v-else-if="user.customer.type === 'ooo'") ООО
                            span.tag.cyan(v-else-if="user.customer.type === 'ao'") АО
                            span.tag.yellow(v-else-if="user.customer.type === 'pao'") АО
                            span.tag(v-else) Прочее 
                    .info-wrapper
                        h4 Реквизиты
                        .info
                            b Название банка:
                            span {{ user.customer.bank_name }}
                        .info
                            b Номер расчетного счета:
                            span {{ user.customer.account_number }}
                        .info 
                            b Номер кореспондентского счета:
                            span {{ user.customer.bank_cor_account }} 
                        .info 
                            b БИК банка:
                            span {{ user.customer.bank_bic }} 
</template>

<script setup>
import { useAppStore } from '~/store/app'

const user = useSanctumUser()

</script>