using Domain.Entities;
using Domain.Aggregates;
﻿using Domain.DTOs;

namespace ServicesClasseslibrary.Interface
{
    public interface IUserService
    {
        void Add(LoginDTO user);
        LoginDTO Validate(LoginDTO model);
    }
}
