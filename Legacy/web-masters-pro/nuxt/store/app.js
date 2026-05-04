import { defineStore } from 'pinia';

export const useAppStore = defineStore('app', {
    state: () => ({
        stats: {
            countWorkers: '150к',
            countTasksCompleted: '1318',
            countServicesTypes: '256'
        },
        downloadLinks: {
            googlePlay: 'https://play.google.com/',
            appStore: 'https://www.apple.com/app-store/'
        },
        professions: [],
        professionsLevels: [],
        orderCounter: 0,
        showModalBalance: false
    }),
    actions: {
        async initStore() {
            await this.getProfessions()
        },
        async getProfessions() {
            try {
                const {professions, levels} = await $api.get('/profession/list/select')
                this.professions = professions
                this.professionsLevels = levels
            }
            catch (error) {
                $notice.handleError(error)
            }
        },
    }
})