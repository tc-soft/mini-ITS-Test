import React, { useState } from 'react';
import { Link } from "react-router-dom";
import { useAuth } from '../../components/AuthProvider';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import ErrorMessage from './ErrorMessage';
import { usersServices } from '../../services/UsersServices';

import '../../styles/pages/Login.scss';

function LoginForm() {
    const { handleLogin } = useAuth();
    const [loginError, setLoginError] = useState("");
    return (
        <React.Fragment>
            {}
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
                    usersServices.login(values.login, values.password)
                    .then((response) => {
                        if (response.ok) {
                            return response.json()
                                .then((data) => {
                                    setSubmitting(false);
                                    resetForm();
                                    handleLogin(data);
                                    setLoginError(null);
                                })
                        } else {
                            return response.json()
                                .then((data) => {
                                    setLoginError(data);
                                    setSubmitting(false);
                                    resetForm();
                                })
                        }
                    })
                    .catch((error) => {
                        setTimeout(() => {
                            console.error('Error:', error);
                            alert(error);
                            setSubmitting(false);
                        }, 200);
                    });
                }}
            >
                {({ values, touched, errors, dirty, isValid, isSubmitting }) => (
                    <Form
                        className="login">
                        <h2 className="login__title">Zaloguj się</h2>
                        <br/>

                        {loginError && (
                            <div style={{ color: "red" }}>
                                <span>{loginError}</span>
                            </div>
                        )}

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
                            autoComplete="on"
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