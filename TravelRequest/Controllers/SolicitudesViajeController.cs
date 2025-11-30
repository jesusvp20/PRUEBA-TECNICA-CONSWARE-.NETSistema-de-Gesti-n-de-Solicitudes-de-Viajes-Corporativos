using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TravelRequests.Application.DTOs;
using TravelRequests.Application.Servicies;

namespace TravelRequest.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class SolicitudesViajeController : ControllerBase
{
    private readonly ISolicitudesViajeService _solicitudesService;

    public SolicitudesViajeController(ISolicitudesViajeService solicitudesService)
    {
        _solicitudesService = solicitudesService;
    }

    // Crear solicitud
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearSolicitudViajeDto request)
    {
        try
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var solicitud = await _solicitudesService.CrearSolicitudAsync(request, usuarioId);
            return CreatedAtAction(nameof(ObtenerMisSolicitudes), solicitud);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    // Listar mis solicitudes
    [HttpGet("mis-solicitudes")]
    public async Task<IActionResult> ObtenerMisSolicitudes()
    {
        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var solicitudes = await _solicitudesService.ObtenerSolicitudesPorUsuarioAsync(usuarioId);
        return Ok(solicitudes);
    }

    // Listar todas (solo Aprobador)
    [HttpGet]
    [Authorize(Roles = "Aprobador")]
    public async Task<IActionResult> ObtenerTodas()
    {
        var solicitudes = await _solicitudesService.ObtenerTodasSolicitudesAsync();
        return Ok(solicitudes);
    }

    // Cambiar estado (solo Aprobador)
    [HttpPatch("{id}/estado")]
    [Authorize(Roles = "Aprobador")]
    public async Task<IActionResult> CambiarEstado(int id, [FromBody] CambiarEstadoSolicitudDto request)
    {
        try
        {
            var rol = User.FindFirst(ClaimTypes.Role)?.Value ?? "";
            var solicitud = await _solicitudesService.CambiarEstadoAsync(id, request, rol);
            return Ok(solicitud);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (UnauthorizedAccessException)
        {
            return StatusCode(403, new { mensaje = "Solo usuarios con rol Aprobador pueden cambiar estado" });
        }
    }
}

