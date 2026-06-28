using Microsoft.Extensions.Configuration;

namespace OrderManagementAPI.Infrastructure.Authentication;

public class AuthService
{
    private readonly JwtTokenService _jwtTokenService;
    private readonly string _validEmail;
    private readonly string _validPassword;

    public AuthService(JwtTokenService jwtTokenService, IConfiguration configuration)
    {
        _jwtTokenService = jwtTokenService;
        _validEmail = configuration["Auth:ValidEmail"] ?? string.Empty;
        _validPassword = configuration["Auth:ValidPassword"] ?? string.Empty;
    }

    public string? Login(string email, string password)
    {
        if (email == _validEmail && password == _validPassword)
        {
            return _jwtTokenService.GenerateToken(email);
        }

        return null;
    }
}