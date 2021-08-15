import React from 'react';
import { Route, Switch, useLocation } from 'react-router-dom';

import Nav from './_components/Nav';
import Login from './pages/login/Login';
import { Users } from './pages/users/Index';

import './styles/main.scss';

function App() {
    const { pathname } = useLocation();

    return (
        <main className="main">
            <header className="main__header">
                <nav className="main__nav">
                    <Nav />
                </nav>
            </header>

            <section className="main__section">
                <Switch>
                    <Route exact path="/Login" component={Login} />
                    <Route path="/test" component={Users} />
                </Switch>
            </section>

            <footer className="main__footer">
                <p>Author: XXxxxx YYyyyy</p>
                <p><a href="mailto:hege@example.com">hege@example.com</a></p>
            </footer>
        </main>
    );
}

export default App;