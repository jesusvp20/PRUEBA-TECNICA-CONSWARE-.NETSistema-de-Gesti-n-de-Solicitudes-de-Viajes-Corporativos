using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using TravelRequests.Application.DTOs;
using TravelRequests.Application.Servicies;
using TravelRequests.Domain.Entities;
using TravelRequests.Domain.Enums;
using TravelRequests.Domain.interfaces;
using TravelRequests.Domain.Interfaces;

namespace TravelRequests.Tests;

// Tests servicio usuarios
public class UsuariosServiceTests
{
    private readonly Mock<IRepositorioUsuario> _mockRepoUsuario;
    private readonly Mock<IRepositorioCodigoRecuperacion> _mockRepoCodigo;
    private readonly Mock<IHasherContraseña> _mockHasher;
    private readonly Mock<ILogger<UsuariosService>> _mockLogger;
    private readonly IConfiguration _configuration;
    private readonly UsuariosService _service;

    public UsuariosServiceTests()
    {
        _mockRepoUsuario = new Mock<IRepositorioUsuario>();
        _mockRepoCodigo = new Mock<IRepositorioCodigoRecuperacion>();
        _mockHasher = new Mock<IHasherContraseña>();
        _mockLogger = new Mock<ILogger<UsuariosService>>();

        // Config JWT para tests
        var configData = new Dictionary<string, string?>
        {
            {"Jwt:Key", "TuClaveSecretaMuyLargaYSeguraParaJWT123456"},
            {"Jwt:Issuer", "TravelRequestsAPI"},
            {"Jwt:Audience", "TravelRequestsClients"},
            {"Jwt:ExpiresMinutes", "60"}
        };
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        _service = new UsuariosService(
            _mockRepoUsuario.Object,
            _mockRepoCodigo.Object,
            _mockHasher.Object,
            _mockLogger.Object,
            _configuration);
    }

    #region RegistrarUsuarioAsync Tests

    [Fact]
    // Registro exitoso retorna DTO con datos correctos
    public async Task RegistrarUsuarioAsync_CorreoNuevo_RetornaUsuarioDto()
    {
        // Arrange
        var dto = new RegistrarUsuarioDto
        {
            Nombre = "Test User",
            Correo = "test@test.com",
            Contraseña = "password123",
            Rol = "Solicitante"
        };
        _mockRepoUsuario.Setup(r => r.ExistePorCorreoAsync(dto.Correo)).ReturnsAsync(false);
        _mockHasher.Setup(h => h.HashContraseña(dto.Contraseña)).Returns("hashedPassword");
        _mockRepoUsuario.Setup(r => r.AgregarAsync(It.IsAny<Usuario>())).Returns(Task.CompletedTask);

        // Act
        var result = await _service.RegistrarUsuarioAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Nombre, result.Nombre);
        Assert.Equal(dto.Correo, result.Correo);
        Assert.Equal("Solicitante", result.Rol);
    }

    [Fact]
    // Registro con correo existente lanza excepción
    public async Task RegistrarUsuarioAsync_CorreoExistente_LanzaArgumentException()
    {
        // Arrange
        var dto = new RegistrarUsuarioDto { Correo = "existe@test.com" };
        _mockRepoUsuario.Setup(r => r.ExistePorCorreoAsync(dto.Correo)).ReturnsAsync(true);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(
            () => _service.RegistrarUsuarioAsync(dto));
        Assert.Equal("Correo ya registrado", ex.Message);
    }

    [Fact]
    // Registro con rol inválido lanza excepción
    public async Task RegistrarUsuarioAsync_RolInvalido_LanzaArgumentException()
    {
        // Arrange
        var dto = new RegistrarUsuarioDto
        {
            Nombre = "Test",
            Correo = "test@test.com",
            Contraseña = "pass",
            Rol = "RolInvalido"
        };
        _mockRepoUsuario.Setup(r => r.ExistePorCorreoAsync(dto.Correo)).ReturnsAsync(false);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(
            () => _service.RegistrarUsuarioAsync(dto));
        Assert.Equal("Rol inválido", ex.Message);
    }

    #endregion

    #region LoginAsync Tests

    [Fact]
    // Login exitoso retorna token JWT
    public async Task LoginAsync_CredencialesValidas_RetornaToken()
    {
        // Arrange
        var dto = new LoginUsuarioDto { Correo = "test@test.com", Contraseña = "password" };
        var usuario = new Usuario
        {
            Id = 1,
            Nombre = "Test",
            Correo = dto.Correo,
            Contraseña = "hashedPassword",
            Rol = RolUsuario.Solicitante
        };
        _mockRepoUsuario.Setup(r => r.ObtenerPorCorreoAsync(dto.Correo)).ReturnsAsync(usuario);
        _mockHasher.Setup(h => h.VerificarContraseña(dto.Contraseña, usuario.Contraseña)).Returns(true);

        // Act
        var token = await _service.LoginAsync(dto);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);
        Assert.Contains(".", token); // JWT tiene 3 partes separadas por punto
    }

    [Fact]
    // Login con usuario no encontrado lanza UnauthorizedAccessException
    public async Task LoginAsync_UsuarioNoExiste_LanzaUnauthorizedException()
    {
        // Arrange
        var dto = new LoginUsuarioDto { Correo = "noexiste@test.com", Contraseña = "pass" };
        _mockRepoUsuario.Setup(r => r.ObtenerPorCorreoAsync(dto.Correo)).ReturnsAsync((Usuario?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _service.LoginAsync(dto));
    }

    [Fact]
    // Login con contraseña incorrecta lanza UnauthorizedAccessException
    public async Task LoginAsync_ContraseñaIncorrecta_LanzaUnauthorizedException()
    {
        // Arrange
        var dto = new LoginUsuarioDto { Correo = "test@test.com", Contraseña = "wrongpass" };
        var usuario = new Usuario { Id = 1, Correo = dto.Correo, Contraseña = "hashedPassword" };
        _mockRepoUsuario.Setup(r => r.ObtenerPorCorreoAsync(dto.Correo)).ReturnsAsync(usuario);
        _mockHasher.Setup(h => h.VerificarContraseña(dto.Contraseña, usuario.Contraseña)).Returns(false);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _service.LoginAsync(dto));
    }

    #endregion

    #region OlvidarContraseñaAsync Tests

    [Fact]
    // Solicitar código exitoso retorna código de 6 dígitos
    public async Task OlvidarContraseñaAsync_CorreoValido_RetornaCodigo()
    {
        // Arrange
        var dto = new OlvidarContraseñaDto { Correo = "test@test.com" };
        var usuario = new Usuario { Id = 1, Correo = dto.Correo };
        _mockRepoUsuario.Setup(r => r.ObtenerPorCorreoAsync(dto.Correo)).ReturnsAsync(usuario);
        _mockRepoCodigo.Setup(r => r.InvalidarCodigosUsuarioAsync(usuario.Id)).Returns(Task.CompletedTask);
        _mockRepoCodigo.Setup(r => r.AgregarAsync(It.IsAny<CodigoRecuperacionContraseña>())).Returns(Task.CompletedTask);

        // Act
        var codigo = await _service.OlvidarContraseñaAsync(dto);

        // Assert
        Assert.NotNull(codigo);
        Assert.Equal(6, codigo.Length);
        Assert.True(int.TryParse(codigo, out _)); // Es numérico
    }

    [Fact]
    // Solicitar código con correo vacío lanza excepción
    public async Task OlvidarContraseñaAsync_CorreoVacio_LanzaArgumentException()
    {
        // Arrange
        var dto = new OlvidarContraseñaDto { Correo = "" };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(
            () => _service.OlvidarContraseñaAsync(dto));
        Assert.Equal("Correo requerido", ex.Message);
    }

    [Fact]
    // Solicitar código con correo no registrado lanza excepción
    public async Task OlvidarContraseñaAsync_CorreoNoRegistrado_LanzaArgumentException()
    {
        // Arrange
        var dto = new OlvidarContraseñaDto { Correo = "noexiste@test.com" };
        _mockRepoUsuario.Setup(r => r.ObtenerPorCorreoAsync(dto.Correo)).ReturnsAsync((Usuario?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(
            () => _service.OlvidarContraseñaAsync(dto));
        Assert.Equal("Correo no registrado", ex.Message);
    }

    #endregion

    #region RestablecerContraseñaAsync Tests

    [Fact]
    // Restablecer contraseña exitoso no lanza excepción
    public async Task RestablecerContraseñaAsync_CodigoValido_ActualizaContraseña()
    {
        // Arrange
        var dto = new RestablecerContraseñaDto
        {
            Correo = "test@test.com",
            Codigo = "123456",
            NuevaContraseña = "newpass123"
        };
        var usuario = new Usuario { Id = 1, Correo = dto.Correo, Contraseña = "oldHash" };
        var codigo = new CodigoRecuperacionContraseña
        {
            Codigo = dto.Codigo,
            Correo = dto.Correo,
            EstaUsado = false,
            Activo = true
        };
        _mockRepoCodigo.Setup(r => r.ObtenerCodigoValidoAsync(dto.Correo, dto.Codigo)).ReturnsAsync(codigo);
        _mockRepoUsuario.Setup(r => r.ObtenerPorCorreoAsync(dto.Correo)).ReturnsAsync(usuario);
        _mockHasher.Setup(h => h.HashContraseña(dto.NuevaContraseña)).Returns("newHash");
        _mockRepoUsuario.Setup(r => r.ActualizarAsync(usuario)).Returns(Task.CompletedTask);
        _mockRepoCodigo.Setup(r => r.ActualizarAsync(codigo)).Returns(Task.CompletedTask);

        // Act & Assert - no debe lanzar excepción
        await _service.RestablecerContraseñaAsync(dto);

        // Verify actualización llamada
        _mockRepoUsuario.Verify(r => r.ActualizarAsync(It.IsAny<Usuario>()), Times.Once);
        _mockRepoCodigo.Verify(r => r.ActualizarAsync(It.IsAny<CodigoRecuperacionContraseña>()), Times.Once);
    }

    [Fact]
    // Restablecer con código inválido lanza excepción
    public async Task RestablecerContraseñaAsync_CodigoInvalido_LanzaArgumentException()
    {
        // Arrange
        var dto = new RestablecerContraseñaDto
        {
            Correo = "test@test.com",
            Codigo = "000000",
            NuevaContraseña = "newpass"
        };
        _mockRepoCodigo.Setup(r => r.ObtenerCodigoValidoAsync(dto.Correo, dto.Codigo))
            .ReturnsAsync((CodigoRecuperacionContraseña?)null);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(
            () => _service.RestablecerContraseñaAsync(dto));
        Assert.Equal("Código inválido o expirado", ex.Message);
    }

    #endregion

    #region ObtenerTodosUsuariosAsync Tests

    [Fact]
    // Obtener usuarios como Aprobador retorna lista
    public async Task ObtenerTodosUsuariosAsync_RolAprobador_RetornaLista()
    {
        // Arrange
        var usuarios = new List<Usuario>
        {
            new Usuario { Id = 1, Nombre = "User1", Correo = "u1@test.com", Rol = RolUsuario.Solicitante, FechaCreacion = DateTime.Now },
            new Usuario { Id = 2, Nombre = "User2", Correo = "u2@test.com", Rol = RolUsuario.Aprobador, FechaCreacion = DateTime.Now }
        };
        _mockRepoUsuario.Setup(r => r.ObtenerTodosAsync()).ReturnsAsync(usuarios);

        // Act
        var result = await _service.ObtenerTodosUsuariosAsync("Aprobador");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    // Obtener usuarios como Solicitante lanza excepción
    public async Task ObtenerTodosUsuariosAsync_RolSolicitante_LanzaUnauthorizedException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _service.ObtenerTodosUsuariosAsync("Solicitante"));
    }

    #endregion
}

