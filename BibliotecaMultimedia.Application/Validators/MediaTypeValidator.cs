using BibliotecaMultimedia.Application.DTOs.Peticion.MediaType;
using FluentValidation;

namespace BibliotecaMultimedia.Application.Validators;

public class MediaTypeValidator : AbstractValidator<PeticionCrearMediaTypeDto>
{
    public MediaTypeValidator()
    {
        RuleFor(m => m.Nombre)
            .NotEmpty().WithMessage("El nombre no puede estar vacio.")
            .NotNull().WithMessage("El nombre no puede estar vacio.")
            .MaximumLength(50).WithMessage("El nombre no puede exceder de 50 caracteres.");
    }
}

public class ActualizarMediaTypeValidator : AbstractValidator<PeticionActualizarMediaTypeDto>
{
    public ActualizarMediaTypeValidator()
    {
        RuleFor(m => m.Nombre)
            .MaximumLength(50).WithMessage("El nombre no puede exceder de 50 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.Nombre));
    }
}