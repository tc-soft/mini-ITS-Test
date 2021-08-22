import { createContext, useContext, useState } from 'react';
import { useHistory } from "react-router-dom";
import { usersServices } from '../services/UsersServices';

const AuthContext = createContext();

export function useAuth() {
    return useContext(AuthContext);
}

export default function AuthProvider({ children }) {
    const [currentUser, setCurrentUser] = useState(null);
    const [loginStatus, setLoginStatus] = useState(false);
    const history = useHistory();


    if (!currentUser && !loginStatus) {
        setLoginStatus(true);

        if (window.performance) {
            if (performance.navigation.type === 1) {
                //This page is reloaded
                usersServices.loginStatus()
                    .then((response) => {
                        if (response.ok) {
                            return response.json()
                                .then((data) => {
                                    console.log("currentUser : check login status");
                                    setCurrentUser(data);
                                })
                        }
                    });
            } else {
                //This page is not reloaded
                console.log("currentUser : check login status - not reloaded");
            }
        }
    }

    console.log(`currentUser : ${currentUser}, loginStatus : ${loginStatus}`);

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
                                history.push("/");
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