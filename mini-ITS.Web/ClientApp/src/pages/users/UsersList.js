import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';

import { usersServices } from '../../services/UsersServices';

function UsersList(props) {
    const { match,
        pagedQuery,
        setPagedQuery,
        activeDepartmentFilter,
        setActiveDepartmentFilter,
        activeRoleFilter,
        setActiveRoleFilter
    } = props;

    const [users, setUsers] = useState({
        results: null,
        currentPage: null,
        resultsPerPage: null,
        totalResults: null,
        totalPages: null
    });
    
    useEffect(() => {
        setTimeout(() => {
            usersServices.index(pagedQuery)
                .then((response) => {
                    if (response.ok) {
                        return response.json()
                            .then((data) => {
                                setUsers(data);
                            })
                    } else {
                        return response.json()
                            .then((data) => {
                                console.log(data);
                            })
                    }
                })
        }, 0);
    }, [pagedQuery, activeDepartmentFilter, activeRoleFilter]);

    function deleteUser(id, login) {
        var answer = window.confirm(`Kasować użytkownika ${login} ?`);

        if (answer) {
            setUsers(prevState => ({
                ...prevState,
                results: users.results.map(x => {
                    if (x.id === id) { x.isDeleting = true; }
                    return x
                })
            }));

            usersServices.delete(id)
                .then((response) => {
                    if (response.ok) {
                        setPagedQuery({
                            ...pagedQuery
                        });
                    } else {
                        return response.text()
                            .then((data) => {
                                //tu dodać rsjx event
                                alert(data);
                            })
                    }
                })
                .catch(error => {
                    console.error('Error: ', error);
                });
        }
    }

    function handleResetFilter() {
        setPagedQuery(prevState => ({
            ...prevState,
            filter: null,
            page: 1
        }));

        setActiveDepartmentFilter(null);
        setActiveRoleFilter(null);
    }

    function handleDepartmentFilter(department) {
        setPagedQuery(prevState => ({
            ...prevState,
            filter: [{
                name: "Department",
                operator: "=",
                value: department
            }],
            page: 1
        }));

        setActiveDepartmentFilter(department);
    }

    function handleRoleFilter(role) {
        setPagedQuery(prevState => ({
            ...prevState,
            filter: [{
                name: "Role",
                operator: "=",
                value: role
            }],
            page: 1
        }));

        setActiveRoleFilter(role);
    }

    function handleFirstPage() {
        setPagedQuery(prevState => ({
            ...prevState,
            page: 1
        }));
    }

    function handlePrevPage() {
        var prevPage = users.currentPage;
        if (users.currentPage > 1) {
            prevPage--;
        }

        setPagedQuery(prevState => ({
            ...prevState,
            page: prevPage
        }));
    }

    function handleNextPage() {
        var nextPage = users.currentPage;
        if (users.currentPage < users.totalPages) {
            nextPage++;
        }

        setPagedQuery(prevState => ({
            ...prevState,
            page: nextPage
        }));
    }

    function handleLastPage() {
        setPagedQuery(prevState => ({
            ...prevState,
            page: users.totalPages
        }));
    }

    return (
        <React.Fragment>
            <button
                onClick={() => { handleDepartmentFilter("IT") }}
                disabled={activeDepartmentFilter === "IT" ? true : false}
            >
                Department: IT
            </button>
            <button
                onClick={() => { handleDepartmentFilter("Sales") }}
                disabled={activeDepartmentFilter === "Sales" ? true : false}
            >
                Department: Sales
            </button>
            <button
                onClick={() => { handleRoleFilter("User") }}
                disabled={activeRoleFilter === "User" ? true : false}
            >
                Role: User
            </button>
            <button
                onClick={() => { handleResetFilter() }}
            >
                Filter: RESET
            </button>
            <h1>Users</h1>
            <Link to={`${match.path}/Create`}>Dodaj</Link>
            <table>
                <thead>
                    <tr>
                        <th style={{ width: '05%' }}>Lp.</th>
                        <th style={{ width: '20%' }}>Login</th>
                        <th style={{ width: '20%' }}>Imie</th>
                        <th style={{ width: '20%' }}>Nazwisko</th>
                        <th style={{ width: '10%' }}>Dział</th>
                        <th style={{ width: '10%' }}>Rola</th>
                        <th style={{ width: '15%' }}></th>
                    </tr>
                </thead>
                <tbody>
                    {users.results && users.results.map((user, index) => {
                        const record = index + ((users.currentPage - 1) * users.resultsPerPage) + 1;
                        return (
                            <tr key={index}>
                                <td>{String("0" + (record)).slice(-2)}</td>
                                <td>{user.login}</td>
                                <td>{user.firstName}</td>
                                <td>{user.lastName}</td>
                                <td>{user.department}</td>
                                <td>{user.role}</td>
                                <td>
                                    <button>
                                        <Link to={`${match.path}/edit/${user.id}`}>Edit</Link>
                                    </button>

                                    <button onClick={() => deleteUser(user.id, user.login)} disabled={user.isDeleting}>
                                        Delete
                                    </button>
                                </td>
                                </tr>
                        )
                    }
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
            <br/>
            <div>
                <p>Strona z odczytu: {users.currentPage}</p>
                <p>Strona z query: {pagedQuery.page}</p>
                <p>Wszystkich stron: {users.totalPages}</p>

                <button
                    onClick={() => { handleFirstPage() }}
                    disabled={users.currentPage <= 1 ? true : false}
                >
                    &#60;&#60;
                </button>


                <button
                    onClick={() => { handlePrevPage() }}
                    disabled={users.currentPage <= 1 ? true : false}
                >
                    &#60;
                </button>

                <button
                    onClick={() => { handleNextPage() }}
                    disabled={users.currentPage >= users.totalPages ? true : false}
                >
                    &#62;
                </button>

                <button
                    onClick={() => { handleLastPage() }}
                    disabled={users.currentPage >= users.totalPages ? true : false}
                >
                    &#62;&#62;
                </button>

            </div>
        </React.Fragment>
    );
}

export default UsersList;