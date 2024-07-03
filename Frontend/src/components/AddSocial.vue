<script setup>

</script>

<template>
    <div class="social-container">
        <div class="social-navigation-container">
            <router-link :to="{ name: 'AddFriend', query: { page: 0 } }"  class="social-link">Add Friend</router-link>
            <router-link :to="{ name: 'AddGroup', query: { page: 0 } }" class="social-link">Add Group</router-link>
            <router-link :to="{ name: 'CreateGroup'}" class="social-link create-group-button"></router-link>
        </div>
        <router-view v-slot="{ Component }">
            <component :is="Component" />
        </router-view>
    </div>
</template>

<style scoped>

.social-container{
    display: flex;
    flex-direction: row;
    margin-left: 0.2em;
}

.social-navigation-container{
    display: flex;
    flex-direction: column;
    width: 10em;
    height: 60vh;
    border-right: 1px solid black;
}

.create-group-button{
    background-image: url('../assets/svg/Social/creategroup.svg');
    background-position: center;
    background-repeat: no-repeat;
    background-size: contain;
    background-color: #AFFFF5;
    border: 5px solid #01EED6;
    border-radius: 0.2em;
    width: 2em;
    height: 2em;
    margin-left: 0.2em;
}

.social-link:visited, .social-link{
    border: 5px solid #01EED6;
    border-radius: 0.2em;
    background-color: #AFFFF5;
    padding: 0.2em;
    margin: 0.2em;
    text-align: center;
    text-decoration: none;
    color: black;
    font-weight: bold;
    font-family: 'Tahoma';
}

.social-link:hover{
    background-color: #01EED6;
}

@media (max-width: 600px){
    .social-navigation-container{
        width: 5em;
    }
}

</style>