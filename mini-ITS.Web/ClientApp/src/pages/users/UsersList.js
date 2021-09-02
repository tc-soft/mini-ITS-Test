import React, { useState, setState, useEffect } from 'react';
import { Link } from 'react-router-dom';

import { usersServices } from '../../services/UsersServices';

function UsersList({ match }) {
    const { path } = match;
    const [pagedQuery, setPagedQuery] = useState({
        filter: [{
            name: "Department",
            operator: "=",
            value: "Sales"
        }],
        sortColumnName: "Login",
        sortDirection: "ASC",
        page: 1,
        resultsPerPage: 5
    });
    const [users, setUsers] = useState({
        results: '',
        currentPage: '',
        resultsPerPage: '',
        totalResults: '',
        totalPages: ''
    });
    const [activeDepartmentFilter, setActiveDepartmentFilter] = useState("")

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
    }, [pagedQuery]);

    function deleteUser(id) {
        var answer = window.confirm("Kasować użytkownika ?");
        if (answer) {
            setUsers({
                results: users.results.map(x => {
                    if (x.id === id) { x.isDeleting = true; }
                    return x
                }),
                currentPage: users.currentPage,
                resultsPerPage: users.resultsPerPage,
                totalResults: users.totalResults,
                totalPages: users.totalPages
            });

            usersServices.delete(id)
                .then(() => {
                    setUsers({
                        results: users.results.filter(x => x.id !== id),
                        currentPage: users.currentPage,
                        resultsPerPage: users.resultsPerPage,
                        totalResults: users.totalResults,
                        totalPages: users.totalPages
                    });
                });
        }
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
                disabled={activeDepartmentFilter == "IT" ? true : false}
            >
                Filter: Department-IT;
            </button>
            <button
                onClick={() => { handleDepartmentFilter("Sales") }}
                disabled={activeDepartmentFilter == "Sales" ? true : false}
            >
                Filter: Department-Sales;
            </button>
            <h1>Users</h1>
            <Link to={`${path}/Create`}>Dodaj</Link>
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
                    {users.results && users.results.map((user, index) =>
                        <tr key={user.login}>
                            <td>{String("0" + index).slice(-2)}</td>
                            <td>{user.login}</td>
                            <td>{user.firstName}</td>
                            <td>{user.lastName}</td>
                            <td>{user.department}</td>
                            <td>{user.role}</td>
                            <td>
                                <button>
                                    <Link to={`${path}/edit/${user.id}`}>Edit</Link>
                                </button>
                                
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

export { UsersList };