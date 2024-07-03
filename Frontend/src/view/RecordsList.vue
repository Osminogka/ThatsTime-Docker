<script setup>
import RecordCard from '@/view/RecordCard.vue'
import CustomHideShow from '@/view/CustomHideShow.vue';
import LoadingAnimation from '@/view/LoadingAnimation.vue';

import DateSelector from '@/view/DateSelector.vue';
import { getCertainRecord, getRecordsFromLocal } from '../core/userRecords'
import { todayDate } from '@/core/month';

import { ref, onBeforeMount, defineProps, computed } from 'vue';

const props = defineProps({
    records: Array,
    group: String,
    friend: String,
    all: Boolean
})

const error = ref("");
const loading = ref(false);

onBeforeMount(async () => {
    records.value.find(record => record.showType == 0).records = getRecordsFromLocal(props.records, todayDate.getDate());
    records.value.find(record => record.showType == 7).records = getRecordsFromLocal(props.records, todayDate.getDate() + 7);
    records.value.find(record => record.showType == -7).records = getRecordsFromLocal(props.records, todayDate.getDate() - 7);
});

const records = ref([
    {
        showType: -7,
        records: [],
        isHidden: true
    },
    {
        showType: 0,
        records: [],
        isHidden: false
    },
    {
        showType: 7,
        records: [],
        isHidden: true
    },
    {
        showType: -1,
        records: [],
    }
]);

function getRecordsLocal(date){
    records.value.find(record => record.showType == date).isHidden = !records.value.find(record => record.showType == date).isHidden;
}

function getVisibility(showType){
    return !records.value.find(record => record.showType == showType).isHidden;
}

const selectedDay = ref(null);
const selectedMonth = ref(null);
const selectedYear = ref(null);

const certainError= ref("");
const certainLoading = ref(false);

async function searchRecord(){
    certainLoading.value = true;
    let certainRecords = await getCertainRecord({
        relatedObject: props.friend? props.friend : props.group,
        forYourSelf: props.all,
        isGroup: props.group? true : false,
        year: selectedYear.value,
        month: selectedMonth.value,
        day: selectedDay.value,
    });

    if(certainRecords.success && certainRecords.length == 0){
        records.value.find(record => record.showType == -1).records = [];
        certainLoading.value = false;
        return;
    }

    if(!certainRecords.success){
        certainError.value = "An certainError occured while searching records!";
        certainLoading.value = false;
        return;
    }
    certainError.value = "";
    records.value.find(record => record.showType == '-1').records = certainRecords.records;
    certainLoading.value = false;
}

const lastWeek = computed(() => records.value.find(record => record.showType == -7));
const today = computed(() => records.value.find(record => record.showType == 0));
const nextWeek = computed(() => records.value.find(record => record.showType == 7));
const certain = computed(() => records.value.find(record => record.showType == -1));

const isThereRecentRec = computed(() => 
    lastWeek.value.records.length > 0 || today.value.records.length > 0 || nextWeek.value.records.length > 0
);
</script>

<template>
    <div class="loading-animation" v-if="loading">
        <loading-animation />
    </div>
    <div v-else-if="error">
        <p style="text-align: center;">{{ error }}</p>
    </div>
    <div v-else-if="isThereRecentRec && !error" class="container-records">
            <custom-hide-show :list="lastWeek.records" :showInterface="lastWeek.isHidden"  @showList="getRecordsLocal" :showType="'-7'">
                Last week
            </custom-hide-show>
            <Transition  name="fadey">
                <div v-if="getVisibility('-7')">
                    <div v-if="lastWeek.records.length == 0">No records</div>
                    <div v-else  class="container-week">
                        <record-card v-for="(record, index) in lastWeek.records" :record="record" :key="index"/>
                    </div>
                </div>
            </Transition>
            <custom-hide-show :list="today.records" :showInterface="today.isHidden" @showList="getRecordsLocal" :showType="'0'">
                Today
            </custom-hide-show>
            <Transition  name="fadey">
                <div v-if="getVisibility('0')">
                    <div v-if="today.records.length == 0">No records</div>
                    <div v-else  class="container-week">
                        <record-card v-for="(record, index) in today.records" :record="record" :key="index"/>
                    </div>
                </div>
            </Transition>
            <custom-hide-show :list="nextWeek.records" :showInterface="nextWeek.isHidden" @showList="getRecordsLocal" :showType="'7'">
                Next week
            </custom-hide-show>
            <Transition name="fadey">
                <div v-if="getVisibility('7')">
                    <div v-if="nextWeek.records.length == 0">No records</div>
                    <div v-else  class="container-week">
                        <record-card v-for="(record, index) in nextWeek.records" :record="record" :key="index"/>
                    </div>
                </div>
            </Transition>
    </div>
    <div v-else>
        <p style="text-align: center;">There is no recent records yet!</p>
    </div>
    <div>
        <div class="search-record-box">
            <date-selector v-model="selectedDay" type="day"/>
            <date-selector v-model="selectedMonth" type="month"/>
            <date-selector v-model="selectedYear" type="year"/>
            <button @click="searchRecord" class="search-button custom-button"></button>
        </div> 
        <div style="text-align: center;" v-if="certain.records.length > 0">{{ certain.records.length }} records were found</div>
        <div class="container-week">
            <div v-if="certainLoading">
                <loading-animation/>
            </div>
            <div v-else class="container-week">
                <div v-if="certainError.value">{{ certainError.value }}</div>
                <div v-if="certain.records.length == 0">No records</div>
                <record-card v-for="(record, index) in certain.records" :record="record" :key="index"/>
            </div>
        </div>
    </div>
</template>

<style scoped>

.container-records{
    margin: 0.2em;
}

.container-week{
    display: flex;
    flex-direction: row;
    justify-content: center;
    flex-wrap: wrap;
}


@media  (max-width: 600px) {
    .container-week{
        justify-content: center;
    }
}

.fadey-enter-active, .fadey-leave-active {
    transition: all 0.3s ease-out;
}

.fadey-leave-active {
    transition: all 0.3 ease-out;
}

.fadey-enter-from,
.fadey-leave-to {
    transform: translateY(-50px);
    opacity: 0;
}

.search-record-box{
    display: flex;
    flex-direction: row;
    justify-content: center;
    align-items: center;
}

.search-button{
    background-image: url('../assets/svg/Records/search.svg');
    background-color: white;
    width: 2em;
    height: 2em;
    margin-left: 10px;
    cursor: pointer;
}

.loading-animation{
    display: flex;
    align-items: center;
    justify-content: center;
}

</style>