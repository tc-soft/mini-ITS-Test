import React from 'react';
import { NavLink } from 'react-router-dom';
import { useAuth } from './AuthProvider';

function Nav() {
    const { currentUser, handleLogout } = useAuth();

    return (
        <nav>
            <div>
                <NavLink exact to='/'>
                    <button type='button'>
                        Home
                    </button>
                </NavLink>
                &nbsp;
                <NavLink exact to='/Login'>
                    <button type='button'>
                        Login
                    </button>
                </NavLink>
                &nbsp;
                {currentUser
                    ?
                    <NavLink to='/Users'>
                        <button type='button'>
                            Users
                        </button>
                    </NavLink>
                    :
                    null
                }

                {currentUser ?
                    (
                        <>
                                <p>Login      : {currentUser.login}</p>
                                <p>firstName  : {currentUser.firstName}</p>
                                <p>lastName   : {currentUser.lastName}</p>
                                <p>Department : {currentUser.department}</p>
                                <p>Role       : {currentUser.role}</p>
                                <br/>
                        </>
                    )
                    :
                    (
                        <>
                            <p>Login      :</p>
                            <p>firstName  :</p>
                            <p>lastName   :</p>
                            <p>Department :</p>
                            <p>Role       :</p>
                            <br />
                        </>
                    )
                }

                <button onClick={() => handleLogout()}>Logout</button>

            </div>
        </nav>
    );
}

export default Nav;