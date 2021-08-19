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
                <NavLink exact to="/Login" className="nav-item nav-link">Login</NavLink>
                &nbsp;
                <NavLink to="/test" className="nav-item nav-link">Users</NavLink>

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
                                {/*<p>isLogged   : {currentUser.isLogged.toString()}</p>*/}
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
                            {/*<p>isLogged   : {currentUser.isLogged.toString()}</p>*/}
                            <br />
                        </React.Fragment>
                    )
                }

                <button onClick={() => handleLogin("David")}>Login David</button>
                <button onClick={() => handleLogin("Zenek")}>Login Zenek</button>
                <button onClick={() => handleLogout()}>Logout</button>


            </div>
        </nav>
    );
}

export default Nav;