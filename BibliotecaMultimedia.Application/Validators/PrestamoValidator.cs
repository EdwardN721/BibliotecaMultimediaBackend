using BibliotecaMultimedia.Application.DTOs.Peticion.Biblioteca;
using FluentValidation;

namespace BibliotecaMultimedia.Application.Validators;

public class CrearPrestamoValidator : AbstractValidator<PeticionCrearPrestamoDto>
{
    public CrearPrestamoValidator()
    {
        RuleFor(x => x.NombrePersona)
            .NotEmpty().WithMessage("El nombre de la persona es obligatorio.")
            .MaximumLength(120).WithMessage("El nombre no puede exceder los 120 caracteres.");

        RuleFor(x => x.Notas)
            .MaximumLength(500).WithMessage("Las notas no pueden exceder los 500 caracteres.")
            .When(x => x.Notas is not null);
    }
}

public class ActualizarPrestamoValidator : AbstractValidator<PeticionActualizarPrestamoDto>
{
    public ActualizarPrestamoValidator()
    {
        RuleFor(x => x.NombrePersona)
            .NotEmpty().WithMessage("El nombre de la persona es obligatorio.")
            .MaximumLength(120).WithMessage("El nombre no puede exceder los 120 caracteres.")
            .When(x => x.NombrePersona is not null);

        RuleFor(x => x.Notas)
            .MaximumLength(500).WithMessage("Las notas no pueden exceder los 500 caracteres.")
            .When(x => x.Notas is not null);
    }
}
