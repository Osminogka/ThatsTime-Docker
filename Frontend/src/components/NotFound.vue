<template>
    <div class="not-found">
        <h1>404</h1>
        <h2>Page Not Found</h2>
        <p>Oops! The page you are looking for does not exist.</p>
        <router-link class="back-home" to="/">Go back to home</router-link>
    </div>
</template>

<style scoped>
.not-found {
    text-align: center;
    margin-top: 100px;
}

h1 {
    font-size: 72px;
    margin-bottom: 20px;
}

h2 {
    font-size: 36px;
    margin-bottom: 20px;
}

p {
    font-size: 18px;
    margin-bottom: 20px;
}

.c {
    color: #007bff;
    text-decoration: none;
    font-size: 18px;
}
</style>