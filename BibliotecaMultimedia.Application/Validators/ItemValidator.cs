using BibliotecaMultimedia.Application.DTOs.Peticion.Items;
using FluentValidation;

namespace BibliotecaMultimedia.Application.Validators;

public class ItemValidator : AbstractValidator<PeticionCrearITemDto>
{
    public ItemValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("El título es obligatorio.")
            .MaximumLength(255).WithMessage("El título no puede exceder los 255 caracteres.");

        RuleFor(x => x.Rating)
            .InclusiveBetween((short)0, (short)5).WithMessage("La calificación debe estar entre 0 y 5.")
            .When(x => x.Rating.HasValue);

        // Fks requeridas
        RuleFor(x => x.MediaTypeId).NotEmpty().WithMessage("El tipo de medio es obligatorio.");
        RuleFor(x => x.FormatId).NotEmpty().WithMessage("El formato es obligatorio.");
        RuleFor(x => x.PlatformId)
            .NotEmpty().WithMessage("La plataforma proporcionada no es válida.")
            .When(x => x.PlatformId.HasValue);
        
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
            .InclusiveBetween((short)0, (short)5).WithMessage("La calificación debe estar entre 0 y 5.")
            .When(x => x.Rating.HasValue);

        // Fks requeridas
        RuleFor(x => x.MediaTypeId).NotEmpty().WithMessage("El tipo de medio es obligatorio.");
        RuleFor(x => x.FormatId).NotEmpty().WithMessage("El formato es obligatorio.");
        RuleFor(x => x.PlatformId)
            .NotEmpty().WithMessage("La plataforma proporcionada no es válida.")
            .When(x => x.PlatformId.HasValue);
        
        RuleFor(x => x.GenreIds)
            .NotEmpty().WithMessage("Debe seleccionar al menos un género.");
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("El título es obligatorio.")
            .MaximumLength(255).WithMessage("El título no puede exceder los 255 caracteres.");

        RuleFor(x => x.Rating)
            .InclusiveBetween((short)0, (short)5).WithMessage("La calificación debe estar entre 0 y 5.")
            .When(x => x.Rating.HasValue);

        // Fks requeridas
        RuleFor(x => x.MediaTypeId).NotEmpty().WithMessage("El tipo de medio es obligatorio.");
        RuleFor(x => x.FormatId).NotEmpty().WithMessage("El formato es obligatorio.");
        RuleFor(x => x.PlatformId)
            .NotEmpty().WithMessage("La plataforma proporcionada no es válida.")
            .When(x => x.PlatformId.HasValue);
        
        RuleFor(x => x.GenreIds)
            .NotEmpty().WithMessage("Debe seleccionar al menos un creador.");
    }
}