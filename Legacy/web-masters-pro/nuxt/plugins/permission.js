export default defineNuxtPlugin(async (_nuxtApp) => {
    let permissions = {}

    try {
        permissions = await $api.get('/permission/list')
    }
    catch(e) {
        console.log(e)
    }

    for (const model of Object.keys(permissions)) {
        for (const action of permissions[model]) {
            addRouteMiddleware(`permission:${model}.${action}`, (to, from) => {
                const user = useSanctumUser().value
                if (user === null)
                    return navigateTo('/login')

                if (!user?.permissions[model]?.includes(action)) {
                    throw createError({
                        statusCode: 403,
                        fatal: true,
                        message: 'Доступ запрещен',
                        unhandled: true,
                     })
                }
                    
            })
        }
    }

    const rolesList = ['admin', 'user', 'superuser', 'worker', 'customer']

    for (const role of rolesList) {
        addRouteMiddleware(`role:${role}`, (to, from) => {
            const user = useSanctumUser().value

            if (user === null)
                return navigateTo('/login')

            if (user?.role === 'superuser') {
                return
            }
    
            if (user?.role !== role) {
                throw createError({
                    statusCode: 403,
                    fatal: true,
                    message: 'Доступ запрещен',
                    unhandled: true,
                })
            }
        })
    }

    addRouteMiddleware('role:administration', (to, from) => {
        const user = useSanctumUser().value

        if (user === null)
            return navigateTo('/login')

        if (user?.role === 'superuser' || user?.role === 'admin' || user?.role === 'moderator' || user?.role === 'manager') {
            return
        }
        else {
            throw createError({
                statusCode: 403,
                fatal: true,
                message: 'Доступ запрещен',
                unhandled: true,
            })
        }

    })
})