import { user } from '@/core/userInfo';
import { computed } from 'vue';

export async function signIn(username, password ) {
    const response = await fetch('/api/auth/signin', {
        method: 'POST',
        body: JSON.stringify({
            username: username,
            password: password,
          }),
        headers: {
            'Content-Type': 'application/json'
        }
    });
    
    if(response.ok){
        let responseData = await response.json();
        if(responseData.success)
        {
            let token = responseData.token;
            const parts = token.split('.');
            if (parts.length !== 3) {
                return null;
            }

            // Decode the payload
            const decoded = atob(parts[1]);
            const payload = JSON.parse(decoded);
            user.value.name = payload.unique_name;
            user.value.email = payload.email;
            localStorage.setItem('jwtToken', JSON.stringify(responseData.token));
        }
        return {...responseData};
    }

    return {sucess: false, message: 'Server error'};
}

export async function signUp(username, email, password) {
    const response = await fetch('/api/auth/signup', {
        method: 'POST',
        body: JSON.stringify({
            username: username,
            email: email,
            password: password,
          }),
        headers: {
            'Content-Type': 'application/json'
        }
    });
    
    if(response.ok){
        let responseData = await response.json();
        if(responseData.success){
            let token = responseData.token;
            const parts = token.split('.');
            if (parts.length !== 3) {
                return null;
            }

            // Decode the payload
            const decoded = atob(parts[1]);
            const payload = JSON.parse(decoded);

            user.value.name = payload.unique_name;
            user.value.email = payload.email;
            localStorage.setItem('jwtToken', JSON.stringify(responseData.token));
        }
        return {...responseData};
    }
    return {sucess: false, message: 'Server error'};
}

export function signout() {
    localStorage.removeItem('jwtToken');
}

export function getCurrentUser() {
    const token = JSON.parse(localStorage.getItem('jwtToken'));
    if (!token) {
        return null;
    }

    // Split the JWT string into three parts: header, payload, signature
    const parts = token.split('.');
    if (parts.length !== 3) {
        return null;
    }

    // Decode the payload
    const decoded = atob(parts[1]);
    const payload = JSON.parse(decoded);

    console.log(payload);

    user.value.name = payload.unique_name;
    user.value.email = payload.email;
    return payload;
}

export const isAuthenticated = computed(() => {
    return user.value.name !== '';
});