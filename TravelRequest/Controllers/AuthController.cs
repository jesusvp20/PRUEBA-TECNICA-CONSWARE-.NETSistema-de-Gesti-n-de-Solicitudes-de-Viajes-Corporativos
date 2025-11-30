using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TravelRequests.Application.DTOs;
using TravelRequests.Application.Servicies;

namespace TravelRequests.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUsuariosService _usuariosServices;
        private readonly IValidator<LoginUsuarioDto> _loginValidator;

        public AuthController(
            IUsuariosService usuariosServices,
            IValidator<LoginUsuarioDto> loginValidator)
        {
            _usuariosServices = usuariosServices;
            _loginValidator = loginValidator;
        }

        // Login retorna JWT
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUsuarioDto request)
        {
            // Validar con FluentValidation
            var validacion = await _loginValidator.ValidateAsync(request);
            if (!validacion.IsValid)
                return BadRequest(new { mensaje = string.Join(", ", validacion.Errors.Select(e => e.ErrorMessage)) });

            try
            {
                var token = await _usuariosServices.LoginAsync(request);
                return Ok(new { mensaje = "Login exitoso", token, tipo = "Bearer" });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { mensaje = "Credenciales inválidas" });
            }
        }
    }
}
