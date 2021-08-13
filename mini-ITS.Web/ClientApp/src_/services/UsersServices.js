import { FetchWrapper } from '../api/FetchWrapper';

const baseUrl = '/test';

export const UsersServices = {
    GetAll,
    GetById,
    Create,
    Update,
    Delete
};

function GetAll() {
    return FetchWrapper.Get(baseUrl);
}

function GetById(id) {
    return FetchWrapper.Get(`${baseUrl}/${id}`);
}

function Create(params) {
    return FetchWrapper.Post(baseUrl, params);
}

function Update(id, params) {
    return FetchWrapper.Put(`${baseUrl}/${id}`, params);
}

function Delete(id) {
    return FetchWrapper.Delete(`${baseUrl}/${id}`);
}