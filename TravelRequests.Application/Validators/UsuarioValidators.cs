using FluentValidation;
using TravelRequests.Application.DTOs;

namespace TravelRequests.Application.Validators;

// Validador para registro de usuario
public class RegistrarUsuarioValidator : AbstractValidator<RegistrarUsuarioDto>
{
    public RegistrarUsuarioValidator()
    {
        // Nombre requerido, 3-100 caracteres
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("Nombre requerido")
            .Length(3, 100).WithMessage("Nombre debe tener entre 3 y 100 caracteres");

        // Correo requerido y formato válido
        RuleFor(x => x.Correo)
            .NotEmpty().WithMessage("Correo requerido")
            .EmailAddress().WithMessage("Correo inválido");

        // Contraseña mínimo 6 caracteres
        RuleFor(x => x.Contraseña)
            .NotEmpty().WithMessage("Contraseña requerida")
            .MinimumLength(6).WithMessage("Contraseña debe tener al menos 6 caracteres");

        // Rol debe ser Solicitante o Aprobador
        RuleFor(x => x.Rol)
            .NotEmpty().WithMessage("Rol requerido")
            .Must(r => r == "Solicitante" || r == "Aprobador")
            .WithMessage("Rol debe ser Solicitante o Aprobador");
    }
}

// Validador para login
public class LoginUsuarioValidator : AbstractValidator<LoginUsuarioDto>
{
    public LoginUsuarioValidator()
    {
        RuleFor(x => x.Correo)
            .NotEmpty().WithMessage("Correo requerido")
            .EmailAddress().WithMessage("Correo inválido");

        RuleFor(x => x.Contraseña)
            .NotEmpty().WithMessage("Contraseña requerida");
    }
}

// Validador para solicitar código de recuperación
public class OlvidarContraseñaValidator : AbstractValidator<OlvidarContraseñaDto>
{
    public OlvidarContraseñaValidator()
    {
        RuleFor(x => x.Correo)
            .NotEmpty().WithMessage("Correo requerido")
            .EmailAddress().WithMessage("Correo inválido");
    }
}

// Validador para restablecer contraseña
public class RestablecerContraseñaValidator : AbstractValidator<RestablecerContraseñaDto>
{
    public RestablecerContraseñaValidator()
    {
        RuleFor(x => x.Correo)
            .NotEmpty().WithMessage("Correo requerido")
            .EmailAddress().WithMessage("Correo inválido");

        // Código de 6 dígitos
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("Código requerido")
            .Length(6).WithMessage("Código debe tener 6 dígitos");

        RuleFor(x => x.NuevaContraseña)
            .NotEmpty().WithMessage("Nueva contraseña requerida")
            .MinimumLength(6).WithMessage("Contraseña debe tener al menos 6 caracteres");
    }
}

