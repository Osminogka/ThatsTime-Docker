<script setup>
import('./assets/css/router.css');
import MainHeader from './components/MainHeader.vue';
import { todayDate, monthNames } from './core/month';
import { friendRequests,groupInvites, getMyFriendList, getMyGroupList, getMyFriendRequests, getMyGroupInvites} from './core/userInfo';
import { isAuthenticated, getCurrentUser } from '@/core/authentication'; 

import { ref, onBeforeMount } from 'vue';

const error = ref(null);

onBeforeMount(async () => {
  getCurrentUser();
  if(isAuthenticated){
    await getMyFriendList();
    await getMyGroupList();
    error.value = null;
    let friendInviteResponse = await getMyFriendRequests();
    let groupInviteResponse = await getMyGroupInvites();
    if(friendInviteResponse.success)
      friendRequests.value = friendInviteResponse.friendList;
    else{
      friendRequests.value = [];
      error.value = "Error getting friend invites";
    }
    if(groupInviteResponse.success)
      groupInvites.value = groupInviteResponse.groups;
    else{
      friendRequests.value = [];
      error.value = "Error getting group invites";
    }
  }
});

</script>

<template>
  <header v-if="isAuthenticated">
    <main-header />
  </header>
  <main>
    <div v-if="isAuthenticated">
      <div class="link-container">
        <router-link to="/" class="custom-link" >Main Page</router-link>
        <router-link to="/records" class="custom-link">Records</router-link>
      </div>
      <h1 class="h1-date">{{ todayDate.getDate() }} {{  monthNames[todayDate.getMonth()] }} {{ todayDate.getFullYear() }}</h1>
    </div>
    <div v-if="error">{{ error }}</div>
    <router-view>
    </router-view>
  </main>
</template>

<style scoped>

.h1-date {
  font-size: 2em;
  color: #333;
  text-align: right;
  border-bottom: 2px solid #333;
  font-family: 'Tahoma';
  font-size: 1.3em;
  margin-right: 0.2em;
}

</style>