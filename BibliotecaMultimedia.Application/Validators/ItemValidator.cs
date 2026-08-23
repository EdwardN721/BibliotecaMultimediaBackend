using BibliotecaMultimedia.Application.DTOs.Peticion.Items;
using FluentValidation;

namespace BibliotecaMultimedia.Application.Validators;

// Creación mínima: solo Title y MediaTypeId son obligatorios.
// Los catálogos asociados (géneros, formatos, plataformas, creadores)
// son opcionales y se completan después desde la edición.
public class ItemValidator : AbstractValidator<PeticionCrearItemDto>
{
    public ItemValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("El título es obligatorio.")
            .MaximumLength(255).WithMessage("El título no puede exceder los 255 caracteres.");

        RuleFor(x => x.MediaTypeId).NotEmpty().WithMessage("El tipo de medio es obligatorio.");

        RuleFor(x => x.IsbnOrUpc)
            .MaximumLength(20).WithMessage("El ISBN/UPC no puede exceder los 20 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.IsbnOrUpc));

        RuleFor(x => x.FormatIds).Must(ids => ids.All(id => id != Guid.Empty))
            .WithMessage("Los formatos proporcionados no son válidos.");
        RuleFor(x => x.PlatformIds).Must(ids => ids.All(id => id != Guid.Empty))
            .WithMessage("Las plataformas proporcionadas no son válidas.");
        RuleFor(x => x.GenreIds).Must(ids => ids.All(id => id != Guid.Empty))
            .WithMessage("Los géneros proporcionados no son válidos.");
        RuleFor(x => x.CreatorIds).Must(ids => ids.All(id => id != Guid.Empty))
            .WithMessage("Los creadores proporcionados no son válidos.");
    }
}

public class ActualizarItemValidator : AbstractValidator<PeticionActualizarItemDto>
{
    public ActualizarItemValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("El título es obligatorio.")
            .MaximumLength(255).WithMessage("El título no puede exceder los 255 caracteres.");

        RuleFor(x => x.MediaTypeId).NotEmpty().WithMessage("El tipo de medio es obligatorio.");

        RuleFor(x => x.IsbnOrUpc)
            .MaximumLength(20).WithMessage("El ISBN/UPC no puede exceder los 20 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.IsbnOrUpc));

        RuleFor(x => x.FormatIds).Must(ids => ids.All(id => id != Guid.Empty))
            .WithMessage("Los formatos proporcionados no son válidos.");
        RuleFor(x => x.PlatformIds).Must(ids => ids.All(id => id != Guid.Empty))
            .WithMessage("Las plataformas proporcionadas no son válidas.");
        RuleFor(x => x.GenreIds).Must(ids => ids.All(id => id != Guid.Empty))
            .WithMessage("Los géneros proporcionados no son válidos.");
        RuleFor(x => x.CreatorIds).Must(ids => ids.All(id => id != Guid.Empty))
            .WithMessage("Los creadores proporcionados no son válidos.");
    }
}
