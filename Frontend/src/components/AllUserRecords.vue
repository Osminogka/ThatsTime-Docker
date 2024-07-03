<script setup>
import RecordList from '@/view/RecordsList.vue';
import LoadingAnimation from '@/view/LoadingAnimation.vue';

import { getRecords } from '../core/userRecords';
import { todayDate } from '@/core/month';

import { ref,onMounted } from 'vue';

const records = ref([]);
const loading = ref(false);
const error = ref(null);

onMounted(async () => {
    loading.value = true;
    let response = await getRecords({
        year: todayDate.getFullYear(),
        month: todayDate.getMonth() + 1,
        day: todayDate.getDate()
    });
    if(response.success){
        records.value = response.records;
    }
    else{
        error.value = "An error occured while getting records!";
    }
    loading.value = false;
});

</script>

<template>
    <LoadingAnimation v-if="loading"/>
    <record-list v-else :all="true" :records="records"/>    
</template>