﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mini_ITS.Core.Database;
using mini_ITS.Core.Models;

namespace mini_ITS.Core.Repository
{
    public interface IUsersRepository
    {
        Task<IEnumerable<Users>> GetAllAsync();
        Task<SqlPagedResult<Users>> GetAllAsync(SqlPagedQuery<Users> sqlPagedQuery);
        Task<Users> GetUserAsync(Guid id);
    }
}
