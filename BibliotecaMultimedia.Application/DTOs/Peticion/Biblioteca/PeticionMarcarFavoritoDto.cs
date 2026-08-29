namespace BibliotecaMultimedia.Application.DTOs.Peticion.Biblioteca;

/// <summary>
/// Marca o desmarca un elemento de la biblioteca como favorito.
/// </summary>
public record PeticionMarcarFavoritoDto
{
    public bool IsFavorite { get; init; }
};
