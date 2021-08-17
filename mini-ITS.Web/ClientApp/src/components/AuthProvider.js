import { createContext, useContext, useState } from 'react';

const AuthContext = createContext({
    currentUser: {
        login: '',
        role: ''
    },
    handleLogin: () => { },
    handleLogout: () => { }
});

export function useAuth() {
    return useContext(AuthContext);
}

export default function AuthProvider({ children }) {
    const [currentUser, setCurrentUser] = useState();

    const handleLogin = (user) => {
        try {
            setCurrentUser({
                login: 'user'
            });
        }
        catch (error) {
            console.log("Error while logging", error.message);
        }
    };

    const handleLogout = () => {
        try {
            setCurrentUser(null);
        }
        catch (error) {
            console.log("Error while logging out", error.message);
        }
    };

    return (
        <AuthContext.Provider value={{currentUser, handleLogin, handleLogout}}>
            {children}
        </AuthContext.Provider>
    );
}