using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TravelRequests.Application.DTOs;
using TravelRequests.Application.Servicies;

namespace TravelRequests.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuariosService _usuariosServices;
        private readonly IValidator<RegistrarUsuarioDto> _registrarValidator;

        public UsuariosController(
            IUsuariosService usuariosServices,
            IValidator<RegistrarUsuarioDto> registrarValidator)
        {
            _usuariosServices = usuariosServices;
            _registrarValidator = registrarValidator;
        }

        // Registro de usuario
        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] RegistrarUsuarioDto request)
        {
            // Validar con FluentValidation
            var validacion = await _registrarValidator.ValidateAsync(request);
            if (!validacion.IsValid)
                return BadRequest(new { mensaje = string.Join(", ", validacion.Errors.Select(e => e.ErrorMessage)) });

            try
            {
                var usuario = await _usuariosServices.RegistrarUsuarioAsync(request);
                return CreatedAtAction(nameof(Registrar), new { id = usuario.Id }, usuario);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // Listar usuarios - solo Aprobadores
        [HttpGet]
        [Authorize(Roles = "Aprobador")]
        public async Task<IActionResult> ObtenerTodos()
        {
            var rol = User.FindFirst(ClaimTypes.Role)?.Value ?? "";
            try
            {
                var usuarios = await _usuariosServices.ObtenerTodosUsuariosAsync(rol);
                return Ok(usuarios);
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(403, new { mensaje = "Solo usuarios con rol Aprobador pueden acceder" });
            }
        }
    }
}
