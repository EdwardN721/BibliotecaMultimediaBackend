using BibliotecaMultimedia.Application.DTOs.Peticion.Creador;
using FluentValidation;

namespace BibliotecaMultimedia.Application.Validators;

public class CreadorValidator : AbstractValidator<PeticionCrearCreadorDto>
{
    public CreadorValidator()
    {
        RuleFor(creator => creator.Nombre)
            .NotEmpty().WithMessage("El nombre no puede estar vacio.")
            .NotNull().WithMessage("El nombre no puede estar vacio.")
            .MaximumLength(255).WithMessage("El nombre no puede exceder de 255 caracteres.");
        
        RuleFor(creator => creator.Biografia)
            .MaximumLength(1500).WithMessage("El nombre no puede exceder de 1500 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.Biografia));
    }
}

public class ActualizarCreadorValidator : AbstractValidator<PeticionActualizarCreadorDto>
{
    public ActualizarCreadorValidator()
    {
        RuleFor(creator => creator.Nombre)
            .NotEmpty().WithMessage("El nombre no puede estar vacio.")
            .NotNull().WithMessage("El nombre no puede estar vacio.")
            .MaximumLength(255).WithMessage("El nombre no puede exceder de 255 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.Nombre));
        
        RuleFor(creator => creator.Biografia)
            .MaximumLength(1500).WithMessage("El nombre no puede exceder de 1500 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.Biografia));
    }
}