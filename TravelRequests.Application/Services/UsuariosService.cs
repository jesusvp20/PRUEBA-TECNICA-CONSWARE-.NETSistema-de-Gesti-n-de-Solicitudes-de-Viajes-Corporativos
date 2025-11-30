using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using TravelRequests.Application.DTOs;
using TravelRequests.Domain.Entities;
using TravelRequests.Domain.Enums;
using TravelRequests.Domain.interfaces;
using TravelRequests.Domain.Interfaces;

namespace TravelRequests.Application.Servicies;

// Servicio de usuarios
public class UsuariosService : IUsuariosService
{
    private readonly IRepositorioUsuario _repositorioUsuario;
    private readonly IRepositorioCodigoRecuperacion _repositorioCodigoRecuperacion;
    private readonly IHasherContraseña _hasherContraseña;
    private readonly ILogger<UsuariosService> _logger;
    private readonly IConfiguration _configuration;

    public UsuariosService(
        IRepositorioUsuario repositorioUsuario,
        IRepositorioCodigoRecuperacion repositorioCodigoRecuperacion,
        IHasherContraseña hasherContraseña,
        ILogger<UsuariosService> logger,
        IConfiguration configuration)
    {
        _repositorioUsuario = repositorioUsuario;
        _repositorioCodigoRecuperacion = repositorioCodigoRecuperacion;
        _hasherContraseña = hasherContraseña;
        _logger = logger;
        _configuration = configuration;
    }

    // Registrar usuario
    public async Task<UsuarioResponseDto> RegistrarUsuarioAsync(RegistrarUsuarioDto registrarDto)
    {
        // Validar correo único
        if (await _repositorioUsuario.ExistePorCorreoAsync(registrarDto.Correo))
            throw new ArgumentException("Correo ya registrado");

        // Validar rol válido (Solicitante/Aprobador)
        if (!Enum.TryParse<RolUsuario>(registrarDto.Rol, true, out var rol))
            throw new ArgumentException("Rol inválido");

        var usuario = new Usuario
        {
            Nombre = registrarDto.Nombre,
            Correo = registrarDto.Correo,
            Contraseña = _hasherContraseña.HashContraseña(registrarDto.Contraseña), // BCrypt hash
            Rol = rol,
            FechaCreacion = DateTime.Now
        };

        await _repositorioUsuario.AgregarAsync(usuario);
        _logger.LogInformation("Usuario registrado: {Correo}", usuario.Correo);

        return MapToDto(usuario);
    }

    // Login - genera JWT
    public async Task<string> LoginAsync(LoginUsuarioDto loginDto)
    {
        var usuario = await _repositorioUsuario.ObtenerPorCorreoAsync(loginDto.Correo);

        // Validar credenciales con BCrypt
        if (usuario == null || !_hasherContraseña.VerificarContraseña(loginDto.Contraseña, usuario.Contraseña))
            throw new UnauthorizedAccessException("Credenciales inválidas");

        // Incluye el rol para autorización
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Correo),
            new Claim(ClaimTypes.Name, usuario.Nombre),
            new Claim(ClaimTypes.Role, usuario.Rol.ToString()) // Rol en el token
        };

        // Configurar firma del token
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = int.TryParse(_configuration["Jwt:ExpiresMinutes"], out var m) ? m : 60;

        // Generar JWT
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(expires),
            signingCredentials: creds
        );

        _logger.LogInformation("Login: {Correo}", usuario.Correo);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // Solicitar código recuperación
    public async Task<string> OlvidarContraseñaAsync(OlvidarContraseñaDto olvidarDto)
    {
        // Validar correo no vacío
        if (string.IsNullOrWhiteSpace(olvidarDto.Correo))
            throw new ArgumentException("Correo requerido");

        var usuario = await _repositorioUsuario.ObtenerPorCorreoAsync(olvidarDto.Correo);
        if (usuario == null)
            throw new ArgumentException("Correo no registrado");

        // Invalidar códigos anteriores
        await _repositorioCodigoRecuperacion.InvalidarCodigosUsuarioAsync(usuario.Id);

        // Generar código de 6 dígitos
        var codigo = GenerarCodigo();
        var codigoRecuperacion = new CodigoRecuperacionContraseña
        {
            Codigo = codigo,
            Correo = usuario.Correo,
            FechaGeneracion = DateTime.Now,
            FechaExpiracion = DateTime.Now.AddMinutes(5), // Expira en 5 min
            EstaUsado = false,
            Activo = true,
            UsuarioId = usuario.Id
        };

        await _repositorioCodigoRecuperacion.AgregarAsync(codigoRecuperacion);
        _logger.LogInformation("Código generado: {Correo}", usuario.Correo);

        return codigo;
    }

    // Restablecer contraseña
    public async Task RestablecerContraseñaAsync(RestablecerContraseñaDto restablecerDto)
    {
        // Validar código activo y no expirado
        var codigoRecuperacion = await _repositorioCodigoRecuperacion.ObtenerCodigoValidoAsync(
            restablecerDto.Correo, restablecerDto.Codigo);

        if (codigoRecuperacion == null)
            throw new ArgumentException("Código inválido o expirado");

        var usuario = await _repositorioUsuario.ObtenerPorCorreoAsync(restablecerDto.Correo)
            ?? throw new ArgumentException("Usuario no encontrado");

        // Actualizar contraseña con BCrypt
        usuario.Contraseña = _hasherContraseña.HashContraseña(restablecerDto.NuevaContraseña);
        usuario.FechaActualizacion = DateTime.Now;

        // Invalidar código usado
        codigoRecuperacion.EstaUsado = true;
        codigoRecuperacion.Activo = false;

        await _repositorioUsuario.ActualizarAsync(usuario);
        await _repositorioCodigoRecuperacion.ActualizarAsync(codigoRecuperacion);

        _logger.LogInformation("Contraseña restablecida: {Correo}", usuario.Correo);
    }

    // Listar usuarios
    public async Task<List<UsuarioResponseDto>> ObtenerTodosUsuariosAsync(string rolUsuarioActual)
    {
        if (rolUsuarioActual != "Aprobador")
            throw new UnauthorizedAccessException("Solo Aprobadores");

        var usuarios = await _repositorioUsuario.ObtenerTodosAsync();
        return usuarios.Select(MapToDto).ToList();
    }

    private UsuarioResponseDto MapToDto(Usuario usuario) => new()
    {
        Id = usuario.Id,
        Nombre = usuario.Nombre,
        Correo = usuario.Correo,
        Rol = usuario.Rol.ToString(),
        FechaCreacion = usuario.FechaCreacion.ToString("yyyy-MM-dd HH:mm")
    };

    private string GenerarCodigo() => new Random().Next(100000, 999999).ToString();
}
