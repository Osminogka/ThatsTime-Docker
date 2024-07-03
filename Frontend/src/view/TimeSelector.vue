<script setup>
import { defineProps, defineModel} from 'vue';

defineProps({
    type: String,
    error: Boolean,
});

const model = defineModel();
</script>

<template>
    <div v-if="type === 'hour'">
        <label class="custom-label">Hour</label>
        <p class="error-message" v-if="error">Invalid hour</p>
        <input class="input-time" :class="{'error-input':error}" type="number" v-model="model" min="0" max="23" step="1" placeholder="Hour"/>
    </div>
    <div v-else-if="type === 'minute'">
        <label class="custom-label">Minute</label>
        <p class="error-message" v-if="error">Invalid minute</p>
        <input class="input-time" :class="{'error-input':error}" type="number" v-model="model" min="0" max="59" step="1" placeholder="Minute"/>
    </div>
</template>

<style scoped>

.input-time{
    display: block;
    background-color: #55FFEF;
    border-style: solid;
    border-color: black;
    border-radius: 0.5em;
    border-width: 0.15em;
    padding: 0.5em;
}

.input-time::-webkit-inner-spin-button,
.input-time::-webkit-outer-spin-button {
  -webkit-appearance: none;
  margin: 0;
}

.input-time {
  -moz-appearance: textfield;
}

</style>