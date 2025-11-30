using FluentValidation;
using System.Net;
using System.Text.Json;

namespace TravelRequest.Middleware;

// Middleware para manejo global de errores
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Ejecutar siguiente middleware
            await _next(context);
        }
        catch (Exception ex)
        {
            // Capturar y manejar excepción
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Error: {Message}", exception.Message);

        var response = context.Response;
        response.ContentType = "application/json";

        // Determinar código de estado según tipo de excepción
        var (statusCode, mensaje) = exception switch
        {
            ValidationException validationEx => (
                HttpStatusCode.BadRequest,
                string.Join(", ", validationEx.Errors.Select(e => e.ErrorMessage))
            ),
            ArgumentException argEx => (HttpStatusCode.BadRequest, argEx.Message),
            UnauthorizedAccessException => (HttpStatusCode.Forbidden, "Acceso denegado"),
            KeyNotFoundException => (HttpStatusCode.NotFound, "Recurso no encontrado"),
            _ => (HttpStatusCode.InternalServerError, "Error interno del servidor")
        };

        response.StatusCode = (int)statusCode;

        // Respuesta JSON consistente
        var result = JsonSerializer.Serialize(new
        {
            mensaje,
            codigo = (int)statusCode
        });

        await response.WriteAsync(result);
    }
}

// Extensión para registrar el middleware
public static class ErrorHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ErrorHandlingMiddleware>();
    }
}

