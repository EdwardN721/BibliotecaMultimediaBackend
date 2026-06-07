using Microsoft.AspNetCore.Identity;

namespace BibliotecaMultimedia.Domain.Models;

public class User : IdentityUser<Guid>
{
    public string Nombre { get; set; } = string.Empty;
    public string PrimerApellido { get; set; } = string.Empty;
    public string? SegundoApellido { get; set; } = string.Empty;
    
    public virtual ICollection<UserItem> UserItems { get; private set; } = new List<UserItem>();
}