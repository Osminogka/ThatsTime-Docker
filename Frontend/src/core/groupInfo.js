export const inviteFriendToGroup = async (friendName, groupName) => {
    let searchParams = new URLSearchParams({
        friendName, groupName
    });
    let response = await fetch(`/api/groups/sendinvite?${searchParams}`,{
        method: 'GET',
        headers: {'Authorization': 'Bearer ' + localStorage.getItem('jwtToken').replace(/"/g, ''),
                    'Content-Type': 'application/json'
    }
    });
    if(response.ok)
    {
        let responseData = await response.json();
        return responseData;
    }
    else
        return {success: false, message: 'Server error'};
}

export const removeMemberFromGroup = async (groupName, friendName) => {
    let searchParams = new URLSearchParams({
        groupName, friendName
    });
    let response = await fetch(`/api/groups/remove?${searchParams}`,{
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

export const promoteMemberInGroup = async (groupName, memberName) => {
    let searchParams = new URLSearchParams({
        groupName, memberName
    })
    let response = await fetch(`/api/groups/promote?${searchParams}`,{
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

export const demoteMemberInGroup = async (groupName, memberName) => {
    let searchParams = new URLSearchParams({
        groupName, memberName
    })
    let response = await fetch(`/api/groups/demote?${searchParams}`,{
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

export const acceptGroupRequest = async (groupname) => {
    let response = await fetch("/api/groups/acceptinvite?groupName=" + groupname,{
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

export const declineGroupRequest = async (groupname) => {
    let response = await fetch("/api/groups/declineinvite?groupName=" + groupname,{
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

export const deleteGroup = async (groupname) => {
    let response = await fetch("/api/groups/deletegroup?groupName=" + groupname,{
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

export const leaveGroup = async (groupname) => {
    let response = await fetch("/api/groups/leave?groupName=" + groupname,{
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