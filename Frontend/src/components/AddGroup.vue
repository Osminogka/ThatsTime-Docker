<script setup>
import { watch, ref, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { getGroupByName, getGroupList } from '../core/addGroup';

import SocialEntity from '@/view/SocialEntity.vue';
import LoadingAnimation from '@/view/LoadingAnimation.vue';
import { groupList } from '@/core/userInfo';

const route = useRoute();
const router = useRouter();

const currentPage = ref(0);

const loading = ref(false);
const groups = ref([]);
const error = ref(null);

const groupname = ref('');

async function loadGroups(){
    loading.value = true;
    try{
        let response = await getGroupList(parseInt(currentPage.value));
        if(response.success)
        {
            groups.value = response.groups;
            error.value = "";
        }
        else
        {
            groups.value = [];
            error.value = "Server error!";
        }
        loading.value = false;
    }
    catch(err) {
        error.value = err;
        loading.value = false;
    }
}

async function getCertainGroup(){
    groups.value = [];
    loading.value = true;
    try{
        let tempGroup = await getGroupByName(groupname.value);
        if(tempGroup.success && tempGroup.groups.length > 0)
            groups.value.push(tempGroup.groups[0]);
        loading.value = false;
    }
    catch(err){
        error.value = err;
        loading.value = false;
    }
}

onMounted(() =>{
    currentPage.value = parseInt(route.query.page) || 0;
    loadGroups();
});

watch(() => route.query.page, (page) => {
    currentPage.value = page;
    loadGroups();
});

function nextPage(){
    currentPage.value++;
    router.push({ query: { page: currentPage.value } });
}

function prevPage(){
    if(currentPage.value < 1) return;
    currentPage.value--;
    router.push({ query: { page: currentPage.value } });
}

function searchGroup(){
    getCertainGroup();
}

function enterTheGroup(groupname){
    groupList.value.push(groupname);
    groups.value = groups.value.filter(group => group !== groupname);
}

</script>

<template>
    <div class="addfriend-container">
        <div class="search-container">
            <form>
                <input v-model="groupname" type="text" placeholder="Search.." name="search">
                <button class="search-friend-button custom-button" type="submit" @click.prevent="searchGroup"><i class="fa fa-search"></i></button>
            </form>
        </div>
        <div class="friend-display-container">
            <loading-animation v-if="loading" />
            <div v-else-if="error" class="error">{{ error.message }}</div>
            <div v-if="groups.length > 0">
                <social-entity v-for="(group, index) in groups" :name="group" type='group' :key="index" @group-enter="enterTheGroup(group)"/>
            </div>
            <div v-else>
                <p>Such group don't exist</p>
            </div>
        </div>
        <div class="page-nav-button-container">
            <button v-if="currentPage > 0" class="page-nav-button page-nav-button-prev" @click="prevPage" />
            <button v-if="groups.length > 4" class="page-nav-button page-nav-button-next" @click="nextPage" />
        </div>
    </div>
</template>

<style scoped>

.addfriend-container{
    display: flex;
    flex-direction: column;
    align-items: center;
    margin-left: 0.2em;
    width: 100%;
}

.search-container {
    display: flex;
    flex-direction: row;
    border-style: solid;
    border-color: black;
    border-width: 3px;
    border-radius: 0.2em;
}

.page-nav-button-container{
    display: flex;
    flex-direction: row;
    margin-top: auto;
    justify-content: flex-end;
    align-items: center;
    width: 100%;
}

.friend-display-container{
    display: flex;
    flex-direction: column;
    align-items: center;
    width: 100%;
    height: 100%;
}

.custom-button{
    background-position: center;
    background-size:contain;
    background-repeat: no-repeat;
    border: none;
}

.search-friend-button{
    background-image: url('../assets/svg/Records/search.svg');
    border-left: 3px solid black;
    float: right;
    height: 100%;
    padding: 1em;
    background-color: #ddd;
    font-size: 17px;
    border: none;
    cursor: pointer;
}

input[type=text] {
    padding: 6px;
    margin-top: 8px;
    font-size: 17px;
    border: none;
    outline: none;
}

.search-friend-button:hover {
    background-color: #ccc;
}

.page-nav-button{
    margin: auto;
    width: 2.5em;
    height: 1.5em;
    border: none;
    background-color: white;
    background-repeat: no-repeat;
    background-position: center;  
    cursor: pointer;
}

.page-nav-button-next{
    background-image: url('../assets/svg/MainTable/rightArrow.svg');
    align-self: flex-start;
}

.page-nav-button-prev{
    background-image: url('../assets/svg/MainTable/leftArrow.svg');
    align-self: flex-end;
}

</style>