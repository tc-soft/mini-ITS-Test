import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';

import { usersServices } from '../../services/UsersServices';

function List({ match }) {
    const { path } = match;
    const [users, setUsers] = useState({
        results: '',
        currentPage: '',
        resultsPerPage: '',
        totalResults: '',
        totalPages: ''
    });

    useEffect(() => {
        setTimeout(() => {
            usersServices.getAll()
                .then(x => {
                    setUsers(x);
                    //console.table(x);
                });
        }, 3000);
    }, []);

    function deleteUser(id) {
        setUsers(users.map(x => {
            if (x.id === id) { x.isDeleting = true; }
            return x;
        }));
        usersServices.delete(id).then(() => {
            setUsers(users => users.filter(x => x.id !== id));
        });
    }

    return (
        <div>
            <h1>Users</h1>
            <Link to={`${path}/add`}>Add User</Link>
            <table>
                <thead>
                    <tr>
                        <th style={{ width: '05%' }}>Lp.</th>
                        <th style={{ width: '10%' }}>Login</th>
                        <th style={{ width: '20%' }}>Imie</th>
                        <th style={{ width: '20%' }}>Nazwisko</th>
                        <th style={{ width: '20%' }}>Dział</th>
                        <th style={{ width: '10%' }}>Rola</th>
                        <th style={{ width: '15%' }}></th>
                    </tr>
                </thead>
                <tbody>
                    {users.results && users.results.map((user, index) =>
                        <tr key={user.login}>
                            <td>{String("0" + index).slice(-2)}</td>
                            <td>{user.login}</td>
                            <td>{user.firstName}</td>
                            <td>{user.lastName}</td>
                            <td>{user.department}</td>
                            <td>{user.role}</td>
                            <td>
                                <Link to={`${path}/edit/${user.id}`}>Edit</Link>
                                <button onClick={() => deleteUser(user.id)} disabled={user.isDeleting}>
                                    {user.isDeleting
                                        ? <span></span>
                                        : <span>Delete</span>
                                    }
                                </button>
                            </td>
                        </tr>
                    )}
                    {!users.results &&
                        <tr>
                            <td>
                                <div>Ładuje dane...</div>
                            </td>
                        </tr>
                    }
                    {users.results && !users.results.length &&
                        <tr>
                            <td>
                                <div>No Users To Display</div>
                            </td>
                        </tr>
                    }
                </tbody>
            </table>
        </div>
    );
}

export { List };