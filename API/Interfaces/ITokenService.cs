using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entites;

namespace API.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
    }
}