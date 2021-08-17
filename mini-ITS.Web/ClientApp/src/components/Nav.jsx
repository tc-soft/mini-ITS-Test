import React from 'react';
import { NavLink } from 'react-router-dom';
import { useAuth } from './AuthProvider';

function Nav() {
    const { currentUser, handleLogin, handleLogout } = useAuth();
    return (
        <nav className="">
            <div className="">
                <NavLink exact to="/" className="nav-item nav-link">Home</NavLink>
                &nbsp;
                <NavLink exact to="/Homero" className="nav-item nav-link">Homero</NavLink>
                &nbsp;
                <NavLink exact to="/Homero2" className="nav-item nav-link">Homero2</NavLink>
                &nbsp;
                <NavLink exact to="/Login" className="nav-item nav-link">Login</NavLink>
                &nbsp;
                <NavLink to="/test" className="nav-item nav-link">Users</NavLink>

                <h5>
                    Hi { currentUser ? currentUser : "stranger"}
                </h5>

                <button onClick={() => handleLogin("David")}>Login David</button>
                <button onClick={() => handleLogin("Zenek")}>Login Zenek</button>
                <button onClick={() => handleLogout()}>Logout</button>


            </div>
        </nav>
    );
}

export default Nav;