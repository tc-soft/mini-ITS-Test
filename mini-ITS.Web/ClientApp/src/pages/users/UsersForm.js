import React, { useState, useEffect } from 'react';
import { Link, NavLink } from 'react-router-dom';
import { useForm } from "react-hook-form";
import { usersServices } from '../../services/UsersServices';
import ErrorMessage from '../login/ErrorMessage';

//import '../../styles/pages/Login.scss';

function UsersForm({ history, match }) {
    const { id } = match.params;
    const isAddMode = !id;
    const isEditMode = !isAddMode;
    const [showPassword, setShowPassword] = useState(false);
    const [activePassword, setActivePassword] = useState(false);
    const [user, setUser] = useState(null);
    const { handleSubmit, register, reset, getValues, formState: { errors } } = useForm();

    const mapRole = [{
        "id": 1,
        "name": "Użytkownik",
        "value": "User"
    }, {
        "id": 2,
        "name": "Kierownik",
        "value": "Manager"
    }, {
        "id": 3,
        "name": "Administrator",
        "value": "Administrator"
    }];

    const mapDepartment = [{
        "id": 1,
        "name": "IT",
        "value": "IT"
    }, {
        "id": 2,
        "name": "Development",
        "value": "Development"
    }, {
        "id": 3,
        "name": "Managers",
        "value": "Managers"
    }, {
        "id": 4,
        "name": "Research",
        "value": "Research"
    }, {
        "id": 5,
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

    
    
    const onSubmit = (values) => {
        console.table(values);
    };

    useEffect(() => {
        if (isEditMode) {
            usersServices.edit(id)
                .then((response) => {
                    if (response.ok) {
                        return response.json()
                            .then((data) => {
                                setUser(data)
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
                department: null,
                email: null,
                phone: null,
                role: null,
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
                    {...register("firstName", { required: true, maxLength: 80 })}
                />
                {errors.firstName && "imie jest wymagane"}
                <p>{errors.firstName?.message}</p><br />


                <label>Nazwisko</label><br />
                <input
                    type="text"
                    placeholder="Wpisz nazwisko"
                    {...register("lastName", { required: true, maxLength: 80 })}
                /><br />

                <label>Dział</label><br />
                <select
                    {...register("department", { required: true, maxLength: 80 })}
                    name="department"
                    placeholder="Wybierz dział"
                    selectValue="-----"
                >
                    {mapDepartment.map(
                        (x) => <option value={x.value}>{x.name}</option>)
                    }
                </select>
                <br /><br />

                <label>Email</label><br />
                <input
                    type="text"
                    placeholder="Wpisz email"
                    {...register("email", {
                        required: true,
                        pattern: /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
                    })}
                /><br />

                <label>Telefon</label><br />
                <input
                    type="tel"
                    placeholder="Wpisz telefon"
                    {...register("phone", {
                        required: true,
                        maxLength: 11,
                        minLength: 8
                    })}
                /><br />


                <label>Rola</label><br />
                <select
                    {...register("role", { required: true, maxLength: 80 })}
                    name="role"
                    placeholder="Wybierz rolę"
                    selectValue="-----"
                >
                    {mapRole.map(
                        (x) => <option value={x.value}>{x.name}</option>)
                    }
                </select>
                <br /><br />

                {isEditMode &&
                    <div>
                        <label>
                        <input
                            type="checkbox"
                            defaultChecked={activePassword}
                            onChange={() => setActivePassword(!activePassword)}
                        />
                                Zmiana hasła
                            </label>
                    </div>
                }
                <br />




                <button type="button" onClick={() => setShowPassword(!showPassword)}>
                    {showPassword ? "Hide" : "Show"}
                </button><br />

                {/*============================================================================================*/}
                <label>Hasło:</label><br />
                <input
                    name="passwordHash"
                    type={showPassword ? "text" : "password"}
                    placeholder="Wpisz hasło"
                    disabled={!activePassword}
                    autoComplete="on"
                    {...register("passwordHash",
                        if({activePassword}) {
                            required: "Hasło jest wymagane",
                            minLength: {value: 6, message: "Hasło musi zawierać min. 8 znaków"
                        }

                        activePassword ? {
                            required: "Hasło jest wymagane",
                            minLength: { value: 6, message: "Hasło musi zawierać min. 8 znaków" }
                        } : required: false
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
                    {...register("confirmPasswordHash", {
                        required: activePassword ? "Potwierdź hasło !" : false,
                        validate: activePassword ?
                        {
                            matchesPreviousPassword: (value) => {
                                const { passwordHash } = getValues();
                                return passwordHash === value || "Hasła powinny się zgadzać !";
                            }
                        } : false
                    })}
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