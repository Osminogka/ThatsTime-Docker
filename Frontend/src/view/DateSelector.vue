<script setup>
import { defineProps, ref, computed, defineModel, onMounted } from 'vue';
import { todayDate } from '@/core/month';
import { shortMonthNames } from '@/core/month';

const props = defineProps({
    type: String
});

const model = defineModel();

const daysInSelectedMonth = computed(() => {
    const selectedMonth = shortMonthNames.indexOf(model.value);
    const selectedYear = model.value;
    const lastDayOfMonth = new Date(selectedYear, selectedMonth + 1, 0).getDate();
    return lastDayOfMonth;
});

onMounted(() =>{
    if(props.type === 'day'){
        model.value = todayDate.getDate();
    } else if(props.type === 'month'){
        model.value = shortMonthNames[todayDate.getMonth()];
    } else if(props.type === 'year'){
        model.value = todayDate.getFullYear();
    }
})

function prevDay(){
    if(model.value > 1)
        model.value--;
    else
        model.value = daysInSelectedMonth.value;
}

function nextDay(){
    if(model.value < daysInSelectedMonth.value)
        model.value++;
    else
        model.value = 1;
}

function nextMonth(){
    const index = shortMonthNames.indexOf(model.value);
    if(index < 11)
        model.value = shortMonthNames[index + 1];
    else
        model.value = shortMonthNames[0];
}

function prevMonth(){
    const index = shortMonthNames.indexOf(model.value);
    if(index > 0)
        model.value = shortMonthNames[index - 1];
    else
        model.value = shortMonthNames[11];
}

function nextYear(){
    if(model.value < todayDate.getFullYear() + 10)
        model.value++;
    else
        model.value = todayDate.getFullYear() - 10;
}

function prevYear(){
    if(model.value > todayDate.getFullYear() - 10)
        model.value--;
    else
        model.value = todayDate.getFullYear() + 10;
}

let timer = ref(null);

function startAutoDay(isNextDayCalled) {
    let func = () => isNextDayCalled? nextDay() : prevDay();
    func();
    
    timer.value = setInterval(() => {func()}, 200); // Call increment every 500ms
}
function stopAutoDay() {
    clearInterval(timer.value);
}

function startAutoMonth(isNextMonthCalled) {
    let func = () => isNextMonthCalled? nextMonth() : prevMonth();
    func();
    timer.value = setInterval(() => {func()}, 200);
}
function stopAutoMonth() {
    clearInterval(timer.value);
}

function startAutoYear(isNextYearCalled) {
    let func = () => isNextYearCalled? nextYear() : prevYear();
    func();
    timer.value = setInterval(() => {func()}, 200);
}
function stopAutoYear() {
    clearInterval(timer.value);
}

</script>

<template>
    <div class="container-date-select">
        <input v-if="type === 'day'" v-model="model" class="record-date-selector" placeholder="DD" type="number"/>
        <div v-else-if="type === 'month'" class="record-date-selector" placeholder="MM" type="text">{{ model }}</div>
        <div v-else-if="type === 'year'" class="record-date-selector" placeholder="YYYY" type="number">{{ model }}</div>
        <div class="container-record-button" v-if="type === 'day'">
            <button @mousedown="startAutoDay(true)" @mouseup="stopAutoDay" class="up-button custom-button"></button>
            <button  @mousedown="startAutoDay(false)" @mouseup="stopAutoYear" class="down-button custom-button"></button>
        </div>
        <div class="container-record-button" v-else-if="type === 'month'">
            <button @mousedown="startAutoMonth(true)" @mouseup="stopAutoMonth" class="up-button custom-button"></button>
            <button @mousedown="startAutoMonth(false)" @mouseup="stopAutoYear" class="down-button custom-button"></button>
        </div>
        <div class="container-record-button" v-else-if="type === 'year'">
            <button @mousedown="startAutoYear(true)" @mouseup="stopAutoYear" class="up-button custom-button"></button>
            <button @mousedown="startAutoYear(false)" @mouseup="stopAutoYear" class="down-button custom-button"></button>
        </div>
    </div>
</template>

<style scoped>

.up-button{
    background-image: url('../assets/svg/Records/uparrow.svg');
    background-color: white;
    width: 1.5em;
    height: 1.5em;
}

.down-button{
    background-image: url('../assets/svg/Records/downarrow.svg');
    background-color: white;
    width: 1.5em;
    height: 1.5em;
}

.container-record-button{
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    margin: 10px;
}

.container-record-button > * {
  display: block; /* Add this line */
}

.container-date-select{
    display: flex;
    flex-direction: row;
    justify-content: center;
    align-items: center;
}

.record-date-selector{
    background-color: #55FFEF;
    border-style: solid;
    border-color: black;
    border-radius: 0.5em;
    border-width: 0.15em;
    padding: 0.5em;
    width: 2.2em;
    height: 1em;
}

input[type="number"]::-webkit-inner-spin-button,
input[type="number"]::-webkit-outer-spin-button {
  -webkit-appearance: none;
  margin: 0;
}

input[type="number"] {
  -moz-appearance: textfield;
}

</style>