import React from 'react';
import { Formik, Form, Field } from 'formik';
import * as Yup from 'yup';
import { usersServices } from '../../services/UsersServices';
import ErrorMessage from '../login/ErrorMessage';

//import '../../styles/pages/Login.scss';

function UsersForm({ history, match }) {
    const { id } = match.params;
    const isAddMode = !id;

    return (
        <React.Fragment>
            <Formik
                initialValues={{
                    login: '',
                    firstName: '',
                    lastName: '',
                    department: '',
                    email: '',
                    phone: '',
                    role: '',
                    password: '',
                    confirmPassword: ''
                }}

                validationSchema={Yup.object({
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
                    password: Yup.string()
                        .transform(x => x === '' ? undefined : x)
                        .concat(isAddMode ? Yup.string().required('Hasło jest wymagane') : null)
                        .min(6, 'Hasło musi mieć minimum 6 znaków'),
                    confirmPassword: Yup.string()
                        .transform(x => x === '' ? undefined : x)
                        .when('password', (password, schema) => {
                            if (password || isAddMode) return schema.required('Potwierdzenie hasła jest wymagane');
                        })
                        .oneOf([Yup.ref('password')], 'Hasła nie są takie same')
                })}

                onSubmit={(values, { setSubmitting, resetForm }) => {
                    usersServices.create(values)
                    .then((response) => {
                        if (response.ok) {
                            return response.json()
                                .then((data) => {
                                    alert(values);
                                    resetForm();
                                })
                        } else {
                            return response.json()
                                .then((data) => {
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
                    <Form className="xxx">
                        <h1 className="xxx__title">{isAddMode ? 'Add User' : 'Edit User'}</h1>
                        <br />

                        <label htmlFor="login">Login</label><br />
                        <Field
                            name="login"
                            type="text"
                            placeholder="Wpisz login"
                            className={errors.login && (touched.login || values.login) && "contact__ValidationError"}
                        />
                        <ErrorMessage errors={errors.login} touched={touched.login} values={values.login} />

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
                            <option value="Produkcja">Produkcja</option>
                            <option value="Dział Techniczny">Dział Techniczny</option>
                            <option value="IT">IT</option>
                        </Field>
                        <ErrorMessage errors={errors.department} touched={touched.department} values={values.department} />

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
                            <option value="Użytkownik">Użytkownik</option>
                            <option value="Kierownik">Kierownik</option>
                            <option value="Administrator">Administrator</option>
                        </Field>
                        <ErrorMessage errors={errors.role} touched={touched.role} values={values.role} />

                        <label htmlFor="password">Hasło</label><br />
                        <Field
                            name="password"
                            type="password"
                            placeholder="Wpisz hasło"
                            className={errors.password && (touched.password || values.password) && "contact__ValidationError"}
                        />
                        <ErrorMessage errors={errors.password} touched={touched.password} values={values.password} />

                        <label htmlFor="confirmPassword">Hasło</label><br />
                        <Field
                            name="confirmPassword"
                            type="password"
                            placeholder="Wpisz hasło"
                            className={errors.confirmPassword && (touched.confirmPassword || values.confirmPassword) && "contact__ValidationError"}
                        />
                        <ErrorMessage errors={errors.confirmPassword} touched={touched.confirmPassword} values={values.confirmPassword} />


                        <div className="contact__buttons">
                            <button type="submit" className="buttonSend" disabled={!(isValid && dirty)}>Zapisz</button>
                        </div>
                    </Form>
                )}
            </Formik>
        </React.Fragment>
    );
}

export default UsersForm;