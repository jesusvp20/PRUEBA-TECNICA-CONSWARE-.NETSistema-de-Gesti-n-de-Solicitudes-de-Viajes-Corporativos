using Moq;
using Microsoft.Extensions.Logging;
using TravelRequests.Application.DTOs;
using TravelRequests.Application.Servicies;
using TravelRequests.Domain.Entities;
using TravelRequests.Domain.Enums;
using TravelRequests.Domain.Interfaces;

namespace TravelRequests.Tests;

// Tests servicio solicitudes viaje
public class SolicitudesViajeServiceTests
{
    private readonly Mock<IRepositorioSolicitudViaje> _mockRepo;
    private readonly Mock<ILogger<SolicitudesViajeService>> _mockLogger;
    private readonly SolicitudesViajeService _service;

    public SolicitudesViajeServiceTests()
    {
        _mockRepo = new Mock<IRepositorioSolicitudViaje>();
        _mockLogger = new Mock<ILogger<SolicitudesViajeService>>();
        _service = new SolicitudesViajeService(_mockRepo.Object, _mockLogger.Object);
    }

    #region CrearSolicitudAsync Tests

    [Fact]
    // Crear solicitud válida retorna DTO con estado Pendiente
    public async Task CrearSolicitudAsync_DatosValidos_RetornaSolicitudPendiente()
    {
        // Arrange
        var dto = new CrearSolicitudViajeDto
        {
            CiudadOrigen = "Bogotá",
            CiudadDestino = "Medellín",
            FechaIda = DateTime.Now.AddDays(7),
            FechaRegreso = DateTime.Now.AddDays(14),
            Justificacion = "Reunión de trabajo"
        };
        _mockRepo.Setup(r => r.AgregarAsync(It.IsAny<SolicitudViaje>())).Returns(Task.CompletedTask);

        // Act
        var result = await _service.CrearSolicitudAsync(dto, usuarioId: 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Bogotá", result.CiudadOrigen);
        Assert.Equal("Medellín", result.CiudadDestino);
        Assert.Equal("Pendiente", result.Estado);
        Assert.Equal(1, result.UsuarioId);
    }

    [Fact]
    // Crear solicitud con mismo origen y destino lanza excepción
    public async Task CrearSolicitudAsync_MismoOrigenDestino_LanzaArgumentException()
    {
        // Arrange
        var dto = new CrearSolicitudViajeDto
        {
            CiudadOrigen = "Bogotá",
            CiudadDestino = "Bogotá",
            FechaIda = DateTime.Now.AddDays(7),
            FechaRegreso = DateTime.Now.AddDays(14),
            Justificacion = "Viaje"
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CrearSolicitudAsync(dto, 1));
        Assert.Equal("Ciudad origen y destino deben ser diferentes", ex.Message);
    }

    [Fact]
    // Crear solicitud con fecha regreso menor a ida lanza excepción
    public async Task CrearSolicitudAsync_FechaRegresoMenorIda_LanzaArgumentException()
    {
        // Arrange
        var dto = new CrearSolicitudViajeDto
        {
            CiudadOrigen = "Bogotá",
            CiudadDestino = "Cali",
            FechaIda = DateTime.Now.AddDays(14),
            FechaRegreso = DateTime.Now.AddDays(7), // Menor que ida
            Justificacion = "Viaje"
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CrearSolicitudAsync(dto, 1));
        Assert.Equal("Fecha regreso debe ser mayor a fecha ida", ex.Message);
    }

    [Fact]
    // Crear solicitud con misma fecha ida y regreso lanza excepción
    public async Task CrearSolicitudAsync_MismaFechaIdaRegreso_LanzaArgumentException()
    {
        // Arrange
        var fecha = DateTime.Now.AddDays(7);
        var dto = new CrearSolicitudViajeDto
        {
            CiudadOrigen = "Bogotá",
            CiudadDestino = "Cali",
            FechaIda = fecha,
            FechaRegreso = fecha, // Igual que ida
            Justificacion = "Viaje"
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CrearSolicitudAsync(dto, 1));
        Assert.Equal("Fecha regreso debe ser mayor a fecha ida", ex.Message);
    }

    #endregion

    #region ObtenerSolicitudesPorUsuarioAsync Tests

    [Fact]
    // Obtener solicitudes del usuario retorna lista correcta
    public async Task ObtenerSolicitudesPorUsuarioAsync_UsuarioConSolicitudes_RetornaLista()
    {
        // Arrange
        var solicitudes = new List<SolicitudViaje>
        {
            new SolicitudViaje
            {
                Id = 1,
                CiudadOrigen = "Bogotá",
                CiudadDestino = "Medellín",
                FechaIda = DateTime.Now,
                FechaRegreso = DateTime.Now.AddDays(3),
                Justificacion = "Reunión",
                Estado = EstadoSolicitud.Pendiente,
                FechaCreacion = DateTime.Now,
                UsuarioId = 1
            }
        };
        _mockRepo.Setup(r => r.ObtenerPorUsuarioAsync(1)).ReturnsAsync(solicitudes);

        // Act
        var result = await _service.ObtenerSolicitudesPorUsuarioAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Bogotá", result[0].CiudadOrigen);
    }

    [Fact]
    // Obtener solicitudes de usuario sin solicitudes retorna lista vacía
    public async Task ObtenerSolicitudesPorUsuarioAsync_UsuarioSinSolicitudes_RetornaListaVacia()
    {
        // Arrange
        _mockRepo.Setup(r => r.ObtenerPorUsuarioAsync(1)).ReturnsAsync(new List<SolicitudViaje>());

        // Act
        var result = await _service.ObtenerSolicitudesPorUsuarioAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #endregion

    #region ObtenerTodasSolicitudesAsync Tests

    [Fact]
    // Obtener todas las solicitudes retorna lista completa
    public async Task ObtenerTodasSolicitudesAsync_HaySolicitudes_RetornaLista()
    {
        // Arrange
        var solicitudes = new List<SolicitudViaje>
        {
            new SolicitudViaje { Id = 1, CiudadOrigen = "A", CiudadDestino = "B", FechaIda = DateTime.Now, FechaRegreso = DateTime.Now.AddDays(1), Justificacion = "X", FechaCreacion = DateTime.Now, UsuarioId = 1 },
            new SolicitudViaje { Id = 2, CiudadOrigen = "C", CiudadDestino = "D", FechaIda = DateTime.Now, FechaRegreso = DateTime.Now.AddDays(1), Justificacion = "Y", FechaCreacion = DateTime.Now, UsuarioId = 2 }
        };
        _mockRepo.Setup(r => r.ObtenerTodosAsync()).ReturnsAsync(solicitudes);

        // Act
        var result = await _service.ObtenerTodasSolicitudesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    #endregion

    #region CambiarEstadoAsync Tests

    [Fact]
    // Cambiar estado a Aprobada como Aprobador funciona
    public async Task CambiarEstadoAsync_AprobadorAproba_RetornaSolicitudAprobada()
    {
        // Arrange
        var solicitud = new SolicitudViaje
        {
            Id = 1,
            CiudadOrigen = "Bogotá",
            CiudadDestino = "Cali",
            FechaIda = DateTime.Now,
            FechaRegreso = DateTime.Now.AddDays(3),
            Justificacion = "Trabajo",
            Estado = EstadoSolicitud.Pendiente,
            FechaCreacion = DateTime.Now,
            UsuarioId = 1
        };
        _mockRepo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(solicitud);
        _mockRepo.Setup(r => r.ActualizarAsync(It.IsAny<SolicitudViaje>())).Returns(Task.CompletedTask);

        var dto = new CambiarEstadoSolicitudDto { Estado = "Aprobada" };

        // Act
        var result = await _service.CambiarEstadoAsync(1, dto, "Aprobador");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Aprobada", result.Estado);
    }

    [Fact]
    // Cambiar estado a Rechazada como Aprobador funciona
    public async Task CambiarEstadoAsync_AprobadorRechaza_RetornaSolicitudRechazada()
    {
        // Arrange
        var solicitud = new SolicitudViaje
        {
            Id = 1,
            CiudadOrigen = "Bogotá",
            CiudadDestino = "Cali",
            FechaIda = DateTime.Now,
            FechaRegreso = DateTime.Now.AddDays(3),
            Justificacion = "Trabajo",
            Estado = EstadoSolicitud.Pendiente,
            FechaCreacion = DateTime.Now,
            UsuarioId = 1
        };
        _mockRepo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(solicitud);
        _mockRepo.Setup(r => r.ActualizarAsync(It.IsAny<SolicitudViaje>())).Returns(Task.CompletedTask);

        var dto = new CambiarEstadoSolicitudDto { Estado = "Rechazada" };

        // Act
        var result = await _service.CambiarEstadoAsync(1, dto, "Aprobador");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Rechazada", result.Estado);
    }

    [Fact]
    // Cambiar estado como Solicitante lanza excepción
    public async Task CambiarEstadoAsync_RolSolicitante_LanzaUnauthorizedException()
    {
        // Arrange
        var dto = new CambiarEstadoSolicitudDto { Estado = "Aprobada" };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _service.CambiarEstadoAsync(1, dto, "Solicitante"));
    }

    [Fact]
    // Cambiar estado de solicitud inexistente lanza excepción
    public async Task CambiarEstadoAsync_SolicitudNoExiste_LanzaArgumentException()
    {
        // Arrange
        _mockRepo.Setup(r => r.ObtenerPorIdAsync(999)).ReturnsAsync((SolicitudViaje?)null);
        var dto = new CambiarEstadoSolicitudDto { Estado = "Aprobada" };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CambiarEstadoAsync(999, dto, "Aprobador"));
        Assert.Equal("Solicitud no encontrada", ex.Message);
    }

    [Fact]
    // Cambiar a estado inválido lanza excepción
    public async Task CambiarEstadoAsync_EstadoInvalido_LanzaArgumentException()
    {
        // Arrange
        var solicitud = new SolicitudViaje { Id = 1, Estado = EstadoSolicitud.Pendiente };
        _mockRepo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(solicitud);
        var dto = new CambiarEstadoSolicitudDto { Estado = "EstadoFalso" };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CambiarEstadoAsync(1, dto, "Aprobador"));
        Assert.Equal("Estado inválido (Aprobada/Rechazada)", ex.Message);
    }

    [Fact]
    // Cambiar estado a Pendiente lanza excepción
    public async Task CambiarEstadoAsync_CambiarAPendiente_LanzaArgumentException()
    {
        // Arrange
        var solicitud = new SolicitudViaje { Id = 1, Estado = EstadoSolicitud.Aprobada };
        _mockRepo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(solicitud);
        var dto = new CambiarEstadoSolicitudDto { Estado = "Pendiente" };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CambiarEstadoAsync(1, dto, "Aprobador"));
        Assert.Equal("No se puede cambiar a Pendiente", ex.Message);
    }

    #endregion
}

