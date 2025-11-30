using Microsoft.AspNetCore.Mvc;
using TravelRequests.Application.DTOs;
using TravelRequests.Application.Servicies;

namespace TravelRequests.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecuperacionContraseñaController : ControllerBase
    {
        private readonly IUsuariosService _usuariosServices;

        public RecuperacionContraseñaController(IUsuariosService usuariosServices)
        {
            _usuariosServices = usuariosServices;
        }

        // Solicita código de recuperación
        [HttpPost("solicitar")]
        public async Task<IActionResult> SolicitarRecuperacion([FromBody] OlvidarContraseñaDto request)
        {
            try
            {
                var codigo = await _usuariosServices.OlvidarContraseñaAsync(request);
                return Ok(new { mensaje = "Código generado", codigo, expiraEn = "5 minutos" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // Restablecer contraseña con código
        [HttpPost("restablecer")]
        public async Task<IActionResult> RestablecerContraseña([FromBody] RestablecerContraseñaDto request)
        {
            try
            {
                await _usuariosServices.RestablecerContraseñaAsync(request);
                return Ok(new { mensaje = "Contraseña restablecida" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}
