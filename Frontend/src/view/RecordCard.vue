<script setup>
import { defineProps } from 'vue';
import { monthNames } from '@/core/month';

const props = defineProps({
    record: Object
});

const isTimeCorrent = () =>{
    return !(props.record.hour == 0 && props.record.minute == 0);
};

const importanceList = ['Low', 'Medium', 'High'];
</script>

<template>
<div class="card">
    <div class="card-title">{{ record.recordName }}</div>
    <div class="info-item">
        <span>Date: </span> {{ record.selectedDay }} {{ monthNames[record.selectedMonth - 1] }} {{ record.selectedYear }}
    </div>
    <div class="info-item">
        <span>Creator:</span> {{ record.creator }}
    </div>
    <div class="info-item">
        <span>For {{ record.showGroupList ? "group" : "user" }}:</span> {{ record.selectedObject }}
    </div>
    <div class="info-item">
        <span>importance:</span> {{ importanceList[record.importance] }}
    </div>
    <div v-if="isTimeCorrent()" class="info-item">
        <span>Time:</span> {{ record.hour }}:{{ record.minute == 0 ? '00' : record.minute }}
    </div>
    <div class="record-content">
        <p>{{ record.recordContent }}</p>
    </div>
</div>
</template>

<style scoped>
.card {
    margin: 10px;
    font-family: 'Roboto', sans-serif;
    width: 300px;
    background-color: #ffffff;
    border: 2px solid #00a8ff;
    border-radius: 12px;
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
    padding: 20px;
    box-sizing: border-box;
    color: #333333;
}

.card-title {
    font-size: 24px;
    font-weight: bold;
    margin-bottom: 10px;
    color: #00a8ff;
}

.info-item {
    margin-bottom: 10px;
}

.info-item span {
    font-weight: bold;
}

.record-content {
    border-top: 2px solid #00a8ff;
    padding-top: 10px;
    margin-top: 10px;
}

@media (max-width: 768px) {
    .card {
        width: 90%;
    }
}
</style>