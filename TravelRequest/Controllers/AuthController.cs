using Microsoft.AspNetCore.Mvc;
using TravelRequests.Application.DTOs;
using TravelRequests.Application.Servicies;

namespace TravelRequests.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUsuariosServices _usuariosServices;

        public AuthController(IUsuariosServices usuariosServices)
        {
            _usuariosServices = usuariosServices;
        }

        // Login - retorna JWT con rol
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUsuarioDto request)
        {
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
