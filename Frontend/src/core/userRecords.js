import { friendList, groupList, user } from "./userInfo";
import { shortMonthNames, todayDate } from "./month";

export const getRecordsFromLocal = (records, date) => {
    let startDay = Math.min(date, todayDate.getDate());
    let endDay = Math.max(date, todayDate.getDate());
    return records.filter(record => record.selectedMonth === todayDate.getMonth() + 1 && record.selectedYear === todayDate.getFullYear() 
        && (todayDate.getDate() == date? record.selectedDay === date : record.selectedDay > startDay && record.selectedDay < endDay));
}

export const getRecords = async (date) => {
    let searchParams = new URLSearchParams({
        ...date
    });
    let response = await fetch(`/api/records/recent?${searchParams}`, {
        method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + localStorage.getItem('jwtToken').replace(/"/g, ''),
                'Content-Type': 'application/json'
            }
    });
    if(response.ok) {
        let responseData = await response.json();
        return responseData;
    }
    return {success: false, message: 'Server error'};
}

export const getCertainRecord = async (CertainRecord) =>  {
    CertainRecord.month = shortMonthNames.indexOf(CertainRecord.month) + 1;
    let searchParams = new URLSearchParams({
        ...CertainRecord
    });
    let response = await fetch(`/api/records/certain?${searchParams}`,
        {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + localStorage.getItem('jwtToken').replace(/"/g, ''),
                'Content-Type': 'application/json'
            }
        }
    );
    if(response.ok) {
        let responseData = await response.json();
        return responseData;
    }
    else
        return {success: false, message: 'Server error'};
}

export const getRecordsWithFriend = async (friendName,date) => {
    let searchParams = new URLSearchParams({
        friendName,
        ...date
    });
    let response = await fetch(`/api/records/friend?${searchParams}`,{
        method: 'GET',
        headers: {
            'Authorization': 'Bearer ' + localStorage.getItem('jwtToken').replace(/"/g, ''),
            'Content-Type': 'application/json'
        }
    });
    if(response.ok) {
        let responseData = await response.json();
        return responseData;
    }
    else
        return {success: false, message: 'Server error'};
}

export const getRecordsWithGroup = async (groupName, date) => {
    let searchParams = new URLSearchParams({
        groupName,
        ...date
    });
    let response = await fetch(`/api/records/groupinfo?${searchParams}`,{
        method: 'GET',
        headers: {
            'Authorization': 'Bearer ' + localStorage.getItem('jwtToken').replace(/"/g, ''),
            'Content-Type': 'application/json'
        }
    });
    if(response.ok) {
        let responseData = await response.json();
        return responseData;
    }
    else
        return {success: false, message: 'Server error'};
}

export const postRecord = async (record) => {
    let errorList = isRecordValid(record);
    if(!errorList.length == 0) {
        return {success: false, message: 'Invalid record', records: errorList};
    }

    const recordCreationFromFrontEnd = {
        selectedYear: record.selectedYear,
        selectedMonth: record.selectedMonth + 1,
        selectedDay: record.selectedDay,
        showGroupList: record.showGroupList,
        yourSelf: record.yourSelf,
        Creator: user.value.name,
        selectedObject: record.yourSelf ? user.value.name : record.selectedObject,
        importance: record.importance,
        hour: record.hour,
        minute: record.minute,
        recordName: record.recordName,
        recordContent: record.recordContent
    };
    let response = await fetch('/api/records/newrecord',
        {
            method: 'POST',
            headers: {
                'Authorization': 'Bearer ' + localStorage.getItem('jwtToken').replace(/"/g, ''),
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(recordCreationFromFrontEnd)
        });
    if(response.ok) {
        let responseData = await response.json();
        return responseData;
    }
    else
        return {success: false, message: 'Server error', records: errorList};
}

function isRecordValid (record) {
    let errorList = [];

    if (!(todayDate.getFullYear() - 10 <= record.selectedYear && record.selectedYear <= todayDate.getFullYear() + 10)) {
        errorList.push('selectedYear');
    }
    // Check if selectedDay is a valid day
    if (!(1 <= record.selectedDay && record.selectedDay <= new Date(record.selectedYear, record.selectedMonth + 1, 0).getDate())) {
        errorList.push('selectedDay');
    }
    // Check if selectedMonth is a valid month
    if (!(0 <= record.selectedMonth && record.selectedMonth <= 11)) {
        errorList.push('selectedMonth');
    }
    // Check if selectedObject is a valid object
    if (record.yourSelf? !(record.selectedObject == user.value.name) 
        : record.showGroupList ? !groupList.value.some(obj => obj.name === record.selectedObject) : !friendList.value.some(obj => obj.name === record.selectedObject)) {
        //errorList.push('selectedObject');
    }
    // Check if hour is a valid hour
    if (!(0 <= parseInt(record.hour) && parseInt(record.hour) <= 23)) {
        errorList.push('hour');
    }
    // Check if minute is a valid minute
    if (!(0 <= parseInt(record.minute) && parseInt(record.minute) <= 59)) {
        errorList.push('minute');
    }
    // Check if recordName is not empty
    if (record.recordName.length <= 0 || record.recordName.length > 50) {
        errorList.push('recordName');
    }
    // Check if recordContent is not empty
    if (record.recordContent.length <= 0 || record.recordContent.length > 500) {
        errorList.push('recordContent');
    }

    return errorList;
}