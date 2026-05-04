<template lang="pug">
main
    section.search.pd-header.bg-screen
        .container
            Pagetitle(:title="`Поиск мастеров`")
            .work-item.order-info
                .grid
                    .col
                        .param
                            InputMap(label="Адрес объекта" v-model:lat="order.address_lat" v-model:lng="order.address_lng" v-model:address="order.address")
                    .col
                        .param
                            InputMap(label="Место встречи" v-model:lat="order.meeting_point_lat" v-model:lng="order.meeting_point_lng" v-model:address="order.meeting_point")
                TextEditor(label="Как добраться" v-model="order.place")
                .grid
                    .col
                        InputFile(label="Прикрепить файл (необязательно)" v-model="order.place_filelist" name="place_files" accept="image/*,.pdf,.doc,.docx,.xls,.xlsx")
                TextEditor(label="Критерии качества" v-model="order.quality")
                .grid
                    .col
                        InputFile(label="Прикрепить файл (необязательно)" v-model="order.quality_filelist" name="quality_files" accept="image/*,.pdf,.doc,.docx,.xls,.xlsx")
                TextEditor(label="Дополнительная информация" v-model="order.info")
                .grid
                    .col
                        InputFile(label="Прикрепить файл (необязательно)" v-model="order.info_filelist" name="info_files" accept="image/*,.pdf,.doc,.docx,.xls,.xlsx")
            WorkOrder(v-for="(work, index) in works" :key="index" v-model="works[index]" :index="index" @delete="works.splice(index, 1)")
                .btn-wrapper.left.mt-10 
                    button.btn.primary.sm(@click="addWork") Добавить вид работ
            .btn-wrapper.left.mt-10 
                button.btn.primary.sm(@click="addWork") Добавить вид работ
    section.total-price 
        .container 
            h2 Общая стоимость заказа: 
                span {{ totalPrice }} р.
            button.btn.secondary(@click="placeNewOrder") Начать поиск
</template>

<script setup>
import { useAppStore } from '~/store/app'

definePageMeta({
    middleware: ['sanctum:auth', 'role:customer'],
});

const appStore = useAppStore()

const totalPrice = ref(0)
const works = ref([])
const emptyWork = {
    profession_id: '',
    profession_level_id: '',
    count: 1,
    start_date: null,
    end_date: null,
}
const order = ref({
    address_lat: '',
    address_lng: '',
    address: '',
    meeting_point_lat:'',
    meeting_point_lng: '',
    meeting_point: '',
    info: '',
    info_filelist: [],
    quality: '',
    quality_filelist: [],
    place: '',
    place_filelist: [],
})

watchEffect(() => {
    totalPrice.value = 0
    for (const work of works.value) {
        if (!work.profession_id || !work.profession_level_id) continue
        const start = work.start_date ? new Date(work.start_date) : null
        const end = work.end_date ? new Date(work.end_date) : null
        let days = 1
        if (start && end) {
            const diff = (end - start) / (1000 * 60 * 60 * 24)
            days = Math.max(1, Math.round(diff) + 1)
        }
        const price = parseInt(appStore.professionsLevels[work.profession_id].find(item => item.value === work.profession_level_id)?.price) || 0
        totalPrice.value += price * work.count * days
    }
})

watchEffect(() => {
    appStore.orderCounter = works.value.length
})

function addWork() {
    works.value.push({...emptyWork})
}

async function placeNewOrder() {
    try {
        console.log(order.value)
        const { orderId } = await $api.post('/order/place', $api.createFormData({
            ...order.value,
            works: works.value,
        }))
        navigateTo(`/customer/order/${orderId}/search`)
    }
    catch(error) {
        $notice.handleError(error)
        if (error?.response?._data.message.includes('Недостаточно средств')) {
            appStore.showModalBalance = true
        }
    }
}

addWork()
</script>