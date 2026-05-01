using BibliotecaMultimedia.Application.DTOs.Peticion.Usuarios;
using FluentValidation;

namespace BibliotecaMultimedia.Application.Validators;

public class UsuarioValidator : AbstractValidator<PeticionCrearUsuarioDto>
{
    public UsuarioValidator()
    {
        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("El correo electrónico es obligatorio.")
            .EmailAddress().WithMessage("El formato del correo no es válido.");
        
        RuleFor(u => u.Password)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.")
            .Matches(@"[A-Z]+").WithMessage("La contraseña debe contener al menos una letra mayúscula.")
            .Matches(@"[0-9]+").WithMessage("La contraseña debe contener al menos un número.");
            
        RuleFor(u => u.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");
        
        RuleFor(x => x.PrimerApellido)
            .NotEmpty().WithMessage("El primer apellido es obligatorio.")
            .MaximumLength(100).WithMessage("El primer apellido no puede exceder los 100 caracteres.");
        
        RuleFor(x => x.SegundoApellido)
            .MaximumLength(100).WithMessage("El segundo apellido no puede exceder los 100 caracteres.")
            .When(x => !string.IsNullOrEmpty(x.SegundoApellido));
        
        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\d{10}$").WithMessage("El número de teléfono debe tener exactamente 10 dígitos.")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber)); // Solo lo validamos si lo envían
    }
}

public class UpdateUserProfileDtoValidator : AbstractValidator<PeticionActualizarUsuarioDto>
{
    public UpdateUserProfileDtoValidator()
    {
        // Solo validamos el Nombre si se envió en la petición
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre no puede quedar vacío si se intenta actualizar.")
            .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.")
            .When(x => x.Nombre != null);

        RuleFor(x => x.PrimerApellido)
            .NotEmpty().WithMessage("El primer apellido no puede quedar vacío si se intenta actualizar.")
            .MaximumLength(100).WithMessage("El primer apellido no puede exceder los 100 caracteres.")
            .When(x => x.PrimerApellido != null);

        RuleFor(x => x.SegundoApellido)
            .MaximumLength(100).WithMessage("El segundo apellido no puede exceder los 100 caracteres.")
            .When(x => x.SegundoApellido != null);
        
        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\d{10}$").WithMessage("El número de teléfono debe tener exactamente 10 dígitos.")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber)); // Solo lo validamos si lo envían
    }
}