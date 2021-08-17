import { createContext, useContext, useState } from 'react';

const AuthContext = createContext({
    currentUser: {
        login: '',
        firstName: '',
        lastName: '',
        department: '',
        role: '',
        isLogged: false
    },
    handleLogin: () => { },
    handleLogout: () => { }
});

export function useAuth() {
    return useContext(AuthContext);
}

export default function AuthProvider({ children }) {
    const [currentUser, setCurrentUser] = useState(
        {
            login: '',
            firstName: '',
            lastName: '',
            department: '',
            role: '',
            isLogged: false
        })

    const handleLogin = (user) => {
        try {
            setCurrentUser(user);
        }
        catch (error) {
            console.log("Error while logging", error.message);
        }
    };

    const handleLogout = () => {
        try {
            setCurrentUser([null]);
            console.table(currentUser);
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