using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using RetailFlow.Application.DTOs;
using RetailFlow.Domain.Entities;
using RetailFlow.Domain.Interfaces;
using RetailFlow.Shared.Helpers;
using Serilog;

namespace RetailFlow.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IRepository<Role> _roleRepo;
        private readonly string _jwtSecret;
        private readonly int _jwtExpiryMinutes;
        private static readonly ILogger _log = Log.ForContext<AuthService>();

        public AuthService(IUserRepository userRepo, IRepository<Role> roleRepo,
            string jwtSecret, int jwtExpiryMinutes = 60)
        {
            _userRepo = userRepo;
            _roleRepo = roleRepo;
            _jwtSecret = jwtSecret;
            _jwtExpiryMinutes = jwtExpiryMinutes;
        }

        public AuthResponse Register(RegisterRequest request)
        {
            var existing = _userRepo.GetByEmail(request.Email);
            if (existing != null)
                throw new InvalidOperationException("Email already registered.");

            // Default role: Customer (Id=1)
            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email.ToLowerInvariant(),
                PasswordHash = PasswordHelper.HashPassword(request.Password),
                RoleId = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _userRepo.Add(user);
            _userRepo.SaveChanges();

            _log.Information("New user registered: {Email}", user.Email);
            return BuildAuthResponse(user, "Customer");
        }

        public AuthResponse Login(LoginRequest request)
        {
            var user = _userRepo.GetByEmail(request.Email.ToLowerInvariant());
            if (user == null || !user.IsActive)
                throw new UnauthorizedAccessException("Invalid credentials.");

            if (!PasswordHelper.VerifyPassword(request.Password, user.PasswordHash))
            {
                _log.Warning("Failed login attempt for {Email}", request.Email);
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            var role = _roleRepo.GetById(user.RoleId);
            _log.Information("User logged in: {Email}", user.Email);
            return BuildAuthResponse(user, role?.Name ?? "Customer");
        }

        public AuthResponse RefreshToken(string refreshToken)
        {
            var user = _userRepo.GetByRefreshToken(refreshToken);
            if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");

            var role = _roleRepo.GetById(user.RoleId);
            return BuildAuthResponse(user, role?.Name ?? "Customer");
        }

        public UserDto GetUserById(int id)
        {
            var user = _userRepo.GetById(id);
            if (user == null) return null;

            var role = _roleRepo.GetById(user.RoleId);
            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = role?.Name,
                IsActive = user.IsActive
            };
        }

        private AuthResponse BuildAuthResponse(User user, string roleName)
        {
            var refreshToken = TokenHelper.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            _userRepo.Update(user);
            _userRepo.SaveChanges();

            var accessToken = GenerateJwt(user, roleName);

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = _jwtExpiryMinutes * 60,
                User = new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Role = roleName,
                    IsActive = user.IsActive
                }
            };
        }

        private string GenerateJwt(User user, string roleName)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, roleName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: "RetailFlow",
                audience: "RetailFlow",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtExpiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
