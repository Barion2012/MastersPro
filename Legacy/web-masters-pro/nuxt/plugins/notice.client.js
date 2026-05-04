import { toast } from "vue3-toastify";
import "vue3-toastify/dist/index.css";

export default defineNuxtPlugin((_nuxtApp) => {
    globalThis.$notice = {
        info(text) {
            toast(text, {
                "type": "info",
                "theme": "light",
                "autoClose": 5000,
                "transition": "slide"
            })
        },
        error(text) {                           
            toast(text, {
                "type": "error",
                "theme": "light",
                "autoClose": 5000,
                "transition": "slide"
            })
        },
        success(text) {
            toast(text, {
                "type": "success",
                "theme": "light",
                "autoClose": 5000,
                "transition": "slide"
            })
        },
        warning(text) {
            toast(text, {
                "type": "warning",
                "theme": "light",
                "autoClose": 5000,
                "transition": "slide"
            })
        },
        handleError(error) {
            if (error?.response?.status === 500) {
                console.error(error);
                return this.error('Произошла непредвиденная ошибка на стороне сервера')
            }
            if (error?.response?._data.message) {
                console.error(error.response._data.message)
                return this.error(error.response._data.message.replace(/\(and \d+ more errors?\)/g, ''))
            }
            console.log(error)
            return this.error('Неизвестная ошибка')
        }
    }
})