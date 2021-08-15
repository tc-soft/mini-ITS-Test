import React from 'react';
import { NavLink } from 'react-router-dom';

function Nav() {
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
            </div>
        </nav>
    );
}

export default Nav;