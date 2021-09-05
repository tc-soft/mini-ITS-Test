import React, { useState } from 'react';
import { Route, Switch } from 'react-router-dom';

import UsersList from './UsersList';
import UsersForm from './UsersForm';

function Users({ match }) {
    const { path } = match;
    const [pagedQuery, setPagedQuery] = useState({
        filter: null,
        sortColumnName: "Login",
        sortDirection: "ASC",
        page: 1,
        resultsPerPage: 5
    });
    const [activeDepartmentFilter, setActiveDepartmentFilter] = useState(null);
    const [activeRoleFilter, setActiveRoleFilter] = useState(null);

    return (
        <Switch>
            <Route
                exact path={path}
                render={(props) => <UsersList {...props}
                    pagedQuery={pagedQuery}
                    setPagedQuery={setPagedQuery}
                    activeDepartmentFilter={activeDepartmentFilter}
                    setActiveDepartmentFilter={setActiveDepartmentFilter}
                    activeRoleFilter={activeRoleFilter}
                    setActiveRoleFilter={ setActiveRoleFilter }
                />}
            />
            <Route path={`${path}/Create`} component={UsersForm} />
            <Route path={`${path}/Edit/:id`} component={UsersForm} />
        </Switch>
    );
}

export default Users;