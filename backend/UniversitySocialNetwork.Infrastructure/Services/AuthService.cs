using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UniversitySocialNetwork.Core.DTOs;
using UniversitySocialNetwork.Core.Entities;
using UniversitySocialNetwork.Core.Services;
using UniversitySocialNetwork.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace UniversitySocialNetwork.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await GetUserByEmailAsync(loginDto.Email);
            if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Credenciales inválidas");
            }

            var token = GenerateJwtToken(user);
            var expires = DateTime.UtcNow.AddHours(24);

            return new AuthResponseDto
            {
                Token = token,
                Expires = expires,
                User = new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    StudentId = user.StudentId,
                    Department = user.Department,
                    AcademicYear = user.AcademicYear,
                    Bio = user.Bio,
                    ProfileImageUrl = user.ProfileImageUrl,
                    CreatedAt = user.CreatedAt
                }
            };
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            // Verificar si el email ya existe
            var existingUser = await GetUserByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("El email ya está registrado");
            }

            // Verificar si el StudentId ya existe (si se proporciona)
            if (!string.IsNullOrEmpty(registerDto.StudentId))
            {
                var existingStudentId = await _context.Users
                    .FirstOrDefaultAsync(u => u.StudentId == registerDto.StudentId);
                if (existingStudentId != null)
                {
                    throw new InvalidOperationException("El ID de estudiante ya está registrado");
                }
            }

            var user = new User
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                PasswordHash = HashPassword(registerDto.Password),
                StudentId = registerDto.StudentId,
                Department = registerDto.Department,
                AcademicYear = registerDto.AcademicYear?.ToString(),
                Bio = registerDto.Bio,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);
            var expires = DateTime.UtcNow.AddHours(24);

            return new AuthResponseDto
            {
                Token = token,
                Expires = expires,
                User = new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    StudentId = user.StudentId,
                    Department = user.Department,
                    AcademicYear = user.AcademicYear,
                    Bio = user.Bio,
                    ProfileImageUrl = user.ProfileImageUrl,
                    CreatedAt = user.CreatedAt
                }
            };
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }

            if (!VerifyPassword(changePasswordDto.CurrentPassword, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Contraseña actual incorrecta");
            }

            user.PasswordHash = HashPassword(changePasswordDto.NewPassword);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<bool> ValidateUserAsync(string email, string password)
        {
            var user = await GetUserByEmailAsync(email);
            return user != null && VerifyPassword(password, user.PasswordHash);
        }

        public string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? "your-super-secret-key-that-is-at-least-32-characters-long";
            var issuer = jwtSettings["Issuer"] ?? "UniversitySocialNetwork";
            var audience = jwtSettings["Audience"] ?? "UniversitySocialNetwork";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim("StudentId", user.StudentId ?? ""),
                new Claim("Department", user.Department ?? "")
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string HashPassword(string password)
        {
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[16];
            rng.GetBytes(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(32);

            var hashBytes = new byte[48];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 32);

            return Convert.ToBase64String(hashBytes);
        }

        private bool VerifyPassword(string password, string hash)
        {
            // Check if it's a BCrypt hash (starts with $2a$, $2b$, $2x$, or $2y$)
            if (hash.StartsWith("$2"))
            {
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            
            // Legacy PBKDF2 verification for existing users
            try
            {
                var hashBytes = Convert.FromBase64String(hash);
                var salt = new byte[16];
                Array.Copy(hashBytes, 0, salt, 0, 16);

                using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
                var testHash = pbkdf2.GetBytes(32);

                for (int i = 0; i < 32; i++)
                {
                    if (hashBytes[i + 16] != testHash[i])
                    {
                        return false;
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}