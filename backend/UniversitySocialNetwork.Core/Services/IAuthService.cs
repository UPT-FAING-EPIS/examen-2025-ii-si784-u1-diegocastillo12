using UniversitySocialNetwork.Core.DTOs;
using UniversitySocialNetwork.Core.Entities;

namespace UniversitySocialNetwork.Core.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> ValidateUserAsync(string email, string password);
        string GenerateJwtToken(User user);
    }
}