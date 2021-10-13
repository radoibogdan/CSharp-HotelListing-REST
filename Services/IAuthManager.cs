using HotelListing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Services
{
    public interface IAuthManager
    {
        // Validate user
        Task<bool> ValidateUser(LoginUserDTO userDTO);

        // Create and return the token
        Task<string> CreateToken();
    }
}
