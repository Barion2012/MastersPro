export default defineNuxtPlugin((_nuxtApp) => {
    const { $sanctumClient } = _nuxtApp;
    const { login, logout, refreshIdentity } = useSanctumAuth()

    globalThis.$api = {
        get(url) {
            return $sanctumClient('/api' + url)
        },
        post(url, body) {
            return $sanctumClient('/api' + url, {
                method: 'POST',
                body: body
            })
        },
        delete(url, body) {
            return $sanctumClient('/api' + url, {
                method: 'DELETE',
                body: body
            })
        },
        login(credentials) {
            return login(credentials)
        },
        async loginBySmsCode(credentials) {
            await $sanctumClient('/api/auth/sms/login', {
                method: 'POST',
                body: credentials
            })
            await refreshIdentity()
        },
        logout() {
            return logout()
        },
        createFormData(obj) {
            let formData = new FormData()
            
            for (const key of Object.keys(obj)) {
                if (obj[key] === null || obj[key] === undefined)
                    continue
                if (Array.isArray(obj[key])) {
                    if (obj[key][0] instanceof File) {
                        for (const file of obj[key]) {
                            formData.append(`${key}[]`, file)
                        }
                        continue
                    } else if (typeof obj[key][0] === 'object') {
                        obj[key].forEach((item, index) => {
                            for (const subKey in item) {
                                formData.append(`${key}[${index}][${subKey}]`, item[subKey])
                            }
                        })
                        continue
                    }
                }
                formData.append(key, obj[key])
            }

            return formData
        },
        checkPermission(permissionCode) {
            let permission = permissionCode.split('.')
            let model = permission[0]
            let action = permission[1]

            return useSanctumUser().value?.permissions[model]?.includes(action)
        }
    };
});