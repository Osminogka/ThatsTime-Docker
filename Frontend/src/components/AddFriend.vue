<script setup>
import { watch, ref, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { getFriendList, getFriendByNickname } from '../core/addFriend';

import SocialEntity from '@/view/SocialEntity.vue';
import LoadingAnimation from '@/view/LoadingAnimation.vue';

const route = useRoute();
const router = useRouter();

const currentPage = ref(0);

const loading = ref(false);
const users = ref([]);
const error = ref(null);

const username = ref('');

async function loadUsers(){
    loading.value = true;
    try{
        let response = await getFriendList(parseInt(currentPage.value));
        if(response.success)
        {
            users.value = response.friendList;
            error.value = "";
        }
        else
        {
            users.value = [];
            error.value = "Server error!";
        }
        loading.value = false;
    }
    catch(err) {
        error.value = err;
        loading.value = false;
    }
}

async function getCertainUser(){
    users.value = [];
    loading.value = true;
    try{
        let tempUser = await getFriendByNickname(username.value)
        if(tempUser.success && tempUser.friendList.length > 0)
            users.value.push(tempUser.friendList[0]);
        loading.value = false;
    }
    catch(err){
        error.value = err;
        loading.value = false;
    }
}

onMounted(() =>{
    currentPage.value = parseInt(route.query.page) || 0;
    loadUsers();
});

watch(() => route.query.page, (page) => {
    currentPage.value = page;
    loadUsers();
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

function searchFriend(){
    if(username.value == '') 
        loadUsers();
    else
        getCertainUser();
}

function inviteIsSent(name){
    users.value = users.value.filter(user => user !== name);
}

</script>

<template>
    <div class="addfriend-container">
        <div class="search-container">
            <form>
                <input class="search-input-box" v-model="username" type="text" placeholder="Search.." name="search">
                <button class="search-friend-button custom-button" type="submit" @click.prevent="searchFriend"><i class="fa fa-search"></i></button>
            </form>
        </div>
        <div class="friend-display-container">
            <loading-animation v-if="loading" />
            <div v-else-if="error" class="error">{{ error.message }}</div>
            <div v-if="users.length > 0">
                <social-entity v-for="(user, index) in users" :name="user" type='friend' @friend-request="inviteIsSent" :key="index"/>
            </div>
            <div v-else-if="!loading">
                <p>Such user don't exist</p>
            </div>
        </div>
        <div class="page-nav-button-container">
            <button v-if="currentPage > 0" class="page-nav-button page-nav-button-prev" @click="prevPage" />
            <button v-if="users.length > 4" class="page-nav-button page-nav-button-next" @click="nextPage" />
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
    justify-content: center;
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
    margin-left: auto;
    float: right;
    height: 100%;
    padding-right: 20px;
    padding-left: 20px;
    background-color: #ddd;
    cursor: pointer;
}

.search-input-box {
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