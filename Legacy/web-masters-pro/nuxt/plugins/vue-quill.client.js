//import 'quill/dist/quill.core.css' 
import '@vueup/vue-quill/dist/vue-quill.snow.css'

import { QuillEditor } from '@vueup/vue-quill'

export default defineNuxtPlugin((nuxtApp) => {
    nuxtApp.vueApp.component('QuillEditor', QuillEditor)
})