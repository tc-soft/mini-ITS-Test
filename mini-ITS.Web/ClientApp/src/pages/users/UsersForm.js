import React, { useState, useEffect } from 'react';
import { Link, NavLink } from 'react-router-dom';
import { useForm } from "react-hook-form";
import { usersServices } from '../../services/UsersServices';

//import '../../styles/pages/Login.scss';

function UsersForm({ history, match }) {
    const { id } = match.params;
    const isAddMode = !id;
    const isEditMode = !isAddMode;
    const [showPassword, setShowPassword] = useState(false);
    const [activePassword, setActivePassword] = useState(false);
    const [user, setUser] = useState(null);
    const { handleSubmit, register, reset, getValues, setValue, clearErrors, formState: { errors } } = useForm();

    const mapRole = [{
        "name": "Użytkownik",
        "value": "User"
    }, {
        "name": "Kierownik",
        "value": "Manager"
    }, {
        "name": "Administrator",
        "value": "Administrator"
    }];

    const mapDepartment = [{
        "name": "IT",
        "value": "IT"
    }, {
        "name": "Development",
        "value": "Development"
    }, {
        "name": "Managers",
        "value": "Managers"
    }, {
        "name": "Research",
        "value": "Research"
    }, {
        "name": "Sales",
        "value": "Sales"
        }
    ];
    
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
                    if (activePassword) {
                        const data = {
                            login: values.login,
                            oldPassword: values.oldPassword,
                            newPassword: values.passwordHash
                        };
                        usersServices.changePassword(id, data)
                            .then((response) => {
                                if (response.ok) {
                                    history.push("/Users");
                                }
                            }
                            );
                    }
                    else {
                        history.push("/Users");
                    }
                    
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

    function onSubmit(values) {
        console.table(values);
        isAddMode
            ? createUser(values)
            : updateUser(id, values)
    };

    useEffect(() => {
        if (isEditMode) {
            usersServices.edit(id)
                .then((response) => {
                    if (response.ok) {
                        return response.json()
                            .then((data) => {
                                data.passwordHash = '';
                                data.confirmPasswordHash = '';
                                setUser(data);
                                //dfsgcsdgctg
                            });
                    } else {
                        return response.json()
                            .then((data) => {
                                console.log(data);
                            })
                    }
                })
            setActivePassword(false);
        }
        else {
            setUser({
                login: null,
                firstName: null,
                lastName: null,
                department: "IT",
                email: null,
                phone: null,
                role: "Manager",
                passwordHash: '',
                confirmPasswordHash: ''
            });
            setActivePassword(true);
        }
    }, [])

    useEffect(() => {
        reset(user)
    }, [user]);

    return (
        <React.Fragment>
            <h1 className="xxx__title">{isAddMode ? 'Dodaj' : 'Edytuj'}</h1>
            <br />
            <form onSubmit={handleSubmit(onSubmit)}>
                <label>Login</label><br/>
                <input
                    type="text"
                    placeholder="Wpisz login"
                    error={errors.login}
                    {...register("login", { required: true, maxLength: 80 })}
                />
                {errors.login && "Login jest wymagany"}
                <br />

                <label>Imię</label><br />
                <input
                    type="text"
                    placeholder="Wpisz imię"
                    error={errors.firstName}
                    {...register("firstName", { required: true, maxLength: 80 })}
                />
                {errors.firstName && "imie jest wymagane"}
                <p>{errors.firstName?.message}</p><br />


                <label>Nazwisko</label><br />
                <input
                    type="text"
                    placeholder="Wpisz nazwisko"
                    error={errors.lastName}
                    {...register("lastName", { required: true, maxLength: 80 })}
                /><br />

                <label>Dział</label><br />
                <select
                    placeholder="Wybierz dział"
                    error={errors.department}
                    {...register("department", { required: true, maxLength: 80 })}
                >
                    {mapDepartment.map(
                        (x, y) => <option key={y} value={x.value}>{x.name}</option>)
                    }
                </select>
                <br /><br />

                <label>Email</label><br />
                <input
                    type="text"
                    placeholder="Wpisz email"
                    error={errors.email}
                    {...register("email", {
                        required: true,
                        pattern: /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
                    })}
                /><br />

                <label>Telefon</label><br />
                <input
                    type="tel"
                    placeholder="Wpisz telefon"
                    error={errors.phone}
                    {...register("phone", {
                        required: true,
                        maxLength: 11,
                        minLength: 8
                    })}
                /><br />


                <label>Rola</label><br />
                <select
                    placeholder="Wybierz rolę"
                    error={errors.role}
                    {...register("role", { required: true, maxLength: 80 })}
                >
                    {mapRole.map(
                        (x, y) => <option key={y} value={x.value}>{x.name}</option>)
                    }
                </select>
                <br /><br />

                {isEditMode &&
                    <div>
                        <label>
                        <input
                            type="checkbox"
                            checked={activePassword}
                            onChange={() => {
                                setActivePassword(!activePassword);
                                setShowPassword(false);
                                setValue("passwordHash", "");
                                clearErrors("passwordHash");
                                setValue("confirmPasswordHash", "");
                                clearErrors("confirmPasswordHash");
                                }
                            }
                        />
                                Zmiana hasła
                            </label>
                    </div>
                }
                <br />

                <button type="button"
                    disabled={!activePassword}
                    onClick={() => activePassword ? setShowPassword(!showPassword) : null}
                >
                    { showPassword ? "Hide" : "Show" }
                </button><br />

                {/*============================================================================================*/}
                <label>Hasło:</label><br />
                <input
                    type={showPassword ? "text" : "password"}
                    placeholder="Wpisz stare hasło"
                    disabled={!activePassword}
                    autoComplete="on"
                    error={errors.oldPassword}
                    {...register("oldPassword", activePassword ?
                        {
                            required: "Hasło jest wymagane",
                            minLength: { value: 6, message: "Hasło musi zawierać min. 6 znaków" }
                        }
                        :
                        {
                            required: false
                        }
                    )
                    }
                />
                {errors.oldPassword && (
                    <p style={{ color: "red" }}>
                        {errors.oldPassword.message}
                    </p>
                )}
                <br />


                <input
                    type={showPassword ? "text" : "password"}
                    placeholder="Wpisz hasło"
                    disabled={!activePassword}
                    autoComplete="on"
                    error={errors.passwordHash}
                    {...register("passwordHash", activePassword ?
                            {
                                required: "Hasło jest wymagane",
                                minLength: { value: 6, message: "Hasło musi zawierać min. 6 znaków" }
                            }
                            :
                            {
                                required: false
                            }
                        )
                    }
                />
                {errors.passwordHash && (
                    <p style={{ color: "red" }}>
                        {errors.passwordHash.message}
                    </p>
                )}
                <br />

                <label>Powtórz hasło:</label><br />
                <input
                    name="confirmPasswordHash"
                    type={showPassword ? "text" : "password"}
                    placeholder="Wpisz hasło"
                    disabled={!activePassword}
                    autoComplete="off"
                    error={errors.confirmPasswordHash}
                    {...register("confirmPasswordHash", activePassword ?
                            {
                                required: "Potwierdź hasło !",
                                validate: value => value === getValues("passwordHash") || "Hasła powinny się zgadzać !"
                            }
                            :
                            {
                            required: false
                            }
                        )
                    }
                />
                {errors.confirmPasswordHash && (
                    <p style={{ color: "red" }}>
                        {errors.confirmPasswordHash.message}
                    </p>
                )}
                <br />
                {/*============================================================================================*/}

                <br />

                <div className="">
                    <button type="submit" className="" disabled={false}>Zapisz</button>

                    <NavLink to={isAddMode ? '.' : '..'} className="">
                        <button type="button">
                            Anuluj
                            </button>
                    </NavLink>

                    <Link to={isAddMode ? '.' : '..'} className="">
                        Anuluj 2
                        </Link>
                </div>

            </form>
        </React.Fragment>
    );
}

export default UsersForm;