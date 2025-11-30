using FluentValidation;
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
        private readonly IValidator<OlvidarContraseñaDto> _olvidarValidator;
        private readonly IValidator<RestablecerContraseñaDto> _restablecerValidator;

        public RecuperacionContraseñaController(
            IUsuariosService usuariosServices,
            IValidator<OlvidarContraseñaDto> olvidarValidator,
            IValidator<RestablecerContraseñaDto> restablecerValidator)
        {
            _usuariosServices = usuariosServices;
            _olvidarValidator = olvidarValidator;
            _restablecerValidator = restablecerValidator;
        }

        // Solicita código de recuperación
        [HttpPost("solicitar")]
        public async Task<IActionResult> SolicitarRecuperacion([FromBody] OlvidarContraseñaDto request)
        {
            // Validar con FluentValidation
            var validacion = await _olvidarValidator.ValidateAsync(request);
            if (!validacion.IsValid)
                return BadRequest(new { mensaje = string.Join(", ", validacion.Errors.Select(e => e.ErrorMessage)) });

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
            // Validar con FluentValidation
            var validacion = await _restablecerValidator.ValidateAsync(request);
            if (!validacion.IsValid)
                return BadRequest(new { mensaje = string.Join(", ", validacion.Errors.Select(e => e.ErrorMessage)) });

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
