using BibliotecaMultimedia.Application.DTOs.Peticion.Catalogos;
using FluentValidation;

namespace BibliotecaMultimedia.Application.Validators;

public class CatalogoValidator : AbstractValidator<PeticionCrearGeneroDto>
{
    public CatalogoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MinimumLength(3).WithMessage("El nombre es muy corto.")
            .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.");

        RuleFor(x => x.Description)
            .MaximumLength(150).WithMessage("La descripción no puede exceder los 150 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}

public class ActualizarGeneroDtoValidator : AbstractValidator<PeticionActualizarGeneroDto>
{
    public ActualizarGeneroDtoValidator()
    {
        RuleFor(x => x.Name)
            .MinimumLength(3).WithMessage("El nombre es muy corto.")
            .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.Description)
            .MaximumLength(150).WithMessage("La descripción no puede exceder los 150 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}