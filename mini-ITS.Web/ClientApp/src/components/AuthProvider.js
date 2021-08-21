import { createContext, useContext, useState } from 'react';
import { usersServices } from '../services/UsersServices';

//login: '',
//    firstName: '',
//        lastName: '',
//            department: '',
//                role: '',
//                    isLogged: false

const AuthContext = createContext({
    currentUser: {},
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
            usersServices.logout()
                .then((response) => {
                    if (response.ok) {
                        return response.json()
                            .then((data) => {
                                setCurrentUser(null);
                                console.info(data);
                            })
                    } else {
                        return response.json()
                            .then((data) => {
                                console.warn(data);
                            })
                    }
                })
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