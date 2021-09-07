import { fetchWrapper } from '../api/FetchWrapper';

const baseUrl = '/Users';

export const usersServices = {
    index,
    create,
    edit,
    update,
    delete: _delete,
    login,
    loginStatus,
    logout
};

function index(values) {
    return fetchWrapper.post(`${baseUrl}/Index`, values);
}

function create(values) {
    return fetchWrapper.post(`${baseUrl}/Create`, values);
}

function edit(id) {
    return fetchWrapper.get(`${baseUrl}/Edit/${id}`);
}

function update(id, values) {
    return fetchWrapper.put(`${baseUrl}/Edit/${id}`, values);
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