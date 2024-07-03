<script setup>
import { createGroup } from '@/core/addGroup';
import { ref, reactive } from 'vue';

import { friendList, groupList } from '@/core/userInfo';
import { inviteFriendToGroup } from '@/core/groupInfo';

const groupname = ref('');
const success_groupname = ref('');
const creationResult = reactive({
    error: false,
    success: false,
    message: ''
});

async function createGroupChecks(){
    success_groupname.value = '';
    const result = await createGroup(groupname.value);
    if(result.success) {
        groupList.value.push(groupname.value);
        success_groupname.value = groupname.value;
        friendsToInvite.value = [...friendList.value];
        creationResult.error = false;
        creationResult.success = true;
    }
    else {
        creationResult.error = true;
        creationResult.success = false;
        creationResult.message = result.message;
    }
    groupToInvite = groupname.value;
    groupname.value = '';
}

const friendsToInvite = ref([]);
const error = ref('');
var groupToInvite = '';

async function inviteFriend(friendName){
    error.value = '';
    let response = await inviteFriendToGroup(friendName, groupToInvite);
    if(response.success)
        friendsToInvite.value = friendsToInvite.value.filter(friend => friend !== friendName);
    else
        error.value = response.message;
}
</script>

<template>
    <div class="creation-group-container">
        <form class="create-group-container">
            <input class="create-group-input-box" v-model="groupname" type="text" placeholder="Group Name" name="groupname">
            <button class="create-group-button" @click.prevent="createGroupChecks()" type="submit">Create Group</button>
        </form>
        <div class="creation-result-container">
            <Transition name="bounce">
                <div v-if="creationResult.error" class="group-creation-status group-creation-failed">{{ creationResult.message }}</div>
                <div v-else-if="creationResult.success">
                    <div class="group-creation-status group-creation-success">Group {{ success_groupname }} created successfully</div>
                    <div v-if="friendsToInvite.length === 0" style="text-align: center; margin-top: 10px">No friends to invite</div>
                    <div v-else-if="error" class="group-creation-status group-creation-failed">{{ error }}</div>
                    <div v-else class="friends-to-invite-container">
                        <div class="friend-invite-container" v-for="(friend, index) in friendsToInvite" :key="index">
                            <p>{{ friend }}</p>
                            <button @click="inviteFriend(friend)" class="send-invitation-button"></button>
                        </div>
                    </div>
                </div>
            </Transition>
        </div>
    </div>
</template>

<style scoped>

.creation-group-container{
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    width: 100%;
    height: 100%;
}

.create-group-container{
    display: flex;
    flex-direction: column;
    width: 100%;
    height: 100%;
    align-items: center;
    justify-content: center;
    margin-left: auto;
}

.creation-result-container{
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    margin-top: 1em;
}

.friend-invite-container{
    display: flex;
    flex-direction: row;
    justify-content: space-between;
    align-items: center;
    border: 1px solid black;
    border-radius: 5px;
    width: 20vw;
    padding: 10px;
    margin: 10px;
}

.friends-to-invite-container{
    display: flex;
    flex-direction: column;
    border-color: black;
    border-style: solid;
    border-radius: 5px;
    background-color: white;
    padding-left: 10px;
    font-size: 1em;
    padding-right: 10px;
    max-height: 60em;
    overflow-y: auto;
    margin: 10px;
}

.create-group-input-box{
    border-style: solid;
    border-color: black;
    border-width: 3px;
    border-radius: 0.2em;
    padding: 0.5em;
    font-size: 1.1em;
}

.create-group-button{
    margin-top: 1em;
    background-color: #55FFEF;
    border-style: solid;
    border-color: black;
    border-radius: 0.5em;
    border-width: 0.15em;
    padding: 0.5em;
    font-weight: bold;
    cursor: pointer;
}
.group-creation-status{
    border-style: solid;
    border-width: 0.3em;
    border-radius: 0.5em;
    padding: 5px;
    text-align: center;
    width: auto;
}

.group-creation-failed{
    border-color: rgb(255, 0, 0);
    background-color: rgb(252, 134, 134);
}

.group-creation-success{
    border-color: green;
    background-color: rgb(9, 197, 9);
}

.bounce-enter-active {
  animation: bounce-in 0.5s;
}
.bounce-leave-active {
  animation: bounce-in 0.5s reverse;
}
@keyframes bounce-in {
  0% {
    transform: scale(0);
  }
  100% {
    transform: scale(1);
  }
}

.send-invitation-button{
    background-image: url('../assets/svg/Social/addfriend.svg');
    background-position: center;
    background-size:contain;
    background-repeat: no-repeat;
    border: none;
    width: 2em;
    height: 2em;
    cursor: pointer;
    background-color: white;
    justify-content: flex-end;
    margin-left: auto;
}

@media (max-width: 600px) or (max-height: 1600px) {
    .friends-to-invite-container{
        height: 20em;
    }  
}
</style>