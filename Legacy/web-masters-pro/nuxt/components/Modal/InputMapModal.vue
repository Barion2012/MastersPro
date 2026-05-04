<template lang="pug">
Modal(v-model="showModal").map-modal
    span.modal-title Укажите адрес
    .address-wrapper
        .input-address
            Input(v-model="address" placeholder="Введите адрес" @input="searchAddress")
            .address-suggestions(v-if="loadAddresses.length > 0")
                .address-suggestion(v-for="item in loadAddresses" :key="item.GeoObject.metaDataProperty.GeocoderMetaData.id" @click.prevent="setPoint(item)")
                    span {{ item.GeoObject.metaDataProperty.GeocoderMetaData.text }}
        button.btn.primary.sm(@click.prevent="confirmAddress") Подтвердить
    YandexMap(
        height="100%"
        width="100%"
        :settings="{ location: { center, zoom, theme: 'dark', showScaleInCopyrights: true } }"
        v-model="map"
        :class="{drag: onDrag}"
    ).ymap
        YandexMapDefaultSchemeLayer
        YandexMapDefaultFeaturesLayer
        YandexMapListener( :settings="{onActionStart: () => {onDrag = true}, onActionEnd: HandlerDragEnd}" )
    
</template>

<script setup>
import {
    YandexMap,
    YandexMapDefaultFeaturesLayer,
    YandexMapDefaultSchemeLayer,
    YandexMapListener,
} from 'vue-yandex-maps';

const config = useRuntimeConfig();

const showModal = defineModel('show')
const modelLat = defineModel('lat')
const modelLng = defineModel('lng')
const modelAddress = defineModel('address')

const map = shallowRef(null)
const center = ref([37.618910773456065, 55.751319219261056])
const zoom = ref(9)
const onDrag = ref(false)
const address = ref('')
const loadAddresses = ref([]);

async function HandlerDragEnd() {
    onDrag.value = false
    try {
        address.value = await $fetch(`https://geocode-maps.yandex.ru/v1/?apikey=${config.public.YANDEX_MAPS_API_KEY}&geocode=${map.value.center[0]},${map.value.center[1]}&format=json`)

        address.value = address.value.response.GeoObjectCollection.featureMember['0']['GeoObject']['metaDataProperty']['GeocoderMetaData']['text']
        modelAddress.value = address.value;
        modelLat.value = map.value.center[1];
        modelLng.value = map.value.center[0];
    }
    catch(error) {
        $notice.handleError(error)
    }
}

function confirmAddress() {
    //modelAddress.value = address.value;
    modelLat.value = map.value.center[1];
    modelLng.value = map.value.center[0];
    showModal.value = false;
}

function setPoint(item) {
    modelAddress.value = address.value = item.GeoObject.metaDataProperty.GeocoderMetaData.text; 
    modelLat.value = item.GeoObject.Point.pos.split(' ')[1]; 
    modelLng.value = item.GeoObject.Point.pos.split(' ')[0]; 
    loadAddresses.value = [];
    center.value = [item.GeoObject.Point.pos.split(' ')[0], item.GeoObject.Point.pos.split(' ')[1]];
    zoom.value = 15
}

async function searchAddress() {
    try {
        if(!address.value) return;

        const fetchAddress = await $fetch(`https://geocode-maps.yandex.ru/v1/?apikey=${config.public.YANDEX_MAPS_API_KEY}&geocode=${encodeURIComponent(address.value)}&format=json&results=5`)

        console.log(fetchAddress.response.GeoObjectCollection.featureMember);

        if (fetchAddress.response?.GeoObjectCollection?.featureMember.length > 0) {
            loadAddresses.value = fetchAddress.response.GeoObjectCollection.featureMember;
        }
    }
    catch(error) {
        $notice.handleError(error)
    }
}

</script>