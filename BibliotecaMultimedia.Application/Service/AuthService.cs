using System.Text;
using FluentValidation;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using BibliotecaMultimedia.Domain.Models;
using BibliotecaMultimedia.Application.Mappers;
using BibliotecaMultimedia.Application.Interfaces;
using BibliotecaMultimedia.Application.Exceptions;
using BibliotecaMultimedia.Application.DTOs.Peticion.Usuarios;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Usuarios;

namespace BibliotecaMultimedia.Application.Service;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly IValidator<PeticionCrearUsuarioDto> _validator;

    public AuthService(UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager, IConfiguration configuration, IValidator<PeticionCrearUsuarioDto> validator)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _validator = validator;
    }

    public async Task<AuthResponseDto> RegisterAsync(PeticionCrearUsuarioDto peticion)
    {
        var validationResult = await _validator.ValidateAsync(peticion);
        if (!validationResult.IsValid)
        {
            throw new ValidationAppException(validationResult.Errors);
        }

        User usuario = peticion.ToEntity();
        
        // Crear al usuario en Identity
        IdentityResult result = await _userManager.CreateAsync(usuario, peticion.Password);
        if (!result.Succeeded)
        {
            throw new IdentityUserException(result.Errors);
        }
        
        // Asignar Rol
        const string defaultRole = "User";
        if (!await _roleManager.RoleExistsAsync(defaultRole))
        {
            await _roleManager.CreateAsync(new IdentityRole<Guid>(defaultRole));
        }
        await _userManager.AddToRoleAsync(usuario, defaultRole);
        
        // Generar Token y devolcer
        string token = await GenerateTokenAsync(usuario);

        return new AuthResponseDto
        {
            Token = token,
            User = usuario.MapToDto()
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto peticion)
    {
        User? usuario = await _userManager.FindByEmailAsync(peticion.Email);
        if (usuario == null || !await _userManager.CheckPasswordAsync(usuario, peticion.Password))
        {
            throw new UnauthorizedAppException("Credenciales inválidas."); 
        }

        string token = await GenerateTokenAsync(usuario);

        return new AuthResponseDto
        {
            Token = token,
            User = usuario.MapToDto()
        };
    }

    #region MetodosPrivados

    private async Task<string> GenerateTokenAsync(User user)
    {
        // Claims básicos del usuario -> datos verificados que forman parte del perfil del usuario
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Name, user.Nombre)
        };
        
        // Extraer roles del usuario e inyectarlos como Claims
        IList<string> roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        
        // Configuración de la llave y expiración
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["Jwt:ExpireDays"])),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = creds
        };
        
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        
        return tokenHandler.WriteToken(token);
    }

    #endregion
}