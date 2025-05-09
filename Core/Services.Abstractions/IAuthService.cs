using Shared.Dtos;
using Shared.Dtos.OrderDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IAuthService
    {
        Task<UserResultDto> LoginAsync(LoginDto loginDto);
        Task<UserResultDto> RegisterAsync(RegisterDto registerDto);

        // Check Email Exists
        Task<bool> CheckEmailExistsAsync(string email);

        // Get Current User
        Task<UserResultDto> GetCurrentUserByEmailAsync(string email);

        // Get Address
        Task<AddressDto> GetCurrentUserAddressByEmailAsync(string email);

        // Update Address
        Task<AddressDto> UpdateCurrentUserAddressByEmailAsync(string email, AddressDto addressDto);
    }
}
