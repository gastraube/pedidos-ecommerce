using Microsoft.AspNetCore.Mvc;
using OrderManagementAPI.Infrastructure.Authentication;

namespace OrderManagementAPI.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var token = _authService.Login(request.Email, request.Password);

        if (token == null)
            return Unauthorized(new { error = "Invalid credentials" });

        return Ok(new { token });
    }
}

public record LoginRequest(string Email, string Password);