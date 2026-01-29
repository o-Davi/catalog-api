using CatalogApi.Dtos.Auth;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
    Task RegisterAsync(RegisterRequestDto dto);
}
