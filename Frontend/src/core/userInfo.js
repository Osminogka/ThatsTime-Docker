import { ref } from 'vue';

export const user = ref({
    name: '',
    email: ''
});
  
export const friendList = ref([

]);
  
export const groupList = ref([

]);
  
export const friendRequests = ref([

]);
  
export const groupInvites = ref([

]);

export const getMyFriendList = async () => {
    let response = await fetch("/api/friends/getfriends",{
        method: 'GET',
        headers: {
            'Authorization': 'Bearer ' + localStorage.getItem('jwtToken').replace(/"/g, ''),
            'Content-Type': 'application/json'
        }
    });
    if(response.ok){
        let responseData = await response.json();
        if(responseData.success)
            friendList.value = responseData.friendList;
    }
    else
        return {success: false, message: 'Server error'};
}

export const getMyGroupList = async () => {
    let response = await fetch("/api/groups/getmygroups",{
        method: 'GET',
        headers: {
            'Authorization': 'Bearer ' + localStorage.getItem('jwtToken').replace(/"/g, ''),
            'Content-Type': 'application/json'
        }
    });
    if(response.ok){
        let responseData = await response.json();
        if(responseData.success)
            groupList.value = responseData.groups;
        else
            groupList.value = [];
    }
    else
        groupList.value = [];
}

export const getMyFriendRequests = async () => {
    let response = await fetch("/api/friends/getinvites",{
        method: 'GET',
        headers: {
            'Authorization': 'Bearer ' + localStorage.getItem('jwtToken').replace(/"/g, ''),
            'Content-Type': 'application/json'
        }
    });
    if(response.ok){
        let responseData = await response.json();
        return responseData;
    }
    else
        return {success: false, message: 'Server error'};
}

export const getMyGroupInvites = async () => {
    let response = await fetch("/api/groups/getinvites",{
        method: 'GET',
        headers: {'Authorization': 'Bearer ' + localStorage.getItem('jwtToken').replace(/"/g, ''),
        'Content-Type': 'application/json'
    }
    });
    if(response.ok){
        let responseData = await response.json();
        return responseData;
    }
    else
        return {success: false, message: 'Server error'};
}

export const acceptFriendRequest = async (friendname) => {
    let response = await fetch("/api/friends/acceptinvite?friendName=" + friendname,{
        method: 'GET',
        headers: {'Authorization': 'Bearer ' + localStorage.getItem('jwtToken').replace(/"/g, ''),
        'Content-Type': 'application/json'
    }
    });
    if(response.ok){
        let responseData = await response.json();
        return responseData;
    }
    else
        return {success: false, message: 'Server error'};
}

export const declineFriendRequest = async (friendname) => {
    let response = await fetch("/api/friends/declineinvite?friendName=" + friendname,{
        method: 'GET',
        headers: {'Authorization': 'Bearer ' + localStorage.getItem('jwtToken').replace(/"/g, ''),
        'Content-Type': 'application/json'
    }
    });
    if(response.ok){
        let responseData = await response.json();
        return responseData;
    }
    else
        return {success: false, message: 'Server error'};
}

export const deleteFriend = async (friendname) => {
    let response = await fetch("/api/friends/deletefriend?friendName=" + friendname,{
        method: 'GET',
        headers: {
        'Authorization': 'Bearer ' + localStorage.getItem('jwtToken').replace(/"/g, ''),
        'Content-Type': 'application/json'
    }
    });
    if(response.ok){
        let responseData = await response.json();
        return responseData;
    }
    return {success: false, message: 'Server error'};
}