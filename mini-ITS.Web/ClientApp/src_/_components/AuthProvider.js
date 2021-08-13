import { createContext, useContext } from 'react';

const AuthContext = createContext({
    currentUser: null,
    handleLogout: (e) => {
        e.preventDefault();
    }
});

export function useAuth() {
    return useContext(AuthContext);
}