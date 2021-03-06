import React from 'react';
import { Route, Switch } from 'react-router-dom';

import Nav from './components/Nav';
import AuthProvider from './components/AuthProvider';
import Login from './pages/login/Login';
import Users from './pages/users/Users';

import './styles/main.scss';

function App() {

    return (
        <AuthProvider>
            <main className="main">
                <header className="main__header">
                    <nav className="main__nav">
                        <Nav />
                    </nav>
                </header>

                <section className="main__section">
                    <Switch>
                        <Route path="/Users" component={Users} />
                        <Route path="/Login" component={Login} />
                    </Switch>
                </section>

                <footer className="main__footer">
                    <p>XXxxxx YYyyyy</p>
                    <p><a href="mailto:hege@example.com">example@example.com</a></p>
                </footer>
            </main>
        </AuthProvider>
    );
}

export default App;