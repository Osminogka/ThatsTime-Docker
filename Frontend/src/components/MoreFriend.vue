<script setup>

import { ref, reactive, watch, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';

import { getRecordsWithFriend } from '../core/userRecords';
import { deleteFriend } from '@/core/userInfo';
import { todayDate } from '@/core/month';

import RecordList from '@/view/RecordsList.vue';
import LoadingAnimation from '@/view/LoadingAnimation.vue';

const route = useRoute();
const router = useRouter();

const infoAboutFriend = reactive({
    isFriend: true,
    records: []
});
const error = ref(null);
const loading = ref(false);

const friendNickname = ref(route.params.nickname);

onMounted(async () => {
    await fetchData();
});

watch(() => route.params.nickname, async (nickname) =>{
    friendNickname.value = nickname;
    await fetchData();
    }
);

async function fetchData() {
    error.value = null;
    loading.value = true;
    try{
        let result = await getRecordsWithFriend(route.params.nickname, {
            year: todayDate.getFullYear(),
            month: todayDate.getMonth() + 1,
            day: todayDate.getDate()
        });
        if(result.success){
            infoAboutFriend.records = result.records;
            infoAboutFriend.isFriend = true;
        }
        else{
            infoAboutFriend.records = [];
            infoAboutFriend.isFriend = false;
        }
    } catch (e) {
        error.value = e;
    }finally{
        loading.value = false;
    }
}

async function deleteFriendLocal(){
  let response = await deleteFriend(route.params.nickname);
  if(response.success){
    friendList.value = friendList.value.filter((friend) => friend != friendname);
    router.push('/');
  }
  else{
    error.value = "Error deleting friend";
  }
  loading.value = false;
}
</script>

<template>
    <div v-if="error">
        <p>{{ error }}</p>
    </div>
    <loading-animation v-else-if="loading" />
    <div v-else-if="infoAboutFriend.isFriend">
        <div class="friend-manage-container">
            <div class="friend-info-header">{{ route.params.nickname }}</div>
            <button class="delete-friend-button custom-button" @click="deleteFriendLocal"/>
        </div>
        <h2 style="text-align: center;">Friend Records</h2>
        <record-list :records="infoAboutFriend.records" :friend="route.params.nickname" />
    </div>
    <div v-if="!infoAboutFriend.isFriend">
        <h1 class="not-your-friend">You don't have such friend</h1>
    </div>
</template>

<style scoped>

.friend-manage-container{
    display: flex;
    flex-direction: row;
    justify-content: space-between;
}

.friend-info-header{
    font-size: 24px;
    font-weight: bold;
    margin-bottom: 20px;
    margin-left: 10px;
}

.not-your-friend{
    font-size: 24px;
    font-weight: bold;
    margin-bottom: 20px;
    text-align: center;
}

.delete-friend-button{
    background-image: url('../assets/svg/Profile/bin.svg');
    background-color: white;
    margin-right: 10px;
}

.custom-button{
    background-position: center;
    background-size:contain;
    background-repeat: no-repeat;
    border: none;
    cursor: pointer;
    height: 2em;
    width: 2em;
}

</style>