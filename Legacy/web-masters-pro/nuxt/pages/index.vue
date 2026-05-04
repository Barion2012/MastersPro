<template lang="pug">
main
    section.main-screen.pd-header.bg-screen
        .container
            .text-wrapper 
                h1 Для любой <span class="accent-one">задачи</span> <br> найдётся <span class="accent-two">Masters PRO</span>
                p Сервис, который помогает строительным компаниям найти <br> надежных работников, а мастерам - новые проекты!
                h2(v-if="!isAuthenticated") Кто вы?
                .btn-wrapper
                    NuxtLink.btn.primary(to="/register/master" v-if="!isAuthenticated") Я мастер
                    NuxtLink.btn.secondary(to="/register/customer" v-if="!isAuthenticated") Я Заказчик
                    NuxtLink.btn.secondary(to="/customer/order/create" v-if="isAuthenticated && user?.role === 'customer'") Найти мастера
                    NuxtLink.btn.secondary(to="/worker/dashboard" v-if="isAuthenticated && user?.role === 'worker'") Найти заказ
                    .masters-avatars
                        img(src="/images/layout_elements/masters_avatars.png")
                        span > {{ appStore.stats.countWorkers }} 
                            small мастеров
            .image-wrapper 
                img.image(src="/images/layout_elements/mockup1.png")
                .download-links 
                    a(:href="appStore.downloadLinks.appStore" target="_blank")
                        img(src="/images/icons/app-store.png")
                    a(:href="appStore.downloadLinks.googlePlay" target="_blank")
                        img(src="/images/icons/google-play.png")
    section.stats 
        .container 
            .item 
                b {{ appStore.stats.countWorkers }}
                span Исполнителей <br> по всей России
            .item 
                b {{ appStore.stats.countTasksCompleted }}
                span Выполненных <br> задач
            .item 
                b {{ appStore.stats.countServicesTypes }}
                span Видов <br> услуг
    section.how-it-works#how-it-works(v-if="!isAuthenticated")
        .container 
            .title-wrapper
                h2.title.accent-left Как это работает?
            .wrapper 
                .image-wrapper 
                    img(src="/images/layout_elements/how-it-works.png")
                .text-wrapper
                    .step 
                        .count 1
                        .text 
                            b Регистрация
                            span Регистрируетесь в системе и получаете доступ к личному кабинету
                    .step 
                        .count 2
                        .text 
                            b Гарант
                            span При необходимости заменим исполнителя
                    .step 
                        .count 3
                        .text 
                            b Поиск исполнителей
                            span Выбирайте необходимые услуги. Так же можем предоставить сопутствующий инвентарь от наших партнеров!
                    .step 
                        .count 4
                        .text 
                            b Фин. отчетность
                            span Вся сопутствующая документация и история заказов сохраняется в вашем личном кабинете
    section.faq#faq(v-if="!isAuthenticated")
        .container 
            .title-wrapper.center 
                h2.title.accent-right Часто задаваемые вопросы
            .faq-wrapper
                Dropdown(
                    header="Можно ли выбрать несколько исполнителей?"
                    content="Да. При формировании вашего заказа вы можете выбрать любое интересующее вас кол-во исполнителей и кол-во необходимых смен на объект"
                )
                Dropdown(
                    header="Что делать если мне не подходит исполнитель?"
                    content="Вы можете отказаться от исполнителя по объективным причинам (пришёл в нетрезвом состоянии, излишне опоздал, грубит и ведёт себя не профессионально). Система подберет нового мастера"
                )
                Dropdown(
                    header="Как происходит оплата?"
                    content="Оплата происходит с помощью удержания средств на вашем балансе. Списания происходят ежедневно соразмерно присланным от мастеров отчётам"
                )
                Dropdown(
                    header="Работаете ли вы с физ. лицами?"
                    content="Как заказчик вы можете быть ИП \ ООО. Как мастер вам необходимо быть самозанятым и мы можем вам в этом помочь!"
                )
                Dropdown(
                    header="Как стать исполнителем?"
                    content="Необходимо всего лишь зарегистрироваться нажав на кнопку \"Я мастер\". После прохождения регистрации вы сможете получать заказы. "
                )
                Dropdown(
                    header="У исполнителей имеется своя экипировка и инструмент?"
                    content="У некоторых исполнителей имеется свой инвентарь. Об этом будет указано."
                )
    section.become-partner(v-if="!isAuthenticated")
        .container 
            .card-wrapper 
                .card 
                    h3.card-title Стать исполнителем 
                    ul.list.primary
                        li Получайте новые заказы с помощью данной системы
                        li Ежедневные выплаты с выполненных заказов
                        li Добавляйте новые услуги для участия в поиске
                        li Можете воспользоватья арендованным оборудованием и экипировкой
                    .btn-wrapper.center
                        NuxtLink(to="/register/master").btn.primary Регистрация 

                .card 
                    h3.card-title Стать заказчиком
                    ul.list.secondary
                        li Система автоматически подберет вам необходимых исполнителей
                        li Предложим необходимое оборудование для работников и доставим до объекта
                        li Возможность нанять работников на длительное время
                    .btn-wrapper.center
                        NuxtLink(to="/register/customer").btn.secondary Регистрация
    section.banner-mobile 
        .container 
            .banner
                .text-wrapper
                    h3 Скачайте приложение <br> и пользуйтесь Masters PRO <span class="accent">где угодно</span>
                    .btn-wrapper.left 
                        a(:href="appStore.downloadLinks.appStore" target="_blank")
                            img(src="/images/icons/app-store.png")
                        a(:href="appStore.downloadLinks.googlePlay" target="_blank")
                            img(src="/images/icons/google-play.png")



</template>

<script setup>
import { useAppStore } from '~/store/app'

definePageMeta({
  layout: 'site'
})

const appStore = useAppStore()
const { isAuthenticated } = useSanctumAuth()
const user = useSanctumUser()
</script>

<style scoped>

</style>