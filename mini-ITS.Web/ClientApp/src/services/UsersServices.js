import { fetchWrapper } from '../api/FetchWrapper';

const baseUrl = '/Users';

export const usersServices = {
    getAll,
    getById,
    create,
    update,
    delete: _delete,
    login,
    loginStatus,
    logout
};

function getAll() {
    return fetchWrapper.get(`${baseUrl}/GetAll`);
}

function getById(id) {
    return fetchWrapper.Get(`${baseUrl}/${id}`);
}

function create(params) {
    return fetchWrapper.Post(baseUrl, params);
}

function update(id, params) {
    return fetchWrapper.Put(`${baseUrl}/${id}`, params);
}

function _delete(id) {
    return fetchWrapper.Delete(`${baseUrl}/${id}`);
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