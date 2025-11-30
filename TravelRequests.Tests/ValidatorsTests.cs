using TravelRequests.Application.DTOs;
using TravelRequests.Application.Validators;

namespace TravelRequests.Tests;

// Tests validadores FluentValidation
public class ValidatorsTests
{
    #region RegistrarUsuarioValidator Tests

    [Fact]
    // Registro válido pasa validación
    public void RegistrarUsuarioValidator_DatosValidos_NoTieneErrores()
    {
        // Arrange
        var validator = new RegistrarUsuarioValidator();
        var dto = new RegistrarUsuarioDto
        {
            Nombre = "Test User",
            Correo = "test@test.com",
            Contraseña = "password123",
            Rol = "Solicitante"
        };

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData("", "Nombre requerido")]
    [InlineData("AB", "Nombre debe tener entre 3 y 100 caracteres")]
    // Nombre inválido falla validación
    public void RegistrarUsuarioValidator_NombreInvalido_TieneError(string nombre, string mensajeEsperado)
    {
        // Arrange
        var validator = new RegistrarUsuarioValidator();
        var dto = new RegistrarUsuarioDto { Nombre = nombre, Correo = "t@t.com", Contraseña = "123456", Rol = "Solicitante" };

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == mensajeEsperado);
    }

    [Theory]
    [InlineData("", "Correo requerido")]
    [InlineData("correo-invalido", "Correo inválido")]
    // Correo inválido falla validación
    public void RegistrarUsuarioValidator_CorreoInvalido_TieneError(string correo, string mensajeEsperado)
    {
        // Arrange
        var validator = new RegistrarUsuarioValidator();
        var dto = new RegistrarUsuarioDto { Nombre = "Test", Correo = correo, Contraseña = "123456", Rol = "Solicitante" };

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == mensajeEsperado);
    }

    [Theory]
    [InlineData("", "Contraseña requerida")]
    [InlineData("12345", "Contraseña debe tener al menos 6 caracteres")]
    // Contraseña inválida falla validación
    public void RegistrarUsuarioValidator_ContraseñaInvalida_TieneError(string contraseña, string mensajeEsperado)
    {
        // Arrange
        var validator = new RegistrarUsuarioValidator();
        var dto = new RegistrarUsuarioDto { Nombre = "Test", Correo = "t@t.com", Contraseña = contraseña, Rol = "Solicitante" };

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == mensajeEsperado);
    }

    [Theory]
    [InlineData("", "Rol requerido")]
    [InlineData("Admin", "Rol debe ser Solicitante o Aprobador")]
    [InlineData("InvalidRol", "Rol debe ser Solicitante o Aprobador")]
    // Rol inválido falla validación
    public void RegistrarUsuarioValidator_RolInvalido_TieneError(string rol, string mensajeEsperado)
    {
        // Arrange
        var validator = new RegistrarUsuarioValidator();
        var dto = new RegistrarUsuarioDto { Nombre = "Test", Correo = "t@t.com", Contraseña = "123456", Rol = rol };

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == mensajeEsperado);
    }

    #endregion

    #region LoginUsuarioValidator Tests

    [Fact]
    // Login válido pasa validación
    public void LoginUsuarioValidator_DatosValidos_NoTieneErrores()
    {
        // Arrange
        var validator = new LoginUsuarioValidator();
        var dto = new LoginUsuarioDto { Correo = "test@test.com", Contraseña = "password" };

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    // Login con correo vacío falla
    public void LoginUsuarioValidator_CorreoVacio_TieneError()
    {
        // Arrange
        var validator = new LoginUsuarioValidator();
        var dto = new LoginUsuarioDto { Correo = "", Contraseña = "password" };

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Correo requerido");
    }

    [Fact]
    // Login con contraseña vacía falla
    public void LoginUsuarioValidator_ContraseñaVacia_TieneError()
    {
        // Arrange
        var validator = new LoginUsuarioValidator();
        var dto = new LoginUsuarioDto { Correo = "test@test.com", Contraseña = "" };

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Contraseña requerida");
    }

    #endregion

    #region OlvidarContraseñaValidator Tests

    [Fact]
    // Olvidar contraseña válido pasa validación
    public void OlvidarContraseñaValidator_CorreoValido_NoTieneErrores()
    {
        // Arrange
        var validator = new OlvidarContraseñaValidator();
        var dto = new OlvidarContraseñaDto { Correo = "test@test.com" };

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("", "Correo requerido")]
    [InlineData("correo-invalido", "Correo inválido")]
    // Correo inválido falla
    public void OlvidarContraseñaValidator_CorreoInvalido_TieneError(string correo, string mensajeEsperado)
    {
        // Arrange
        var validator = new OlvidarContraseñaValidator();
        var dto = new OlvidarContraseñaDto { Correo = correo };

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == mensajeEsperado);
    }

    #endregion

    #region RestablecerContraseñaValidator Tests

    [Fact]
    // Restablecer contraseña válido pasa validación
    public void RestablecerContraseñaValidator_DatosValidos_NoTieneErrores()
    {
        // Arrange
        var validator = new RestablecerContraseñaValidator();
        var dto = new RestablecerContraseñaDto
        {
            Correo = "test@test.com",
            Codigo = "123456",
            NuevaContraseña = "newpass123"
        };

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("12345", "Código debe tener 6 dígitos")]
    [InlineData("1234567", "Código debe tener 6 dígitos")]
    [InlineData("", "Código requerido")]
    // Código inválido falla
    public void RestablecerContraseñaValidator_CodigoInvalido_TieneError(string codigo, string mensajeEsperado)
    {
        // Arrange
        var validator = new RestablecerContraseñaValidator();
        var dto = new RestablecerContraseñaDto { Correo = "t@t.com", Codigo = codigo, NuevaContraseña = "123456" };

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == mensajeEsperado);
    }

    #endregion

    #region CrearSolicitudViajeValidator Tests

    [Fact]
    // Crear solicitud válida pasa validación
    public void CrearSolicitudViajeValidator_DatosValidos_NoTieneErrores()
    {
        // Arrange
        var validator = new CrearSolicitudViajeValidator();
        var dto = new CrearSolicitudViajeDto
        {
            CiudadOrigen = "Bogotá",
            CiudadDestino = "Medellín",
            FechaIda = DateTime.Now.AddDays(7),
            FechaRegreso = DateTime.Now.AddDays(14),
            Justificacion = "Reunión de trabajo"
        };

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    // Crear solicitud con misma ciudad origen y destino falla
    public void CrearSolicitudViajeValidator_MismoOrigenDestino_TieneError()
    {
        // Arrange
        var validator = new CrearSolicitudViajeValidator();
        var dto = new CrearSolicitudViajeDto
        {
            CiudadOrigen = "Bogotá",
            CiudadDestino = "Bogotá",
            FechaIda = DateTime.Now.AddDays(7),
            FechaRegreso = DateTime.Now.AddDays(14),
            Justificacion = "Reunión"
        };

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Ciudad destino debe ser diferente a origen");
    }

    [Fact]
    // Crear solicitud con fecha regreso menor a ida falla
    public void CrearSolicitudViajeValidator_FechaRegresoMenorIda_TieneError()
    {
        // Arrange
        var validator = new CrearSolicitudViajeValidator();
        var dto = new CrearSolicitudViajeDto
        {
            CiudadOrigen = "Bogotá",
            CiudadDestino = "Medellín",
            FechaIda = DateTime.Now.AddDays(14),
            FechaRegreso = DateTime.Now.AddDays(7),
            Justificacion = "Reunión"
        };

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Fecha regreso debe ser mayor a fecha ida");
    }

    [Fact]
    // Crear solicitud sin justificación falla
    public void CrearSolicitudViajeValidator_SinJustificacion_TieneError()
    {
        // Arrange
        var validator = new CrearSolicitudViajeValidator();
        var dto = new CrearSolicitudViajeDto
        {
            CiudadOrigen = "Bogotá",
            CiudadDestino = "Medellín",
            FechaIda = DateTime.Now.AddDays(7),
            FechaRegreso = DateTime.Now.AddDays(14),
            Justificacion = ""
        };

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == "Justificación requerida");
    }

    #endregion

    #region CambiarEstadoSolicitudValidator Tests

    [Theory]
    [InlineData("Aprobada")]
    [InlineData("Rechazada")]
    // Estados válidos pasan validación
    public void CambiarEstadoSolicitudValidator_EstadoValido_NoTieneErrores(string estado)
    {
        // Arrange
        var validator = new CambiarEstadoSolicitudValidator();
        var dto = new CambiarEstadoSolicitudDto { Estado = estado };

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("", "Estado requerido")]
    [InlineData("Pendiente", "Estado debe ser Aprobada o Rechazada")]
    [InlineData("InvalidState", "Estado debe ser Aprobada o Rechazada")]
    // Estados inválidos fallan
    public void CambiarEstadoSolicitudValidator_EstadoInvalido_TieneError(string estado, string mensajeEsperado)
    {
        // Arrange
        var validator = new CambiarEstadoSolicitudValidator();
        var dto = new CambiarEstadoSolicitudDto { Estado = estado };

        // Act
        var result = validator.Validate(dto);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage == mensajeEsperado);
    }

    #endregion
}

