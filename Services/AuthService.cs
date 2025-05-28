using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ExpenseTrackerApi.Data;
using ExpenseTrackerApi.DTOs.Auth;
using ExpenseTrackerApi.Helpers;
using ExpenseTrackerApi.Interfaces;
using ExpenseTrackerApi.Models;

namespace ExpenseTrackerApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly JwtSettings _jwtSettings;
        public AuthService(AppDbContext db, IOptions<JwtSettings> jwtOptions)
        {
            _db = db;
            _jwtSettings = jwtOptions.Value;
        }

        public async Task RegisterAsync(RegisterDto dto)
        {
            // Check user in system
            if (await _db.Users.AnyAsync(u => u.Username == dto.Username))
                throw new ApplicationException($"Username '{dto.Username}' is already taken");

            // Salt & Hash password
            using var hmac = new HMACSHA512();
            var user = new User
            {
                Username = dto.Username,
                PasswordSalt = hmac.Key,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password))
            };

            // Save user
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            // Get user
            var user = await _db.Users.SingleOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null)
                throw new ApplicationException("User not found!");

            // Check password with salt password
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));
            if (!computedHash.SequenceEqual(user.PasswordHash))
                throw new ApplicationException("Password not correct");

            // Create Jwt
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name,           user.Username)
                }),
                Expires = DateTime.UtcNow.AddDays(_jwtSettings.ExpiresInDay),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };
            
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}