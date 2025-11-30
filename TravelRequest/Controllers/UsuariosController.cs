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
        private readonly IUsuariosServices _usuariosServices;

        public UsuariosController(IUsuariosServices usuariosServices)
        {
            _usuariosServices = usuariosServices;
        }

        // Registro de usuario
        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] RegistrarUsuarioDto request)
        {
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
                return Forbid();
            }
        }
    }
}
