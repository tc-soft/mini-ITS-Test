import React from 'react';
import { Link } from "react-router-dom";
import { useAuth } from '../../components/AuthProvider';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import ErrorMessage from './ErrorMessage';

import '../../styles/pages/Login.scss';

function LoginForm() {
    const { currentUser, handleLogin, handleLogout } = useAuth();
    return (
        <React.Fragment>
            <Formik
                initialValues={{
                    login: '',
                    password: ''
                }}

                validationSchema={Yup.object({
                    login: Yup
                        .string()
                        .required('Pole wymagane'),
                    password: Yup
                        .string()
                        .required('Pole wymagane')
                })}

                onSubmit={(values, { setSubmitting, resetForm }) => {
                    //https://localhost:44375/Users/Login
                    const url = new URL(window.location.href);
                    const port = (url.port ? `:${url.port}` : "");
                    //const apiAddress = `${url.protocol}//${url.hostname}${port}/api${url.pathname}`;
                    const apiAddress = "https://localhost:44375/Users/Login";
                    

                    fetch(apiAddress, {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json',
                            'Accept': 'application/json'
                        },
                        body: JSON.stringify({
                            login: values.login,
                            password: values.password
                        })
                    })
                    .then((response) => response.json())
                    .then((data) => {
                        console.log("messages", data);
                        setSubmitting(false);
                        resetForm();
                        alert("Wiadomość wysłana.\n\nDziękuję.");
                        handleLogin(values.login);
                    })
                    .catch((error) => {
                        setTimeout(() => {
                            console.error('Error:', error);
                            setSubmitting(false);
                            alert("Wiadomość nie została wysłana.\n\nBłąd serwera.");
                        }, 200);
                    });
                }}
            >
                {({ values, touched, errors, dirty, isValid, isSubmitting }) => (
                    <Form
                        className="login">
                        <h2 className="login__title">Zaloguj się</h2>
                        <br/>

                        <label htmlFor="login">Nazwa użytkownika</label><br />
                        <Field
                            name="login"
                            type="text"
                            placeholder="Wpisz login"
                            className={errors.name && (touched.name || values.name) && "contact__ValidationError"}
                        />
                        <ErrorMessage errors={errors.name} touched={touched.name} values={values.name} />

                        <label htmlFor="password">Hasło</label><br/>
                        <Field
                            name="password"
                            type="password"
                            placeholder="Wpiz hasło"
                            className={errors.email && (touched.email || values.email) && "contact__ValidationError"}
                        />
                        <ErrorMessage errors={errors.email} touched={touched.email} values={values.email} />

                    <label className="contact__rodo">
                        <Field 
                            type="checkbox"
                            name="toggle"
                        />
                        &nbsp;Zapamiętaj mnie&nbsp;
                    </label>

                    <div className="contact__buttons">
                        <button type="submit" className="buttonSend" disabled={!(isValid && dirty)}>Login</button>
                    </div>

                    <br />
                    <Link to="/restore">Zapomniałem hasła</Link>
                </Form>
            )}
            </Formik>
        </React.Fragment>
    );
}

export default LoginForm;