using BibliotecaMultimedia.Application.DTOs.Peticion.Formatos;
using BibliotecaMultimedia.Application.DTOs.Peticion.Plataformas;
using FluentValidation;

namespace BibliotecaMultimedia.Application.Validators;

public class FormatoValidator : AbstractValidator<PeticionCrearPlataformaDto>
{
    public FormatoValidator()
    {
        RuleFor(p => p.Nombre)
            .NotEmpty().WithMessage("El nombre no puede estar vacio.")
            .NotNull().WithMessage("El nombre no puede estar vacio.")
            .MaximumLength(50).WithMessage("El nombre no puede exceder de 50 caracteres.");
    }
}

public class ActualizarFormatoValidator : AbstractValidator<PeticionActualizarFormatoDto>
{
    public ActualizarFormatoValidator()
    {
        RuleFor(p => p.Nombre)
            .MaximumLength(50).WithMessage("El nombre no puede exceder de 50 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.Nombre));
    }
}