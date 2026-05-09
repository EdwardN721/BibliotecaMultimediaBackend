using BibliotecaMultimedia.Application.DTOs.Peticion.Plataformas;
using FluentValidation;

namespace BibliotecaMultimedia.Application.Validators;

public class PlataformaValidator : AbstractValidator<PeticionCrearPlataformaDto>
{
    public PlataformaValidator()
    {
        RuleFor(p => p.Nombre)
            .NotNull().WithMessage("El nombre de la plataforma no es correcto.")
            .NotEmpty().WithMessage("El nombre de la plataforma no es correcto.")
            .MaximumLength(50).WithMessage("El nombre de la plataforma no debe exceder los 50 caracteres.");
    }
}

public class ActualizarPlataformaValidator : AbstractValidator<PeticionActualizarPlataformaDto>
{
    public ActualizarPlataformaValidator()
    {
        RuleFor(p => p.Nombre)
            .NotNull().WithMessage("El nombre de la plataforma no es correcto.")
            .NotEmpty().WithMessage("El nombre de la plataforma no es correcto.")
            .MaximumLength(50).WithMessage("El nombre de la plataforma no debe exceder los 50 caracteres.");
    }
}