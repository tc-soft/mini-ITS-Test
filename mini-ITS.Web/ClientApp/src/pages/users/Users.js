import React from 'react';
import { Route, Switch } from 'react-router-dom';

import { UsersList } from './UsersList';
import UsersForm from './UsersForm';

function Users({ match }) {
    const { path } = match;
    
    return (
        <Switch>
            <Route exact path={path} component={UsersList} />
            <Route path={`${path}/Create`} component={UsersForm} />
            {/*<Route path={`${path}/edit/:id`} component={AddEdit} />*/}
        </Switch>
    );
}

export { Users };