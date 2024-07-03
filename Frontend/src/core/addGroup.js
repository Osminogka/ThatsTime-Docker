export const getGroupList = async (page) => {
    let response = await fetch("/api/groups/getgroups?page="+page,{
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

export const getGroupByName = async (name) => {
    let response = await fetch("/api/groups/getcertaingroup?groupname="+name,{
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

export const sendGroupInvite = async () => {

}

export const createGroup = async (name) => {
    let response = await fetch("/api/groups/create?groupname=" + name,{
        method: 'POST',
        headers: {
            'Authorization': 'Bearer ' + localStorage.getItem('jwtToken').replace(/"/g, ''),
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

export const enterTheGroup = async (name) => {
    let response = await fetch("/api/groups/enter?groupname=" + name,{
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

export const leaveTheGroup = async (name) => {
    let response = await fetch("/api/groups/leave?groupname=" + name,{
        method: 'GET',
        headers: {
            'Authorization': 'Bearer ' + localStorage.getItem('jwtToken').replace(/"/g, ''),
            'Content-Type': 'application/json'
        }
    });
    if(response.success){
        let responseData = await response.json();
        return responseData;
    }
    else
        return {success: false, message: 'Server error'};
}