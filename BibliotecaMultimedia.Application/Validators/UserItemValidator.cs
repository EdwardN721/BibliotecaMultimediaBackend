using BibliotecaMultimedia.Application.DTOs.Peticion.Biblioteca;
using FluentValidation;

namespace BibliotecaMultimedia.Application.Validators;

public class AgregarABibliotecaValidator : AbstractValidator<PeticionAgregarABibliotecaDto>
{
    public AgregarABibliotecaValidator()
    {
        RuleFor(x => x.ItemId)
            .NotEmpty().WithMessage("El ítem es obligatorio.");

        RuleFor(x => x.PersonalRating)
            .InclusiveBetween((short)1, (short)5).WithMessage("La calificación debe estar entre 1 y 5.")
            .When(x => x.PersonalRating.HasValue);

        RuleFor(x => x.Progress)
            .MaximumLength(500).WithMessage("El progreso no puede exceder los 500 caracteres.")
            .When(x => x.Progress is not null);

        RuleFor(x => x.Review)
            .MaximumLength(2000).WithMessage("La reseña no puede exceder los 2000 caracteres.")
            .When(x => x.Review is not null);
    }
}

public class ActualizarUserItemValidator : AbstractValidator<PeticionActualizarUserItemDto>
{
    public ActualizarUserItemValidator()
    {
        RuleFor(x => x.PersonalRating)
            .InclusiveBetween((short)1, (short)5).WithMessage("La calificación debe estar entre 1 y 5.")
            .When(x => x.PersonalRating.HasValue);

        RuleFor(x => x.Progress)
            .MaximumLength(500).WithMessage("El progreso no puede exceder los 500 caracteres.")
            .When(x => x.Progress is not null);

        RuleFor(x => x.Review)
            .MaximumLength(2000).WithMessage("La reseña no puede exceder los 2000 caracteres.")
            .When(x => x.Review is not null);
    }
}