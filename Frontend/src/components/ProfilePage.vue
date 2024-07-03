<script setup>
import('../assets/css/profile.css');
import CustomHideShow from '@/view/CustomHideShow.vue';
import LoadingAnimation from '@/view/LoadingAnimation.vue';

import { ref, reactive} from 'vue';
import { friendList, friendRequests, groupInvites, groupList, user} from '../core/userInfo';
import { acceptFriendRequest, declineFriendRequest, deleteFriend} from '../core/userInfo';
import { acceptGroupRequest, declineGroupRequest } from '../core/groupInfo';
import { leaveTheGroup } from '../core/addGroup';

const error = ref("");
let loading = ref(false);

function showList(list){
  showInterface[list] = !showInterface[list];
}

const showInterface = reactive({
    showFriendList: false,
    showGroupList: false,
    showFriendInvites: false,
    showGroupInvites: false,
});

async function acceptFriendInvite(friendname){
  loading.value = true;
  error.value = null;
  let response = await acceptFriendRequest(friendname);
  if(response.success){
    friendRequests.value = friendRequests.value.filter((request) => request != friendname);
    friendList.value.push(friendname);
  }
  else{
    error.value = "Error accepting friend invite";
  }
  loading.value = false;
}

async function declineFriendInvite(friendname){
  loading.value = true;
  error.value = null;
  let response = await declineFriendRequest(friendname);
  if(response.success){
    friendRequests.value = friendRequests.value.filter((request) => request != friendname);
  }
  else{
    error.value = "Error declining friend invite";
  }
  loading.value = false;
}

async function acceptGroupInvite(groupname){
  loading.value = true;
  error.value = null;
  let response = await acceptGroupRequest(groupname);
  if(response.success){
    groupInvites.value = groupInvites.value.filter((invite) => invite != groupname);
    groupList.value.push(groupname);
  }
  else{
    error.value = "Error accepting group invite";
  }
  loading.value = false;
}

async function declineGroupInvite(groupname){
  let response = await declineGroupRequest(groupname);
  if(response.success){
    groupInvites.value = groupInvites.value.filter((invite) => invite != groupname);
  }
  else{
    error.value = "Error declining group invite";
  }
  loading.value = false;
}

async function deleteFriendLocal(friendname){
  let response = await deleteFriend(friendname);
  if(response){
    friendList.value = friendList.value.filter((friend) => friend != friendname);
  }
  else{
    error.value = "Error deleting friend";
  }
  loading.value = false;
}

async function leaveGroupLocal(groupname){
  let response = await leaveTheGroup(groupname);
  if(response){
    groupList.value = groupList.value.filter((group) => group != groupname);
  }
  else{
    error.value = "Error leaving group";
  }
  loading.value = false;
}

</script>

<template>
  <div class="container-user">
    <div class="user-basic-info">
        <p>{{ user.name }}</p>
        <p>{{ user.email }}</p>
    </div>
    <div v-if="error" class="error">
      <p>{{ error }}</p>
    </div>
    <div v-if="loading">
      <loading-animation/>
    </div>
    <div v-else class="user-info-list">
        <custom-hide-show :list="friendList" :showInterface="showInterface.showFriendList" @showList="showList" :showType="'showFriendList'">
          Friends
        </custom-hide-show>
        <Transition name="navlists">
          <div v-if="showInterface.showFriendList">
            <div v-if="friendList.length == 0" class="no-user-info">
              <p>No friends</p>
            </div>
            <div v-else class="main-profile-div">
              <div v-for="(friend ,index) in friendList" :key="index" class="entity">
                <p style="display: inline;">{{ friend }}</p>
                <router-link class="button-nav custom-button" :to="{ name: 'Friend', params: { nickname: friend } }" />
                <button class="delete-button custom-button" @click="deleteFriendLocal(friend)"></button>
              </div>
            </div>
          </div>
        </Transition>


        <custom-hide-show :list="groupList" :showInterface="showInterface.showGroupList" @showList="showList" :showType="'showGroupList'">
          Groups
        </custom-hide-show>
        <Transition name="navlists">
          <div v-if="showInterface.showGroupList">
            <div v-if="groupList.length == 0" class="no-user-info">
              <p>No groups</p>
            </div>
            <div v-else class="main-profile-div">
              <div v-for="(group ,index) in groupList" :key="index" class="entity">
                <p style="display: inline;">{{ group }}</p>
                <router-link class="button-nav custom-button" :to="{ name: 'Group', params: { groupname: group } }" />
                <button class="custom-button delete-button" @click="leaveGroupLocal(group)"></button>
              </div>
            </div>
          </div>
        </Transition>

        <custom-hide-show :list="friendRequests" :showInterface="showInterface.showFriendInvites" @showList="showList" :showType="'showFriendInvites'">
          Invites
        </custom-hide-show>
        <Transition name="navlists">
          <div v-if="showInterface.showFriendInvites">
            <div v-if="friendRequests.length == 0" class="no-user-info">
              <p>No friend requests</p>
            </div>
            <div v-else class="main-profile-div">
              <div v-for="(request ,index) in friendRequests" :key="index" class="entity">
                <p style="display: inline;">{{ request }}</p>
                <div style="display: block;">
                  <button class="button-accept custom-button" @click="acceptFriendInvite(request)"></button>
                  <button class="button-decline custom-button" @click="declineFriendInvite(request)"></button>
                </div>
              </div>
            </div>
          </div>
        </Transition>

        
        <custom-hide-show :list="groupInvites" :showInterface="showInterface.showGroupInvites" @showList="showList" :showType="'showGroupInvites'">
          Group Invites
        </custom-hide-show>
        <Transition name="navlists">
          <div v-if="showInterface.showGroupInvites">
            <div v-if="groupInvites.length == 0" class="no-user-info">
              <p>No group invites</p>
            </div>
            <div v-else class="main-profile-div">
              <div v-for="(invite ,index) in groupInvites" :key="index" class="entity">
                <p style="display: inline;">{{ invite }}</p>
                <div style="display: block;">
                  <button class="button-accept custom-button" @click="acceptGroupInvite(invite)"></button>
                  <button class="button-decline custom-button" @click="declineGroupInvite(invite)"></button>
                </div>
              </div>
            </div>
          </div>
        </Transition>
    </div>
  </div>
</template>

<style scoped>

.no-user-info{
  text-align: center;
}

</style>