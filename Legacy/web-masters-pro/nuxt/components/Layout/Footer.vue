<template lang="pug">
    footer 
        .container 
            .info 
                .btn-wrapper.between 
                    NuxtLink(to="/")
                        img(src="/images/logo_alt.png")
                    BtnLogin
                .row 
                    b © Copyright Masters PRO 2025
                .row 
                    //a(href="#" target="_blank").indev «Masters PRO» осуществляет деятельность в области информационных технологий. Вид деятельности (код): 2.01.
                    a(href="/files/docs/consent_personal_data.docx" external) Согласие на обработку персональных данных
                    a(href="/files/docs/consent_cookie.docx" target="_blank") Согласие на обработку Cookie
                    a(href="/files/docs/personal_data_processing_policy.docx" target="_blank") Политика Обработки персональных данных
                    a(href="/files/docs/user_agreement_on_the_use_of_the_software.docx" target="_blank")  Пользовательское соглашение об использовании ПО
            .menu-wrapper
                .menu 
                    //NuxtLink(to="/").indev Поставщикам
                    //NuxtLink(to="/").indev Стать исполнителем
                    //NuxtLink(to="/").indev Помощь
                    //NuxtLink(to="/").indev Как оформить заказ
                    //NuxtLink(to="/").indev Замена мастера
                .menu 
                    //NuxtLink(to="/").indev О Masters PRO
                    //NuxtLink(to="/").indev Вакансии
                    //NuxtLink(to="/").indev Реквизиты
                .info-text 
                    span ООО «Кью Лаб»
                    span Адрес местонахождения: 117452, г. Москва, вн.тер.г. муниципальный округ Зюзино, б-р Черноморский, д. 17, к. 1, помещ. 5/3/5
                .info-text 
                    span ИНН / КПП: 9727098700/ 772701001
                    span ОГРН: 1257700049529
                    span Р/С 40702810620000181117
                .info-text 
                    span в ООО «Банк Точка»
                    span БИК: 044525104
                    span К/С: 30101810745374525104
</template>