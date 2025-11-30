using FluentValidation;
using TravelRequests.Application.DTOs;

namespace TravelRequests.Application.Validators;

// Validador para crear solicitud de viaje
public class CrearSolicitudViajeValidator : AbstractValidator<CrearSolicitudViajeDto>
{
    public CrearSolicitudViajeValidator()
    {
        // Ciudad origen requerida
        RuleFor(x => x.CiudadOrigen)
            .NotEmpty().WithMessage("Ciudad origen requerida")
            .MaximumLength(100).WithMessage("Ciudad origen máximo 100 caracteres");

        // Ciudad destino requerida y diferente a origen
        RuleFor(x => x.CiudadDestino)
            .NotEmpty().WithMessage("Ciudad destino requerida")
            .MaximumLength(100).WithMessage("Ciudad destino máximo 100 caracteres")
            .NotEqual(x => x.CiudadOrigen).WithMessage("Ciudad destino debe ser diferente a origen");

        // Fecha ida requerida
        RuleFor(x => x.FechaIda)
            .NotEmpty().WithMessage("Fecha ida requerida");

        // Fecha regreso mayor a fecha ida
        RuleFor(x => x.FechaRegreso)
            .NotEmpty().WithMessage("Fecha regreso requerida")
            .GreaterThan(x => x.FechaIda).WithMessage("Fecha regreso debe ser mayor a fecha ida");

        // Justificación requerida
        RuleFor(x => x.Justificacion)
            .NotEmpty().WithMessage("Justificación requerida")
            .MaximumLength(500).WithMessage("Justificación máximo 500 caracteres");
    }
}

// Validador para cambiar estado
public class CambiarEstadoSolicitudValidator : AbstractValidator<CambiarEstadoSolicitudDto>
{
    public CambiarEstadoSolicitudValidator()
    {
        // Estado debe ser Aprobada o Rechazada
        RuleFor(x => x.Estado)
            .NotEmpty().WithMessage("Estado requerido")
            .Must(e => e == "Aprobada" || e == "Rechazada")
            .WithMessage("Estado debe ser Aprobada o Rechazada");
    }
}

