<script setup>
import { user } from '@/core/userInfo';

import { useRouter } from 'vue-router';

const router = useRouter();

function logout(){
    localStorage.removeItem('jwtToken');
    user.value.name = '';
    user.value.email = ''; 
    router.push({ name: 'Login' });
}

</script>

<template>
<div class="main-container">
    <div class="container-logout">
        <h1>Logout</h1>
        <p>Are you sure you want to log out?</p>
        <button @click="logout">Logout</button>
    </div>
</div>
</template>

<style scoped>

.main-container{
    display: flex;
    width: 100%;
    height: 60vh;
    align-items: center;
    justify-content: center;
}

.container-logout{
    width: 60%;
    max-width: 600px;
    margin: 50px auto;
    padding: 20px;
    background-color: aqua; /* Aqua color */
    border-radius: 10px;
    box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
    text-align: center;
}

h1{
    color: #333;
}

p{
    color: #555;
    margin-bottom: 20px;
}

button{
    color: #fff;
    background-color: #007bff; /* Blue color for the link */
    padding: 10px 20px;
    border-radius: 5px;
    text-decoration: none;
    transition: background-color 0.3s ease;
    border: none;
}

button:hover {
    background-color: #0056b3; /* Darker blue on hover */
}

</style>