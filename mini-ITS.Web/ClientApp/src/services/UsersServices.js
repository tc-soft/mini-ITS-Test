import { fetchWrapper } from '../api/FetchWrapper';

const baseUrl = '/Users';

export const usersServices = {
    index,
    edit,
    create,
    update,
    delete: _delete,
    login,
    loginStatus,
    logout
};

function index(params) {
    return fetchWrapper.post(`${baseUrl}/Index`, params);
}

function edit(id) {
    return fetchWrapper.get(`${baseUrl}/Edit/${id}`);
}

function create(params) {
    return fetchWrapper.post(`${baseUrl}/Create`, params);
}

function update(id, params) {
    return fetchWrapper.put(`${baseUrl}/${id}`, params);
}

function _delete(id) {
    return fetchWrapper.delete(`${baseUrl}/Delete/${id}`);
}

function login(login, password) {
    return fetchWrapper.login(`${baseUrl}/Login`, login, password);
}

function loginStatus() {
    return fetchWrapper.loginStatus(`${baseUrl}/LoginStatus`);
}

function logout() {
    return fetchWrapper.logout(`${baseUrl}/Logout`);
}