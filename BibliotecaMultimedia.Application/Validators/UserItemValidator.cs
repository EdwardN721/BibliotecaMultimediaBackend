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

        RuleForEach(x => x.OwnedFormatIds)
            .NotEmpty().WithMessage("Los formatos propios deben ser identificadores válidos.");
        RuleFor(x => x.OwnedFormatIds)
            .Must(ids => ids.Count == ids.Distinct().Count())
            .WithMessage("Los formatos propios no pueden repetirse.")
            .When(x => x.OwnedFormatIds.Count > 0);

        RuleForEach(x => x.OwnedPlatformIds)
            .NotEmpty().WithMessage("Las plataformas propias deben ser identificadores válidos.");
        RuleFor(x => x.OwnedPlatformIds)
            .Must(ids => ids.Count == ids.Distinct().Count())
            .WithMessage("Las plataformas propias no pueden repetirse.")
            .When(x => x.OwnedPlatformIds.Count > 0);
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

        RuleForEach(x => x.OwnedFormatIds)
            .NotEmpty().WithMessage("Los formatos propios deben ser identificadores válidos.")
            .When(x => x.OwnedFormatIds is not null);
        RuleFor(x => x.OwnedFormatIds!)
            .Must(ids => ids.Count == ids.Distinct().Count())
            .WithMessage("Los formatos propios no pueden repetirse.")
            .When(x => x.OwnedFormatIds is not null && x.OwnedFormatIds.Count > 0);

        RuleForEach(x => x.OwnedPlatformIds)
            .NotEmpty().WithMessage("Las plataformas propias deben ser identificadores válidos.")
            .When(x => x.OwnedPlatformIds is not null);
        RuleFor(x => x.OwnedPlatformIds!)
            .Must(ids => ids.Count == ids.Distinct().Count())
            .WithMessage("Las plataformas propias no pueden repetirse.")
            .When(x => x.OwnedPlatformIds is not null && x.OwnedPlatformIds.Count > 0);
    }
}

public class MarcarFavoritoValidator : AbstractValidator<PeticionMarcarFavoritoDto>
{
    public MarcarFavoritoValidator()
    {
        // Sin reglas por ahora: bool no requiere validación adicional.
    }
}

public class PuntuarValidator : AbstractValidator<PeticionPuntuarDto>
{
    public PuntuarValidator()
    {
        RuleFor(x => x.Rating)
            .InclusiveBetween((short)1, (short)5).WithMessage("La calificación debe estar entre 1 y 5.");
    }
}