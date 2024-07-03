import { createRouter, createWebHistory } from "vue-router";

import { getCurrentUser } from '@/core/authentication'; 

const routes = [
    {
        path: "/login",
        name: "Login",
        component: () => import("../components/LoginPage.vue"),
        meta:{
            title: "Login",
            requireAuth: false
        }
    },
    {
        path: "/register",
        name: "Register",
        component : () => import("../components/RegisterPage.vue"),
        meta:{
            title: "Register",
            requireAuth: false
        }
    },
    {
        path: "/logout",
        name: "Logout",
        component: () => import("../components/LogoutPage.vue"),
        meta:{
            title: "Logout",
            requireAuth: true
        }
    },
    {
        path: "/profile",
        name: "Profile",
        component: () => import("../components/ProfilePage.vue"),
        meta: {
            title: "Profile",
            requireAuth: true
        }
    },
    {
        path: "/social",
        name: "AddSocial",
        component: () => import("../components/AddSocial.vue"),
        children:[
            {
                path: "addfriend",
                name: "AddFriend",
                component: () => import("../components/AddFriend.vue"),
                meta:{
                    title: "Add Friend",
                    requireAuth: true
                }
            },
            {
                path: "addgroup",
                name: "AddGroup",
                component: () => import("../components/AddGroup.vue"),
                meta: {
                    title: "Add Group",
                    requireAuth: true
                }
            },
            {
                path:"creategroup",
                name: "CreateGroup",
                component: () => import("../components/CreateGroup.vue"),
                meta: {
                    title: "Create Group",
                    requireAuth: true
                }
            }
        ],
        meta:{
            title: "Add Social",
            requireAuth: true
        }
    },
    {
        path: "/",
        name: "Mainpage",
        component: () => import("../components/MainTable.vue"),
        meta:{
            title: "Mainpage",
            requireAuth: true
        }
    },
    {
        path: "/records",
        name: "Records",
        component: () => import("../components/AllUserRecords.vue"),
        meta:{
            title: "Records",
            requireAuth: true
        }
    },
    {
        path: "/friend/:nickname",
        name: "Friend",
        component: () => import("../components/MoreFriend.vue"),
        meta:{
            title: "Friend",
            requireAuth: true
        }
    },
    {
        path: "/group/:groupname",
        name: "Group",
        component: () => import("../components/MoreGroup.vue"),
        meta:{
            title: "Group",
            requireAuth: true
        }
    },
    {
        path: "/:pathMatch(.*)*",
        name: "NotFound",
        component: () => import("../components/NotFound.vue"),
        meta:{
            title: "Not Found",
            requireAuth: false
        }
    
    }
];

const router = createRouter({
    history: createWebHistory(),
    routes
});

router.beforeEach((to) => {
    const loggedIn = getCurrentUser();
    
    if(to.meta.requireAuth && !loggedIn){
        return {name: "Login", query: {redirect: to.fullPath}};
    }
});

router.beforeEach((to, from, next) => {
    document.title = to.meta.title || 'Default Title';
    next();
});

export default router;