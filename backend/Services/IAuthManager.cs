using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Models;

namespace backend.Services
{
    public interface IAuthManager
    {
        Task<bool> ValidateUser(UserCredentialsDTO userCredentialsDTO);
        Task<string> CreateToken();
    }
}