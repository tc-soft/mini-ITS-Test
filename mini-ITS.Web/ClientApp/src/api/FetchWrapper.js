export const fetchWrapper = {
    get,
    post,
    put,
    patch,
    delete: _delete,
    login,
    loginStatus,
    logout
}

function get(url, params) {
    const urlParams = encodeQueryString(params);
    const requestOptions = {
        method: 'GET',
        headers: { 'Content-Type': 'application/json' }
    };
    return fetch(`${url}${urlParams}`, requestOptions);
    //return fetch(`${url}`, requestOptions);
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
    return fetch(url, requestOptions);
}

function patch(url, body) {
    const requestOptions = {
        method: 'PATCH',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(body)
    };
    return fetch(url, requestOptions);
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

//function handleResponse(response) {
//    return response.json()
//        .then((responseData) => {
//            return responseData;
//        })
//        .catch(error => {
//                console.warn(error);
//                Promise.reject(error);
//            }
//        );
//}

function encodeQueryString(params) {
    const paramsKeys = Object.keys(params)
    var results = "";

    if (paramsKeys.length) {
        results = "?" + paramsKeys
            .map(key => {
                const filters = params[key]
                if (typeof filters === 'object' && filters !== null) {
                    return filters
                        .map((element, index) => {
                                const paramsFilter = Object.keys(element)
                            return `${key}[${index}].${paramsFilter[0]}=` + element.name + '&' +
                                    `${key}[${index}].${paramsFilter[1]}=` + element.operator + '&' +
                                    `${key}[${index}].${paramsFilter[2]}=String : ` + element.value
                        })
                        .join('&')
                }
                else {
                    return key + "=" + params[key]
                }
            }
            )
            .join('&')
    }

    return results;

    //const jsonParams = JSON.stringify(params);
    //return params
    //    ? '?' + jsonParams
    //    : "";

    //const keys = Object.keys(params)
    //return keys.length
    //    ? {
    //        "?" + keys
    //            .map(key => {
    //                if (key.length) {
    //                    return key.map(object =>
    //                        encodeURIComponent(object + "[]") + "=" + encodeURIComponent(params[key])
    //                        )
    //                }
    //                    return encodeURIComponent(key) + "=" + encodeURIComponent(params[key])
    //                }
    //            )
    //            .join("&")
    //    }
    //    : ""
}