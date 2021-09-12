import React, { useState, useEffect } from 'react';
import { Link, NavLink } from 'react-router-dom';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import { usersServices } from '../../services/UsersServices';
import ErrorMessage from '../login/ErrorMessage';

//import '../../styles/pages/Login.scss';

function UsersForm({ history, match }) {
    const { id } = match.params;
    const isAddMode = !id;
    const isEditMode = !isAddMode;
    const [showPassword, setShowPassword] = useState(false);
    const [activePassword, setActivePassword] = useState(false);
    const [schema, setSchema] = useState(null);
    const [user, setUser] = useState({
        login: '',
        firstName: null,
        lastName: null,
        department: null,
        email: null,
        phone: null,
        role: null,
        passwordHash: '',
        confirmPasswordHash: ''    });
    
    function createUser(values) {
        usersServices.create(values)
            .then((response) => {
                if (response.ok) {
                    history.push("/Users");
                } else {
                    return response.text()
                        .then((data) => {
                            alert(data);
                        })
                }
            })
            .catch(error => {
                console.error('Error: ', error);
            });
    }

    function updateUser(id, values) {
        usersServices.update(id, values)
            .then((response) => {
                if (response.ok) {
                    history.push("/Users");
                } else {
                    return response.text()
                        .then((data) => {
                            alert(data);
                        })
                }
            })
            .catch(error => {
                console.error('Error: ', error);
            });
    }

    function initialValues() {
        if (isEditMode) {
            setTimeout(() => {
                usersServices.edit(id)
                    .then((response) => {
                        if (response.ok) {
                            return response.json()
                                .then((data) => {
                                    setUser(data);
                                });
                        } else {
                            return response.json()
                                .then((data) => {
                                    console.log(data);
                                })
                        }
                    });
                setActivePassword(false);
            }, 0);
        }
        else {
            setUser({
                login: null,
                firstName: null,
                lastName: null,
                department: null,
                email: null,
                phone: null,
                role: null,
                passwordHash: '',
                confirmPasswordHash: ''
            });
            setActivePassword(true);
        }
        return user;
    }

    const FormSchema = (activePassword) => Yup.object().shape({
        login: Yup.string()
            .required('Login użytkownika jest wymagana'),
        firstName: Yup.string()
            .required('Imię użytkownika jest wymagane'),
        lastName: Yup.string()
            .required('Nazwisko użytkownika jest wymagane'),
        department: Yup.string()
            .required('Dział użytkownika jest wymagany'),
        email: Yup.string()
            .email('Email jest niewłaściwy')
            .required('Email użytkownika jest wymagany'),
        phone: Yup.string()
            .required('Telefon użytkownika jest wymagany'),
        role: Yup.string()
            .required('Rola użytkownika jest wymagana'),
        passwordHash:
            activePassword ? Yup.string().required('Hasło jest wymagane') : Yup.string(),
        confirmPasswordHash: Yup.string()
            .when('passwordHash', (password, schema) => {
                if (password && activePassword) return schema.required('Potwierdzenie hasła jest wymagane');
            })
            .oneOf([Yup.ref('passwordHash')], 'Hasła nie są takie same')
    });

    useEffect(() => {
        setSchema(FormSchema(activePassword));
    }, [activePassword]);

    return (
        <React.Fragment>
            <Formik
                //enableReinitialize={true}
                initialValues={initialValues()}
                validationSchema={schema}

                onSubmit={(values, { setSubmitting }) => {
                    setSubmitting(false);
                        isAddMode
                            ? createUser(values)
                            : updateUser(id, values);
                    }}
            >
            {({ values, touched, errors, dirty, isValid, isSubmitting, setFieldValue, setErrors }) => (
                <Form className="xxx">
                    <h1 className="xxx__title">{isAddMode ? 'Dodaj' : 'Edytuj'}</h1>
                    <br />

                    <label htmlFor="login">Login</label><br />
                    <Field
                        name="login"
                        type="text"
                        placeholder="Wpisz login"
                        className={errors.login && (touched.login || values.login) && "contact__ValidationError"}
                    />
                    <ErrorMessage errors={errors.login} touched={touched.login} values={values.login} />

                    <div>
                        <label htmlFor="firstName">Imię</label><br />
                        <Field
                            name="firstName"
                            type="text"
                            placeholder="Wpisz imię"
                            className={errors.firstName && (touched.firstName || values.firstName) && "contact__ValidationError"}
                        />
                        <ErrorMessage errors={errors.firstName} touched={touched.firstName} values={values.firstName} />

                        <label htmlFor="lastName">Nazwisko</label><br />
                        <Field
                            name="lastName"
                            type="text"
                            placeholder="Wpisz nazwisko"
                            className={errors.lastName && (touched.lastName || values.lastName) && "contact__ValidationError"}
                        />
                        <ErrorMessage errors={errors.lastName} touched={touched.lastName} values={values.lastName} />

                        <label htmlFor="department">Dział</label><br />
                        <Field name="department" as="select"
                            className={errors.department && (touched.department || values.department) && "contact__ValidationError"}
                        >
                            <option value=""></option>
                            <option value="Managers">Managersi</option>
                            <option value="Produkcja">Produkcja</option>
                            <option value="Dział Techniczny">Dział Techniczny</option>
                            <option value="IT">IT</option>
                        </Field>
                        <ErrorMessage errors={errors.department} touched={touched.department} values={values.department} />
                    </div>

                    <div>
                        <label htmlFor="email">Email</label><br />
                        <Field
                            name="email"
                            type="text"
                            placeholder="Wpisz email"
                            className={errors.email && (touched.email || values.email) && "contact__ValidationError"}
                        />
                        <ErrorMessage errors={errors.email} touched={touched.email} values={values.email} />


                        <label htmlFor="phone">Telefon</label><br />
                        <Field
                            name="phone"
                            type="text"
                            placeholder="Wpisz telefon"
                            className={errors.phone && (touched.phone || values.phone) && "contact__ValidationError"}
                        />
                        <ErrorMessage errors={errors.phone} touched={touched.phone} values={values.phone} />

                        <label htmlFor="role">Rola</label><br />
                        <Field name="role" as="select"
                            className={errors.role && (touched.role || values.role) && "contact__ValidationError"}
                        >
                            <option value=""></option>
                            <option value="User">Użytkownik</option>
                            <option value="Manager">Kierownik</option>
                            <option value="Administrator">Administrator</option>
                        </Field>
                        <ErrorMessage errors={errors.role} touched={touched.role} values={values.role} />
                    </div>

                    <br />

                    {isEditMode &&
                        <div>
                            <label>
                                <input type="checkbox"
                                    defaultChecked={activePassword}
                                    onChange={() => setActivePassword(!activePassword)}
                                />
                                Zmiana hasła
                            </label>

                            <Field
                                type="checkbox"
                                name="activePassword"
                                checked={activePassword}
                                    onChange={() => {
                                        setActivePassword(!activePassword);
                                        setFieldValue('firstName', '');
                                    }
                                }
                            />
                        </div>
                    }

                    <br />

                    <button type="button" onClick={() => setShowPassword(!showPassword)}>
                        {showPassword ? "Hide" : "Show"}
                    </button>

                    <br />

                    <label htmlFor="passwordHash">Hasło</label><br />
                    <Field
                        name="passwordHash"
                        type={showPassword ? "text" : "password"}
                        placeholder="Wpisz hasło"
                        disabled={!activePassword}
                        autoComplete="on"
                        className={errors.passwordHash && (touched.passwordHash || values.passwordHash) && "contact__ValidationError"}
                    />
                    <ErrorMessage errors={errors.passwordHash} touched={touched.passwordHash} values={values.passwordHash} />

                    <label htmlFor="confirmPasswordHash">Hasło</label><br />
                    <Field
                        name="confirmPasswordHash"
                        type={showPassword ? "text" : "password"}
                        placeholder="Wpisz hasło"
                        disabled={!activePassword}
                        autoComplete="on"
                        className={errors.confirmPasswordHash && (touched.confirmPasswordHash || values.confirmPasswordHash) && "contact__ValidationError"}
                    />
                    <ErrorMessage errors={errors.confirmPasswordHash} touched={touched.confirmPasswordHash} values={values.confirmPasswordHash} />

                    <div className="">
                        <button type="submit" className="" disabled={!(isValid)}>Zapisz</button>

                        <NavLink to={isAddMode ? '.' : '..'} className="">
                            <button type="button">
                                Anuluj
                            </button>
                        </NavLink>

                        <Link to={isAddMode ? '.' : '..'} className="">
                            Anuluj 2
                        </Link>

                    </div>
                </Form>
            )}
            </Formik>
        </React.Fragment>
    );
}

export default UsersForm;