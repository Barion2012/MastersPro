<template lang="pug">
main 
    section.dashdoard.pd-header.bg-screen.screen
        .container(v-if="activeWork.work_id")
            .wait-wrapper(v-if="!activeWork.current_work_shift")
                .app-title-wrapper
                        h1.app-title Принятое предложение
                .jobs-list 
                    WorkItem(:order="activeWork")
                        button.btn.primary.sm.danger(@click="refuseWorkAddress") Отказаться от работы
            .confirm-address(v-else-if="!activeWork.current_work_shift.confirm_address")
                .app-title-wrapper
                    h1.app-title Необходимо подтвердить выход на объект
                    .app-subtitle Если вы не подтвердите выход в течении <span class="secondary">20 часов</span>, то произойдет отказ от работы
                .jobs-list 
                    WorkItem(:order="activeWork")
                        button.btn.primary.sm.danger.mr-10(@click="refuseWorkAddress") Отказаться от работы
                        button.btn.primary.sm(@click="showModalContract = true") Подтверждаю
            .confirm-meeting(v-else-if="!activeWork.current_work_shift.confirm_meeting")
                .app-title-wrapper
                    h1.app-title Сообщите, что вы достигли адреса встречи
                    .app-subtitle Если вы не подтвердите в течении <span class="secondary">15 минут</span>, то произойдет отказ от работы
                .works-list 
                    WorkItem(:order="activeWork")
                        button.btn.primary.sm(@click="acceptWorkShiftMeeting") Я на месте
            .active-work(v-else-if="activeWork.current_work_shift.confirm_address && activeWork.current_work_shift.confirm_meeting && activeWork.status != 'accepted'")
                .app-title-wrapper
                    h1.app-title Ждем потверждения от заказчика
                .works-list
                    WorkItem(:order="activeWork")
                        button.btn.primary.sm.mr-10(@click="showModalMeet = true" v-if="activeWork.current_work_shift.not_met") Меня не встретили
            .active-work(v-else-if="activeWork.current_work_shift.confirm_address && activeWork.current_work_shift.confirm_meeting && activeWork.status == 'accepted'")
                .app-title-wrapper
                    h1.app-title Активные объекты
                    .app-subtitle После окончания смены вам необходимо сделать фото своей работы и загрузить их
                .works-list
                    WorkReport(:order="activeWork" @send-report="getActiveWork")
                    WorkItem(:order="activeWork")
                        button.btn.secondary.sm(@click="showModalDoc = true") Показать документы
        .container.mt-10
            .btn-wrapper.right.mb-10
                BtnLogout
        Modal(v-model="showModalDoc").user-modal
            span.modal-title Документы
            .files
                .info(v-if="activeWork.files.passport_scan")
                    b Паспорт 
                    a(:href="`/api/order/${activeWork.order_id}/worker/${user.worker.id}/file/${activeWork.files.passport_scan}/get`" target="_blank").link Скачать
                .info(v-if="activeWork.files.snils")
                    b СНИЛС 
                    a(:href="`/api/order/${activeWork.order_id}/worker/${user.worker.id}/file/${activeWork.files.snils}/get`" target="_blank").link Скачать
                .info(v-if="activeWork.files.migration_card")
                    b Миграционная карта 
                    a(:href="`/api/order/${activeWork.order_id}/worker/${user.worker.id}/file/${activeWork.files.migration_card}/get`" target="_blank").link Скачать
                .info(v-if="activeWork.files.patent")
                    b Патент
                    a(:href="`/api/order/${activeWork.order_id}/worker/${user.worker.id}/file/${activeWork.files.patent}/get`" target="_blank").link Скачать
                .info(v-if="activeWork.files.dms")
                    b ДМС
                    a(:href="`/api/order/${activeWork.order_id}/worker/${user.worker.id}/file/${activeWork.files.dms}/get`" target="_blank").link Скачать
        Modal(v-model="showModalMeet").user-modal
            span.modal-title Вы уверены, что вас не встретили? 
            .btn-wrapper.center
                button.btn.danger.sm.mr-10(@click="workerNotMet") Меня не встретили
                button.btn.primary.sm(@click="showModalMeet = false") Отмена
        Modal(v-model="showModalContract").user-modal
            span.modal-title(style="margin-bottom: 15px") Подтверждая выход на объект, вы соглашаетесь с условиями договора
            p(style="margin-bottom: 35px; text-align: center;") Ознакомиться с договором можно по 
                a(:href="`/api/order/${activeWork.order_id}/worker/${user.worker.id}/contract/get`" target="_blank" style="font-size: 16px;").link ссылке
            .btn-wrapper.center
                button.btn.primary.sm(@click="acceptWorkShiftAddress") Подтверждаю
        SupportWidget
</template>

<script setup>
definePageMeta({
    middleware: ['sanctum:auth', 'role:worker'],
});

const activeWork = ref({})
const showModalDoc = ref(false)
const showModalMeet = ref(false)
const showModalContract = ref(false)
const user = useSanctumUser()
const channelName = ref('')
async function getActiveWork() {
    try {
        activeWork.value = await $api.get('/work/active')
    }
    catch (error) {
        $notice.handleError(error)
    }
}

async function acceptWorkShiftAddress() {
    try {
        await $api.post(`/work/shift/${activeWork.value.current_work_shift.id}/address/accept`)
        await checkAcceptWork()
        showModalContract.value = false
    }
    catch (error) {
        $notice.handleError(error)
    }
}

async function refuseWorkAddress() {
    try {
        await $api.post(`/work/${activeWork.value.work_id}/refsuse`)
        navigateTo('/worker/dashboard')
    }
    catch (error) {
        $notice.handleError(error)
    }
}

async function acceptWorkShiftMeeting() {
    try {
        await $api.post(`/work/shift/${activeWork.value.current_work_shift.id}/meeting/accept`)
        await checkAcceptWork()
    }
    catch (error) {
        $notice.handleError(error)
    }
}

async function workerNotMet() {
    try {
        await $api.post(`/work/${activeWork.value.work_id}/not-met`)
        navigateTo('/worker/dashboard')
    }
    catch (error) {
        $notice.handleError(error)
    }
}

function listenNotifications() {
    $echo.private(channelName.value)
        .listen('WorkerRefused', (e) => {
            navigateTo('/worker/dashboard')
        })
        .listen('WorkShiftUpdate', (e) => {
            checkAcceptWork()
        })
}

checkAcceptWork()

onMounted(() => {
    channelName.value = `user-notification.${user.value.id}`;
    listenNotifications()
})

async function checkAcceptWork() {
    try {
        const {workId} = await $api.get(`/work/check/accept`)
        if (workId)
            await getActiveWork()
        else
            navigateTo('/worker/job/search')
    }
    catch (error) {
        $notice.handleError(error)
    }
}

onBeforeUnmount(() => {
    $echo.leave(channelName.value);
})
</script>