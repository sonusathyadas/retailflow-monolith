using RetailFlow.Application.DTOs;

namespace RetailFlow.Application.Services
{
    public interface IAuthService
    {
        AuthResponse Register(RegisterRequest request);
        AuthResponse Login(LoginRequest request);
        AuthResponse RefreshToken(string refreshToken);
        UserDto GetUserById(int id);
    }
}
