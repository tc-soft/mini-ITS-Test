import React from 'react';
import { Route, Switch, Redirect, useLocation } from 'react-router-dom';

import Nav from './_components/Nav';
import Home from './home/Index';
import LoginForm from './home/LoginForm';
import { Users } from './users/Index';

import './styles/main.scss';

function App() {
    const { pathname } = useLocation();

    return (
        <main className="main">
            <header className="header">
                <nav className="nav">
                    <Nav />
                </nav>
            </header>

            <section className="section">
                Tola ma kota
                <Switch>
                    <Redirect from="/:url*(/+)" to={pathname.slice(0, -1)} />
                    <Route exact path="/" component={Home} />
                    <Route exact path="/Login" component={LoginForm} />
                    <Route path="/test" component={Users} />
                    <Redirect from="*" to="/" />
                </Switch>
            </section>

            <footer>
                <p>Author: XXxxxx YYyyyy</p>
                <p><a href="mailto:hege@example.com">hege@example.com</a></p>
            </footer>
        </main>
    );
}

export default App;