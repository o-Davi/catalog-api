using CatalogApi.Common;
using System.Net;
using System.Text.Json;

namespace CatalogApi.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (UnauthorizedAccessException ex)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;

            var response = ApiResponse<object>.Fail("Credenciais inválidas");

            await context.Response.WriteAsJsonAsync(response);

        }

        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado");

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new
            {
                status = context.Response.StatusCode,
                title = "Erro interno do servidor",
                detail = "Ocorreu um erro inesperado. Tente novamente mais tarde."
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response)
            );
    }
    }
}
