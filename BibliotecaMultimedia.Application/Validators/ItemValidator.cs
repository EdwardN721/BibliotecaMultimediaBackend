using BibliotecaMultimedia.Application.DTOs.Peticion.Items;
using FluentValidation;

namespace BibliotecaMultimedia.Application.Validators;

public class ItemValidator : AbstractValidator<PeticionCrearItemDto>
{
    public ItemValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("El título es obligatorio.")
            .MaximumLength(255).WithMessage("El título no puede exceder los 255 caracteres.");

        RuleFor(x => x.Rating)
            .InclusiveBetween((short)1, (short)5).WithMessage("La calificación debe estar entre 1 y 5.")
            .When(x => x.Rating.HasValue);

        // Fks requeridas
        RuleFor(x => x.MediaTypeId).NotEmpty().WithMessage("El tipo de medio es obligatorio.");
        RuleFor(x => x.FormatIds)
            .NotEmpty().WithMessage("Debe seleccionar al menos un formato.")
            .Must(ids => ids.All(id => id != Guid.Empty)).WithMessage("Los formatos proporcionados no son válidos.");
        RuleFor(x => x.PlatformIds)
            .Must(ids => ids.All(id => id != Guid.Empty)).WithMessage("Las plataformas proporcionadas no son válidas.");

        RuleFor(x => x.GenreIds)
            .NotEmpty().WithMessage("Debe seleccionar al menos un género.");
        RuleFor(x => x.CreatorIds)
            .NotEmpty().WithMessage("Debe seleccionar al menos un creador.");
    }
}

public class ActualizarItemValidator : AbstractValidator<PeticionActualizarItemDto>
{
    public ActualizarItemValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("El título es obligatorio.")
            .MaximumLength(255).WithMessage("El título no puede exceder los 255 caracteres.");

        RuleFor(x => x.Rating)
            .InclusiveBetween((short)1, (short)5).WithMessage("La calificación debe estar entre 1 y 5.")
            .When(x => x.Rating.HasValue);

        // Fks requeridas
        RuleFor(x => x.MediaTypeId).NotEmpty().WithMessage("El tipo de medio es obligatorio.");
        RuleFor(x => x.FormatIds)
            .NotEmpty().WithMessage("Debe seleccionar al menos un formato.")
            .Must(ids => ids.All(id => id != Guid.Empty)).WithMessage("Los formatos proporcionados no son válidos.");
        RuleFor(x => x.PlatformIds)
            .Must(ids => ids.All(id => id != Guid.Empty)).WithMessage("Las plataformas proporcionadas no son válidas.");

        RuleFor(x => x.GenreIds)
            .NotEmpty().WithMessage("Debe seleccionar al menos un género.");
        RuleFor(x => x.CreatorIds)
            .NotEmpty().WithMessage("Debe seleccionar al menos un creador.");
    }
}
