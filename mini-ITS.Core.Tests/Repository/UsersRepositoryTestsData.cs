using System;
using System.Collections.Generic;
using mini_ITS.Core.Database;
using mini_ITS.Core.Models;

namespace mini_ITS.Core.Tests.Repository
{
    public class UsersRepositoryTestsData
    {
        public static IEnumerable<Users> UsersCases
        {
            get
            {
                yield return new Users
                {
                    Id = new Guid("5ee56913-7441-4305-8b31-bc86584fff47"),
                    Login = "bartlbri",
                    FirstName = "Brigita",
                    LastName = "Bartles",
                    Department = "Sales",
                    Email = "brigita.bartles@example.com",
                    Phone = "505XXX505",
                    Role = "User",
                    PasswordHash = "duMypqCJ6/H4N6VxJvAcqjgOSd7fGHojrn1qbf9Nmhgn/Vk4tS/un0jF0OR2+bCf3Qz1gJHyZIWNSw3J6kNKmQ=="
                };
                yield return new Users
                {
                    Id = new Guid("3131c3ea-5607-4fa0-b9d7-712ff41baa4e"),
                    Login = "atkincol",
                    FirstName = "Colin",
                    LastName = "Atkins",
                    Department = "Marketing",
                    Email = "colin.atkins@example.com",
                    Phone = "507XXX507",
                    Role = "Manager",
                    PasswordHash = "lpaRF1gwtbaR4GYdwIODxQ59uYhV8d0Tf0WjGsP3fwKrW46w+eaiA6hDBIuDrU/1ObpwsbaGaKB7vQwcCyjVpg=="
                };
                yield return new Users
                {
                    Id = new Guid("dfe4d2bf-08ea-4d86-9ccd-4e1ce3459c48"),
                    Login = "kirbyisa",
                    FirstName = "Isabella",
                    LastName = "Kirby",
                    Department = "Marketing",
                    Email = "isabella.kirby@example.com",
                    Phone = "507XXX507",
                    Role = "Manager",
                    PasswordHash = "s6Rs+ACBhZNSZDqS9+oAHcY6HbXabI6EZ2SoAd2I/6wzwH54BaspoCYfut/Zj9qQXis7erobV/RhFGALfRZfxg=="
                };
                yield return new Users
                {
                    Id = new Guid("99fcf2cf-9080-4c61-bd3d-66f78ce4e39f"),
                    Login = "trevidor",
                    FirstName = "Dora",
                    LastName = "Trevino",
                    Department = "Sales",
                    Email = "dora.trevino@example.com",
                    Phone = "509XXX509",
                    Role = "User",
                    PasswordHash = "SY1O1aLA3Ii7mvPeWy0535B1cwspmPjvrL94FmYlpgWGpIx81yUzD/JToTjdQvUQm4HeQUw7ZaQ7xwxTGMYF7Q=="
                };
            }
        }
        public static IEnumerable<SqlPagedQuery<Users>> SqlPagedQueryCases
        {
            get
            {
                yield return new SqlPagedQuery<Users>
                {
                    Filter = new List<SqlQueryCondition>()
                    {
                        new SqlQueryCondition
                        {
                            Name = "Department",
                            Operator = SqlQueryOperator.Equal,
                            Value = "Sales"
                        },
                        new SqlQueryCondition
                        {
                            Name = "Role",
                            Operator = SqlQueryOperator.Equal,
                            Value = null
                        }
                    },
                    SortColumnName = "Login",
                    SortDirection = "DESC",
                    Page = 1,
                    ResultsPerPage = 5
                };
                yield return new SqlPagedQuery<Users>
                {
                    Filter = new List<SqlQueryCondition>()
                    {
                        new SqlQueryCondition
                        {
                            Name = "Department",
                            Operator = SqlQueryOperator.Equal,
                            Value = "Development"
                        },
                        new SqlQueryCondition
                        {
                            Name = "Role",
                            Operator = SqlQueryOperator.Equal,
                            Value = "User"
                        }
                    },
                    SortColumnName = "FirstName",
                    SortDirection = "ASC",
                    Page = 1,
                    ResultsPerPage = 2
                };
                yield return new SqlPagedQuery<Users>
                {
                    Filter = new List<SqlQueryCondition>()
                    {
                        new SqlQueryCondition
                        {
                            Name = "Department",
                            Operator = SqlQueryOperator.Equal,
                            Value = null
                        },
                        new SqlQueryCondition
                        {
                            Name = "Role",
                            Operator = SqlQueryOperator.Equal,
                            Value = null
                        }
                    },
                    SortColumnName = "Email",
                    SortDirection = "ASC",
                    Page = 1,
                    ResultsPerPage = 10
                };
                yield return new SqlPagedQuery<Users>
                {
                    Filter = new List<SqlQueryCondition>()
                    {
                        new SqlQueryCondition
                        {
                            Name = "Department",
                            Operator = SqlQueryOperator.Equal,
                            Value = "Research"
                        },
                        new SqlQueryCondition
                        {
                            Name = "Role",
                            Operator = SqlQueryOperator.Equal,
                            Value = "Manager"
                        }
                    },
                    SortColumnName = "Login",
                    SortDirection = "ASC",
                    Page = 1,
                    ResultsPerPage = 3
                };
            }
        }
        public static IEnumerable<Users> CRUDCases
        {
            get
            {
                yield return new Users
                {
                    Login = "ciszetad",
                    FirstName = "Tadeusz",
                    LastName = "Ciszewski",
                    Department = "IT",
                    Email = "tadeusz.ciszewski@example.com",
                    Phone = "502XXX502",
                    Role = "User",
                    PasswordHash = "iAYfIwMTEqPYMRmGl8nyv4pgZttVB6M6m8B55iImIFb7WnlkKZKH9w8dFgQY65X1VYZnX+Qnb791DMnauIICEg =="
                };
                yield return new Users
                {
                    Login = "ciszeaga",
                    FirstName = "Agnieszka",
                    LastName = "Ciszewska",
                    Department = "IT",
                    Email = "agnieszka.ciszewska@example.com",
                    Phone = "505XXX505",
                    Role = "User",
                    PasswordHash = "9png3yh4cPfTHfHJAq7I/18bkGan5QeJ7J7hMlv4XEqMGXSG9w8PHvDEt3TYg2QMiMXHhtoPu6BmLrVGY/Lx3Q=="
                };

            }
        }
        public static IEnumerable<string> testDepartment
        {
            get
            {
                yield return null;
                yield return "Sales";
                yield return "Research";
            }
        }
        public static IEnumerable<string> testRole
        {
            get
            {
                yield return null;
                yield return "User";
                yield return "Manager";
            }
        }
    }
}