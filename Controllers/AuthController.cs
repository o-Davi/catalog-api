using CatalogApi.Common;
using CatalogApi.Dtos.Auth;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto dto)
    {
        var result = await _service.LoginAsync(dto);

        return Ok(
            ApiResponse<LoginResponseDto>.Ok(
                result,
                "Login realizado com sucesso"
            )
        );
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequestDto dto)
    {
        await _service.RegisterAsync(dto);

        return StatusCode(
            StatusCodes.Status201Created,
            ApiResponse<object>.Ok(null, "Usuário cadastrado com sucesso")
        );
    }

}


