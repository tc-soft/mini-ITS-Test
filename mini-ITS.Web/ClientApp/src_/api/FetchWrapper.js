export const FetchWrapper = {
    Get,
    Post,
    Put,
    Delete
}

function Get(url) {
    const requestOptions = {
        method: 'GET'
    };
    return fetch(url, requestOptions).then(HandleResponse);
}

function Post(url, body) {
    const requestOptions = {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body)
    };
    return fetch(url, requestOptions).then(HandleResponse);
}

function Put(url, body) {
    const requestOptions = {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body)
    };
    return fetch(url, requestOptions).then(HandleResponse);    
}

function Delete(url) {
    const requestOptions = {
        method: 'DELETE'
    };
    return fetch(url, requestOptions).then(HandleResponse);
}

// helper functions
function HandleResponse(response) {
    return response.json()
        .then((responseData) => {
            return responseData;
        })
        .catch(error => {
                console.warn(error);
                Promise.reject(error);
            }
        );
}