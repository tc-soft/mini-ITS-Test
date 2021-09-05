export const fetchWrapper = {
    get,
    post,
    put,
    delete: _delete,
    login,
    loginStatus,
    logout
}

function get(url, params) {
    //const urlParams = encodeQueryString(params);
    const requestOptions = {
        method: 'GET',
        headers: { 'Content-Type': 'application/json' }
    };
    //return fetch(`${url}${urlParams}`, requestOptions);
    return fetch(`${url}`, requestOptions);
}

function post(url, body) {
    const requestOptions = {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body)
    };
    return fetch(url, requestOptions);
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
    return fetch(url, requestOptions);
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

function loginStatus(url) {
    const requestOptions = {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        },
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

function encodeQueryString(params) {
    const keys = Object.keys(params)
    return keys.length
        ? "?" + keys
            .map(key => encodeURIComponent(key)
                + "=" + encodeURIComponent(params[key]))
            .join("&")
        : ""
}