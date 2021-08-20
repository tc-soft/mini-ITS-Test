export const fetchWrapper = {
    get,
    post,
    put,
    delete: _delete,
    login,
    logout
}

function get(url) {
    const requestOptions = {
        method: 'GET'
    };
    return fetch(url, requestOptions).then(handleResponse);
}

function post(url, body) {
    const requestOptions = {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body)
    };
    return fetch(url, requestOptions).then(handleResponse);
}

function put(url, body) {
    const requestOptions = {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body)
    };
    return fetch(url, requestOptions).then(handleResponse);    
}

function _delete(url) {
    const requestOptions = {
        method: 'DELETE'
    };
    return fetch(url, requestOptions).then(handleResponse);
}

function login(url, login, password) {
    const requestOptions = {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        },
        body: JSON.stringify({
            login: login,
            password: password
        })
    };
    return fetch(url, requestOptions);
}

function logout(url) {
    const requestOptions = {
        method: 'DELETE',
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        }
    };
    return fetch(url, requestOptions);
}

function handleResponse(response) {
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