import React, { useState } from 'react';
import { Link } from "react-router-dom";
import { useAuth } from '../../components/AuthProvider';
import { useForm } from "react-hook-form";
import ErrorMessage from './ErrorMessage';
import { usersServices } from '../../services/UsersServices';

import '../../styles/pages/Login.scss';

function LoginForm({ history }) {
    const { handleLogin } = useAuth();
    const [loginError, setLoginError] = useState("");
    const { handleSubmit, register, reset, formState: { errors } } = useForm();

    function onSubmit(values) {
        usersServices.login(values.login, values.password)
            .then((response) => {
                if (response.ok) {
                    return response.json()
                        .then((data) => {
                            handleLogin(data);
                            setLoginError(null);
                            reset();
                            history.push('/');
                        })
                } else {
                    return response.json()
                        .then((data) => {
                            setLoginError(data);
                            reset();
                        })
                }
            })
            .catch((error) => {
                setTimeout(() => {
                    console.error('Error:', error);
                    alert(error);
                }, 200);
            });
    };

    return (
        <>
            <h3>Logowanie</h3>
            <br />

            <form onSubmit={handleSubmit(onSubmit)}>

                {loginError
                    ? <p style={{ color: 'red' }}>{loginError}</p>
                    : <p style={{ color: 'red' }}>&nbsp;</p>
                }

                <label>Nazwa użytkownika</label><br />
                <input
                    type='text'
                    placeholder='Wpisz login'
                    error={errors.login}
                    {...register('login', {
                        required: 'Nazwa użytkownika jest wymagana',
                        maxLength: { value: 20, message: 'Nazwa użytkownika za długa' }
                    })}
                />
                {errors.login
                    ? <p style={{ color: 'red' }}>{errors.login?.message}</p>
                    : <p style={{ color: 'red' }}>&nbsp;</p>
                }

                <label>Hasło</label><br />
                <input
                    type='password'
                    placeholder='Wpiz hasło'
                    autoComplete='on'
                    error={errors.password}
                    {...register('password', {
                        required: 'Hasło jest wymagane',
                        maxLength: { value: 40, message: 'Hasło za długie' }
                    })}
                />
                {errors.password
                    ? <p style={{ color: 'red' }}>{errors.password?.message}</p>
                    : <p style={{ color: 'red' }}>&nbsp;</p>
                }

                <label className='contact__rodo'>
                    <input
                        type='checkbox'
                        name='toggle'
                    />
                    &nbsp;Zapamiętaj mnie&nbsp;
                </label>

                <div className='contact__buttons'>
                    <button
                        type='submit'
                        className=''
                        disabled={false}
                    >
                        Login
                    </button>
                </div>

                <br />
                <Link to='/restore'>Zapomniałem hasła</Link>

                <div className=''>
                    <button
                        type='submit'
                        className=''
                        disabled={false}
                    >
                        Zapisz
                    </button>

                    <Link to='.' className=''>
                        Anuluj 2
                    </Link>

                </div>

            </form>
        </>
    );
}

export default LoginForm;