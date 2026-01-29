using CatalogApi.Data;
using CatalogApi.Domain.Entities;
using CatalogApi.Dtos.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace CatalogApi.Services;

public class AuthService : IAuthService
{
    private readonly CatalogDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(CatalogDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user is null ||
            !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Credenciais inválidas");
        }

        var jwt = _config.GetSection("Jwt");

        if (string.IsNullOrWhiteSpace(jwt["Key"]) ||
            string.IsNullOrWhiteSpace(jwt["Issuer"]) ||
            string.IsNullOrWhiteSpace(jwt["Audience"]) ||
            string.IsNullOrWhiteSpace(jwt["ExpiresInMinutes"]))
        {
            throw new InvalidOperationException("Configuração JWT incompleta ou inválida");
        }

        if (!int.TryParse(jwt["ExpiresInMinutes"], out var expiresInMinutes))
        {
            throw new InvalidOperationException("Jwt:ExpiresInMinutes deve ser um número inteiro");
        }

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Name ?? user.Email)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwt["Key"]!)
        );

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
            signingCredentials: credentials
        );

        return new LoginResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = token.ValidTo
        };
    }

    public async Task RegisterAsync(RegisterRequestDto dto)
    {
        var exists = await _context.Users
            .AnyAsync(u => u.Email == dto.Email);

        if (exists)
            throw new BadHttpRequestException("Usuário já existe");

        var user = new User
        {
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }
}