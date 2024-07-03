<script setup>
import { defineProps, defineEmits } from 'vue'

defineProps({
    showInterface: Object,
    showType: String,
    list: {
        type: Array,
        default: () => []
    },
});

const emits = defineEmits(['showList']);

function showHidden(propName){
  emits('showList', propName);
}
</script>

<template>
    <div @click="showHidden(showType)" class="nav-header">
          <p class="single-line"><slot/><span v-if="list.length > 0">(<span class="amount-number">{{ list.length }}</span>)</span></p>
          <hr class="nav-hr"/>
          <div class="arrow-select-user" 
            :class="{ 'expanded-user' : showInterface }"></div>
    </div>
</template>

<style scoped>

.nav-header{
    margin-left: 10px;
    display: flex;
    flex-direction: row;
    align-items: center;
    cursor: pointer;
}

.nav-hr{
    width: 100%;
    background-color: black;
    margin-left: 1em;
    margin-right: 1em;
}

.arrow-select-user {
    width: 0;
    height: 0;
    border-left: 7px solid transparent;
    border-right: 7px solid transparent;
    border-top: 10px solid #888;
    transform: rotateZ(0deg) translateY(0px);
    transition-duration: 0.3s;
    transition-timing-function: cubic-bezier(.59,1.39,.37,1.01);
    background-color: var(--background-color);
}

.expanded-user {
    transform: rotateZ(180deg) translateY(2px);
}

.single-line{
    font-size: 1.2em;
    white-space: nowrap;
}

.amount-number{
  color: rgb(1, 184, 7)
}

</style>