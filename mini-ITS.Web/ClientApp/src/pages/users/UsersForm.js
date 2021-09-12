import React, { useState, useEffect } from 'react';
import { Link, NavLink } from 'react-router-dom';
import { useForm } from "react-hook-form";
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
    const [user, setUser] = useState(null);
    
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

    const { handleSubmit, register, required, errors } = useForm({
        //defaultValues: initialValues()
    });

    const onSubmit = (values) => {
        console.log(values);
    };

    //useEffect(() => {
    //    setSchema(FormSchema(activePassword));
    //}, [activePassword]);

    return (
        <React.Fragment>
            <form onSubmit={handleSubmit(onSubmit)}>
                <label>First name</label>
                <input
                    type="text"
                    {...register("First name", { required: true, maxLength: 80 })}
                />
                <label>Last name</label>
                <input
                    type="text"
                    {...register("Last name", { required: true, maxLength: 100 })}
                />
                <label>Email</label>
                <input
                    type="text"
                    {...register("Email", {
                        required: true,
                        pattern: /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
                    })}
                />
                <label>Mobile number</label>
                <input
                    type="tel"
                    {...register("Mobile number", {
                        required: true,
                        maxLength: 11,
                        minLength: 8
                    })}
                />
                <label>Title</label>

                <label>Are you a developer?</label>
                <input
                    type="radio"
                    value="Yes"
                    {...register("developer", { required: true })}
                />
                <input
                    type="radio"
                    value="No"
                    {...register("developer", { required: true })}
                />

                <input type="submit" />
            </form>
        </React.Fragment>
    );
}

export default UsersForm;