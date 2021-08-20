import React from 'react';
import { NavLink } from 'react-router-dom';
import { useAuth } from './AuthProvider';

function Nav() {
    const { currentUser, handleLogout } = useAuth();
    return (
        <nav className="">
            <div className="">
                <NavLink exact to="/" className="nav-item nav-link">
                    <button type="button">
                        Home
                    </button>
                </NavLink>
                &nbsp;
                <NavLink exact to="/Login" className="nav-item nav-link">
                    <button type="button">
                        Login
                    </button>
                </NavLink>
                &nbsp;
                <NavLink to="/Users" className="nav-item nav-link">
                    <button type="button">
                        Users
                    </button>
                </NavLink>

                <h5>
                    {currentUser ? null : "Unknown"}
                    {/*{currentUser == null ? null : "Unknown"}*/}
                </h5>

                {currentUser ?
                    (
                        <React.Fragment>
                                <p>Login      : {currentUser.login}</p>
                                <p>firstName  : {currentUser.firstName}</p>
                                <p>lastName   : {currentUser.lastName}</p>
                                <p>Department : {currentUser.department}</p>
                                <p>Role       : {currentUser.role}</p>
                                <br/>
                        </React.Fragment>
                    )
                    :
                    (
                        <React.Fragment>
                            <p>Login      :</p>
                            <p>firstName  :</p>
                            <p>lastName   :</p>
                            <p>Department :</p>
                            <p>Role       :</p>
                            <br />
                        </React.Fragment>
                    )
                }

                <button onClick={() => handleLogout()}>Logout</button>

            </div>
        </nav>
    );
}

export default Nav;