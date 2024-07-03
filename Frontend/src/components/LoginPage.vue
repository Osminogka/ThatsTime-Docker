<script setup>
import { signIn } from '@/core/authentication';

import { ref } from 'vue';
import { useRouter } from 'vue-router';

const router = useRouter();

const username = ref('');
const password = ref('');
const error = ref('');

async function login(){
  error.value = '';
  let result = await signIn(username.value, password.value);
  if(!result.success)
    error.value = 'Invalid username or password';
  else
    router.push({ name: 'Mainpage' });
}

</script>

<template>
<div class="login-container">
    <div class="login-form">
        <div class="login-header">That's Time</div>
        <h1>Login</h1>
        <p style="color: red;">{{ error }}</p>
        <form>
            <input v-model="username" type="text" placeholder="Username" required>
            <input v-model="password" type="password" placeholder="Password" required>
            <button type="submit" @click.prevent="login()">Login</button>
        </form>
        <router-link :to="{ name: 'Register' }" class="register-link">Register here</router-link>
    </div>
</div>
</template>

<style scoped>

.login-container {
  display: flex;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
  background-color: white; /* Aqua background color */
  margin: 0;
  padding: 0;
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
  width: 100vw;
}

.login-form {
  margin: auto;
  background-color: #bff9fb;
  border-radius: 8px;
  padding: 20px;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
  text-align: center;
}

.login-form h1 {
  font-family: 'Roboto', 'Arial', sans-serif;
  color: #333; /* Dark text color */
  margin-bottom: 20px;
}

.login-form input {
  width: calc(100% - 20px);
  padding: 10px;
  margin: 10px 0;
  border: 1px solid #ccc;
  border-radius: 4px;
  box-sizing: border-box;
  font-size: 16px;
}

.login-form button {
  width: calc(100% - 20px);
  padding: 10px;
  margin: 10px 0;
  border: none;
  border-radius: 4px;
  background-color: #40e0d0; /* Green button color */
  color: #fff; /* White text color */
  font-size: 16px;
  cursor: pointer;
}

.login-form button:hover {
  background-color: #31aca0; /* Darker green on hover */
}

.register-link {
  color: #009688; /* Aqua color for link */
  text-decoration: none;
  margin-top: 10px;
  display: block;
}

.login-header{
  font-family: 'Brush Script MT', cursive;
  justify-self: center;
  margin-right: auto;
  margin-left: auto;
  text-shadow: 3px 4px 0px rgba(0,0,0,0.3);
  font-size: 2em;
}


</style>