<script setup>
import('../assets/css/header.css');
import('../assets/css/navigation.css');
import('../assets/css/nav-buttons.css');
import('../assets/css/user.css');
import CustomHideShow from '@/view/CustomHideShow.vue';
import { reactive, ref, onMounted } from 'vue';
import { vOnClickOutside } from '@vueuse/components';
import { friendList, groupList, friendRequests, groupInvites} from '@/core/userInfo';

function hideSideBar() {
  if(showInterface.showSideBar && event.target.id != 'showSideBar')
  {
    showInterface.showSideBar = false;
    rotateTag(true);
  }
}

function hideUserNav() {
  if(showUserNav.value && event.target.id != 'user-icon-button')
    showUserNav.value = false;
}

onMounted(() => {
  window.addEventListener('scroll', hideSideBar);
  window.addEventListener('scroll', hideUserNav);

  let animateButton = document.getElementById('user-icon-button');

  animateButton.addEventListener('click', () => {
    animateButton.classList.add('animate');

    // Remove the 'animate' class after the animation is finished
    setTimeout(() => {
      animateButton.classList.remove('animate');
    }, 300);
  });

  const mainHeader = document.getElementById('main-header');
  const sidebar = document.getElementById('sidebar');
  let parentElement = mainHeader.parentElement;
  let heightPercentage = (mainHeader.offsetHeight / parentElement.offsetHeight) * 100 - 13;
  sidebar.style.height = 'calc(100% - ' + heightPercentage + 'px)';
});

function rotateTag(isExplicit = false) {
  let tempProp = showInterface.showSideBar;
  if(!isExplicit) tempProp = !tempProp;
  showInterface.showSideBar = tempProp;
  let new_value = tempProp ? 'rotate(90deg)' : "rotate(0deg)";
  document.getElementById('showSideBar').style.transform = new_value;
}

function showList(list){
  showInterface[list] = !showInterface[list];
}

const showInterface = reactive({
    showSideBar: false,
    showFriendList: false,
    showGroupList: false,
});

const showUserNav = ref(false);
</script>

<template>
    <div id="main-header" class="main-header">
      <div class="main-header">
        <button id="showSideBar" class="show-nav-button" @click="rotateTag()">
        </button>
        <h1 class="header-text">That's Time!</h1>
        <button id="user-icon-button" @click="showUserNav = !showUserNav" class="user-icon" v-on-click-outside="hideUserNav">
          <span v-if="groupInvites.length + friendRequests.length > 0" class="invite-count">{{ groupInvites.length + friendRequests.length }}</span>
        </button>
      </div>
    </div>
    <Transition name="usernav">
      <div id="usernavmenu" v-show="showUserNav" class="user-nav">
        <router-link to="/profile" class="button-nav-user button-nav-profile"></router-link>
        <router-link to="/logout" class="button-nav-user button-nav-logout"></router-link>
      </div>
    </Transition>

    <Transition name="sidebar">
      <div ref="scrollable" id="sidebar" v-show="showInterface.showSideBar" class="sidenav" v-on-click-outside="hideSideBar">
        <router-link :to="{ name: 'AddFriend', query: { page: 0 } }" class="add-button add-friend-button custom-button" @click="hideSideBar"></router-link>
        <custom-hide-show :list="friendList" :showInterface="showInterface.showFriendList" @showList="showList" :showType="'showFriendList'">
          Friends
        </custom-hide-show>
        <Transition name="navlists">
          <div v-if="showInterface.showFriendList">
            <div v-if="friendList.length == 0">
              <p class="no-info-p">No friends</p>
            </div>
            <div v-else class="main-nav-div">
              <div v-for="(friend ,index) in friendList" :key="index" class="sidebar-entity-box">
                <p style="display: inline;">{{ friend }}</p>
                <router-link :to="{ name: 'Friend', params: { nickname: friend } }" class="button-nav custom-button" @click="hideSideBar"/>
              </div>
            </div>
          </div>
        </Transition>

        <custom-hide-show :list="groupList" :showInterface="showInterface.showGroupList" @showList="showList" :showType="'showGroupList'">
          Groups
        </custom-hide-show>
        <Transition name="navlists">
          <div v-if="showInterface.showGroupList">
            <div v-if="groupList.length == 0">
              <p class="no-info-p">No groups</p>
            </div>
            <div v-else class="main-nav-div">
              <div v-for="(group ,index) in groupList" :key="index" class="sidebar-entity-box">
                <p style="display: inline;">{{ group }}</p>
                <router-link :to="{ name: 'Group', params: { groupname: group } }" class="button-nav custom-button" @click="hideSideBar"/>
              </div>
            </div>
          </div>
        </Transition>
      </div>
  </Transition>
</template>

<style scoped>

.no-info-p{
  margin: 0;
  padding: 0;
  font-size: 1.5em;
  color: black;
  text-align: center;
}

.invite-count {
  position: absolute;
    bottom: -5px;
    right: -5px; /* Adjust this value to center the circle horizontally */
    width: 20px; /* Diameter of the circle */
    height: 20px; /* Diameter of the circle */
    border-radius: 50%; /* Make it a circle */
    background-color: red; /* Red circle */
    color: white; /* Text color */
    font-size: 12px; /* Font size of the number */
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 1;
}

</style>