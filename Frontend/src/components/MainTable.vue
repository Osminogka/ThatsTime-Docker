<script setup>
import('../assets/css/table.css')
import('../assets/css/mainForm.css')
import('../assets/css/user.css')
import('../assets/css/customSelect.css')

import TimeSelector from '@/view/TimeSelector.vue'

import { friendList, groupList, user} from '../core/userInfo'
import {todayDate, monthNames} from '../core/month'
import { postRecord } from '@/core/userRecords';

import {ref, computed, reactive, toRaw } from 'vue';

const recordCreationStatus = reactive({
    status: true,
    message: '',
    showMessage: false
})

async function submitForm() {
    let result = await postRecord({...toRaw(recordCreationForm)});
    for(let key in errorList) 
        errorList[key].error = false;
    if(!result.success && result.message == 'Invalid record'){
        for(let key of result.records) 
            errorList[key].error = true;
        recordCreationStatus.showMessage = true;
        recordCreationStatus.message = result.message;
        recordCreationStatus.status = false;
        clearInputs(false);
    }
    else if(!result.success && result.message == 'Server error'){
        recordCreationStatus.showMessage = true;
        recordCreationStatus.message = 'Server error!';
        recordCreationStatus.status = false;
        clearInputs(true);
    }
    else{
        recordCreationStatus.showMessage = true;
        recordCreationStatus.message = 'Record created successfully!';
        recordCreationStatus.status = true;
        clearInputs(true);
    }
    setTimeout(() => {
        recordCreationStatus.showMessage = false;
    }, 3000);
}

function clearInputs(wasSuccessful) {
    for (let key in errorList) {
        let errorProp = errorList[key];
        if(wasSuccessful){
            errorProp.error = false;
            if(key !== 'selectedObject')
                recordCreationForm[key] = errorProp.default;
        }
        if(errorProp.error && !wasSuccessful)
            recordCreationForm[key] = errorProp.default;
    }
}


const importanceList = [
    { name: 'Low', value: 0 },
    { name: 'Medium', value: 1 },
    { name: 'High', value: 2 }
];

const recordCreationForm = reactive({
    selectedYear: todayDate.getFullYear(),
    selectedMonth: todayDate.getMonth(),
    selectedDay: 1,
    showGroupList: false,
    yourSelf: false,
    selectedObject: "",
    importance: 0,
    enterTime: false,
    hour: 0,
    minute: 0,
    recordName: '',
    recordContent: '',
    showObjectList: false,
    showImportanceList: false
});

const errorList = reactive({
    selectedYear: {
        error: false,
        message: 'Invalid year',
        default: todayDate.getFullYear()
    },
    selectedDay: {
        error: false,
        message: 'Invalid day',
        default: 1
    },
    selectedMonth: {
        error: false,
        message: 'Invalid month',
        default: todayDate.getMonth()
    },
    selectedObject: {
        error: false,
        message: 'Invalid person selected',
        default: ''
    },
    hour: {
        error: false,
        message: 'Invalid hour',
        default: 0
    },
    minute: {
        error: false,
        message: 'Invalid minute',
        default: 0
    },
    recordName: {
        error: false,
        message: recordCreationForm.recordName.length <= 0 ? 'Record name is too short' : 'Record name is too long',
        default: ''
    },
    recordContent: {
        error: false,
        message: recordCreationForm.recordContent <= 0 ? 'Record content is too long' : 'Record content is too short',
        default: ''
    }
});

let currentYear = todayDate.getFullYear();
let daysInMonth = ref(new Date(currentYear, todayDate.getMonth() + 1, 0).getDate());
let firstDayOfMonth = ref(new Date(currentYear, todayDate.getMonth(), 1).getDay() - 1);

let selectedMonthName = computed(() => monthNames[recordCreationForm.selectedMonth]);
let selectedYear = computed(() => recordCreationForm.selectedYear !== todayDate.getFullYear() ? recordCreationForm.selectedYear : '');

let isTodayDate = (day) => {
    return day === todayDate.getDate() && todayDate.getMonth() === recordCreationForm.selectedMonth;
}

function toggleObjectList() {
    if(recordCreationForm.showImportanceList)
        recordCreationForm.showImportanceList = false;
    recordCreationForm.showObjectList = !recordCreationForm.showObjectList;
}
function selectObjectList(option) {
    recordCreationForm.selectedObject = option;
}

function selectObjectType(option) {
    if(option === 'Group'){
        recordCreationForm.showGroupList = !recordCreationForm.showGroupList;
        recordCreationForm.yourSelf = false;
        recordCreationForm.selectedObject = recordCreationForm.showGroupList? 
            groupList.value.length > 0 ? groupList.value[0] : "" :  
            friendList.value.length > 0 ? friendList.value[0]: "";
    }
    else{
        recordCreationForm.yourSelf = !recordCreationForm.yourSelf;
        recordCreationForm.showGroupList = false;
        recordCreationForm.selectedObject = recordCreationForm.yourSelf? user.value.name : 
            friendList.value.length == 0 ? "" : friendList.value[0];
    }
}

function toggleImportance() {
    recordCreationForm.showImportanceList = !recordCreationForm.showImportanceList;
}

function selectImportance(option) {
    recordCreationForm.importance = importanceList.find(item => item.name === option).value;
}

let animateCell = (event) => {
    let button = event.target;
    button.classList.add('animate');
    setTimeout(() => {
        button.classList.remove('animate');
        recordCreationForm.selectedDay = parseInt(event.target.textContent);
    }, 300);
}

let animateArrow = (monthChange) => {
    recordCreationForm.selectedDay = 1;
    if(recordCreationForm.selectedMonth + monthChange < 0) {
        recordCreationForm.selectedMonth = 11;
        recordCreationForm.selectedYear--;
    } else if(recordCreationForm.selectedMonth + monthChange > 11) {
        recordCreationForm.selectedMonth = 0;
        recordCreationForm.selectedYear++;
    } else
        recordCreationForm.selectedMonth += monthChange;
    daysInMonth.value = new Date(recordCreationForm.selectedYear, recordCreationForm.selectedMonth + 1, 0).getDate();
    firstDayOfMonth.value = new Date(recordCreationForm.selectedYear, recordCreationForm.selectedMonth , 1).getDay() - 1;
    let button = event.target;
    button.classList.add('animate');
    setTimeout(() => {
        button.classList.remove('animate');
    }, 300);
}

const weeks = computed(() => {
    let weeks = [];
    let week = [];
    let day = 1;

  // Fill the first week of the month with empty days until the first day of the month
    for (let i = 0; i < 7; i++) {
        if (i < firstDayOfMonth.value) {
            week.push(null);
        } else {
            week.push(day++);
        }
    }
    weeks.push(week);

// Fill the rest of the weeks
    while (day <= daysInMonth.value) {
        week = [];
        for (let i = 0; i < 7; i++) {
            if(day <= daysInMonth.value)
                week.push(day++);
            else
                week.push(null);
        }
        weeks.push(week);
    }
    return weeks;
});
</script>

<template>
    <div class="container">
        <div class="table-container">
            <p class="month-header">{{ selectedMonthName }} <span v-if="selectedYear">{{ selectedYear }}</span></p>
            <table>
                <thead>
                    <tr>
                        <th>Mon</th>
                        <th>Tue</th>
                        <th>Wed</th>
                        <th>Thu</th>
                        <th>Fri</th>
                        <th>Sat</th>
                        <th>Sun</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(week, index) in weeks" :key="index">
                        <td v-for="(day, index) in week" :key="index" 
                            :class="{ 'td-today': isTodayDate(day), 'td-common-day':!isTodayDate(day), 'td-null-day': day === null }">
                                <button v-if="day !== null" @click="animateCell" class="button-cell" 
                                    :class="{'button-cell-current-day':isTodayDate(day), 'button-cell-selected': day == recordCreationForm.selectedDay}">
                                    {{ day }}
                                </button>
                        </td>
                    </tr>
                </tbody>
            </table>
            <div class="arrow-container">
                <button @click="animateArrow(-1)" class="left-arrow custom-button arrow" />
                <button @click="animateArrow(1)" class="right-arrow custom-button arrow" />
            </div>
        </div>
        <form class="form-for-record">
            <p class="p-selected-day">Selected day: {{ recordCreationForm.selectedDay }}</p>
            <div class="checkbox-container">
                <div id="input-checkbox" :class="{'input-checkbox-checked': recordCreationForm.showGroupList}" class="input-checkbox" 
                    @click="selectObjectType('Group')"></div>
                <label class="label-checkbox">For Group</label>
                <div id="input-checkbox-yourself" :class="{ 'input-checkbox-checked': recordCreationForm.yourSelf }" class="input-checkbox" 
                    @click="selectObjectType('Yourself')" style="margin-left: 1em;"></div>
                <label class="label-checkbox">Yourself</label>
            </div>
            <p class="error-message" v-if="errorList.selectedObject.error">{{ errorList.selectedObject.message }}</p>
            <Transition name="showTimeEnter">
                <div v-if="!recordCreationForm.yourSelf" class="aselect" :data-value="recordCreationForm.selectedObject" :data-list="recordCreationForm.showGroupList? groupList : friendList">
                    <div :class="{'error-input': errorList.selectedObject.error }" class="selector" @click="toggleObjectList()">
                        <div class="label">
                            <span>{{ recordCreationForm.selectedObject == "" ?  
                                recordCreationForm.showGroupList? "Choose group" : "Choose friend" 
                                : recordCreationForm.selectedObject }}</span>
                        </div>
                        <div class="arrow-select" :class="{ expanded : recordCreationForm.showObjectList }"></div>
                        <Transition name="fadey">
                            <div v-if="recordCreationForm.showObjectList">
                                <ul>
                                    <div v-if="recordCreationForm.showGroupList? groupList.length == 0 : friendList.length == 0">
                                        <p class="no-info-p">No {{ recordCreationForm.showGroupList? 'groups' : 'friends' }} found</p>
                                    </div>
                                    <div v-else>
                                        <li :class="{ current : item === value }" v-for="(item, index) in recordCreationForm.showGroupList? groupList : friendList" 
                                        @click="selectObjectList(item)" :key="index">
                                            {{ item }}
                                        </li>
                                    </div>
                                </ul>
                            </div>
                        </Transition>
                    </div>
                </div>
            </Transition>
            <label class="custom-label">Select importance</label>
            <div class="aselect" :data-value="recordCreationForm.importance" :data-list="importanceList.name">
                <div class="selector" @click="toggleImportance()">
                    <div class="label">
                        <span>{{ importanceList.find(item => item.value === recordCreationForm.importance).name }}</span>
                    </div>
                    <div class="arrow-select" :class="{ expanded : recordCreationForm.showImportanceList }"></div>
                    <Transition name="fadey">
                        <div v-if="recordCreationForm.showImportanceList">
                            <ul>
                                <li :class="{ current : item === value }" v-for="(item, index) in importanceList" 
                                @click="selectImportance(item.name)" :key="index">
                                    {{ item.name }}
                                </li>
                            </ul>
                        </div>
                    </Transition>
                </div>
            </div>
            <div class="checkbox-container checkbox-container-time">
                <div id="input-checkbox" :class="{'input-checkbox-checked': recordCreationForm.enterTime}" class="input-checkbox" @click="recordCreationForm.enterTime = !recordCreationForm.enterTime"></div>
                <label class="label-checkbox">Enter Time</label>
            </div>
            <Transition name="showTimeEnter">
                <div v-if="recordCreationForm.enterTime">
                    <time-selector v-model="recordCreationForm.hour" type="hour" :error="errorList.hour.error"/>
                    <time-selector v-model="recordCreationForm.minute" type="minute" :error="errorList.minute.error"/>
                </div>
            </Transition>
            <div>
                <label class="custom-label">Record Name</label>
                <p class="error-message" v-if="errorList.recordName.error">{{ errorList.recordName.message }}</p>
                <input class="input-form" :class="{'error-input':errorList.recordName.error}" type="text" v-model="recordCreationForm.recordName" />
                <label class="custom-label">Record Content</label>
                <p class="error-message" v-if="errorList.recordContent.error">{{ errorList.recordContent.message }}</p>
                <textarea class="textarea-form input-record-content" :class="{'error-input': errorList.recordContent.error}"  v-model="recordCreationForm.recordContent"></textarea>
            </div>
            <button @click.prevent="submitForm" class="submit-button">Create Record</button>
            <Transition name="showTimeEnter">
                <p class="creation-status" :class="{'creation-successful': recordCreationStatus.status, 'creation-failed': !recordCreationStatus.status}" v-if="recordCreationStatus.showMessage">{{ recordCreationStatus.message }}</p>
            </Transition>
        </form>
    </div>
</template>