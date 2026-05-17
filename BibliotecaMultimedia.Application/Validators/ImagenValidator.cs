using BibliotecaMultimedia.Application.DTOs.Peticion.Images;
using FluentValidation;

namespace BibliotecaMultimedia.Application.Validators;

public class ImagenValidator : AbstractValidator<PeticionAgregarImagenDto>
{
    public ImagenValidator()
    {
        RuleFor(x => x.ImageUrl)
            .NotEmpty().WithMessage("La URL de la Imagen no es correcta.");

        RuleFor(x => x.ItemId)
            .NotEmpty().WithMessage("El Item no es correcto.");
    }
}

public class ActualizarImagenValidator : AbstractValidator<PeticionActualizarImagenDto>
{
    public ActualizarImagenValidator()
    {
        RuleFor(x => x.ImageUrl)
            .NotEmpty().WithMessage("La URL de la Imagen no es correcta.");

        RuleFor(x => x.ItemId)
            .NotEmpty().WithMessage("El Item no es correcto.");
    }
}