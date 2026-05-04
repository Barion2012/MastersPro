<template lang="pug">
main 
    section.page-stats.pd-header.pd-bottom.bg-screen.screen
        .container 
            Pagetitle(title="Статистика" :goback="true")
            .statistic-wrapper 
                .grid-stats
                    .statistic-item 
                        .wrapper.border.blue
                            b Заказов оформлено
                            span {{ stats.orders }} 
                    .statistic-item
                        .wrapper.border.indigo
                            b Заказов завершено
                            span {{ stats.completed }}
                    .statistic-item
                        .wrapper.border.purple
                            b Заморожено средств 
                            span {{ stats.hold_balance }}
                    .statistic-item
                        .wrapper.border.pink
                            b Выплачено мастерам 
                            span {{ stats.paid }}
                    .statistic-item
                        .wrapper.border.red
                            b Мастеров в системе
                            span {{ stats.workers }}
                    .statistic-item
                        .wrapper.border.orange
                            b Заказчиков в системе
                            span {{ stats.customers }}
                .chart-wrapper
                    ClientOnly
                        .chart
                            Line(:data="chartData" :options="chartOptions" ref="line" v-if="loaded")
</template>

<script setup>
definePageMeta({
    middleware: ['sanctum:auth', 'role:administration'],
});

import {
    Chart as ChartJS,
    CategoryScale,
    LinearScale,
    PointElement,
    LineElement,
    Title,
    Tooltip,
    Legend,
    Filler,
    plugins,
} from 'chart.js'
import { Line } from 'vue-chartjs'

ChartJS.register(
    CategoryScale,
    LinearScale,
    PointElement,
    LineElement,
    Title,
    Tooltip,
    Legend,
    Filler
)

const chartData = ref({
    labels: ['Янв', 'Фев', 'Март', 'Апр', 'Май', 'Июнь', 'Июль', 'Авг', 'Сен', 'Окт', 'Нояб', 'Дек'],
    datasets: [
        {
            label: 'Кол-во заказов',
            backgroundColor: 'rgb(34, 120, 233, 0.2)',
            borderColor: '#2278e9',
            borderWidth: 2,
            pointBackgroundColor: '#fff',
            pointRadius: 4,
            pointBorderWidth: 2,
            pointBorderColor: '#2278e9',
            data: [40, 39, 10, 40, 39, 80, 40, 40, 39, 10, 40, 39],
            fill: true,
            tension: 0.1
        }
    ]
})

const chartOptions = ref({
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
        legend: {
            display: false
        }
    },
    scales: {
        y: {
            grid: {
                display: false // Отключает полоски (grid lines) по оси Y
            }
        },
    }
})

const stats = ref({
    orders: 0,
    completed: 0,
    hold_balance: 0,
    paid: 0,
    workers: 0,
    customers: 0
})

const line = useTemplateRef('line')
const loaded = ref(false)

async function getStatistics() {
    try {
        stats.value = await $api.get(`/stats/get`)
        if (stats.value.orders_per_month) {
            chartData.value.datasets[0].data = stats.value.orders_per_month
        }
        loaded.value = true
    }
    catch (error) {
        $notice.handleError(error)
    }
}

onMounted(() => {
    getStatistics()
})
</script>