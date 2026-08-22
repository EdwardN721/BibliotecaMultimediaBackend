using BibliotecaMultimedia.Application.DTOs.Peticion.Biblioteca;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Biblioteca;
using BibliotecaMultimedia.Domain.Models;

namespace BibliotecaMultimedia.Application.Mappers;

public static class UserItemMapper
{
    public static UserItem MapToEntity(this PeticionAgregarABibliotecaDto dto, Guid userId)
    {
        return new UserItem
        {
            UserId = userId,
            ItemId = dto.ItemId,
            Status = dto.Status,
            Progress = dto.Progress,
            IsFavorite = dto.IsFavorite,
            PersonalRating = dto.PersonalRating,
            Review = dto.Review,
            IsPrivate = dto.IsPrivate,
            DateAdded = DateTimeOffset.UtcNow,
            StartedAt = dto.StartedAt,
            FinishedAt = dto.FinishedAt,
        };
    }

    public static void UpdateEntity(this UserItem userItem, PeticionActualizarUserItemDto dto)
    {
        if (dto.Status.HasValue) userItem.Status = dto.Status.Value;
        if (dto.Progress is not null) userItem.Progress = dto.Progress;
        if (dto.IsFavorite.HasValue) userItem.IsFavorite = dto.IsFavorite.Value;
        if (dto.PersonalRating.HasValue) userItem.PersonalRating = dto.PersonalRating.Value;
        if (dto.Review is not null) userItem.Review = dto.Review;
        if (dto.IsPrivate.HasValue) userItem.IsPrivate = dto.IsPrivate.Value;
        if (dto.StartedAt.HasValue) userItem.StartedAt = dto.StartedAt.Value;
        if (dto.FinishedAt.HasValue) userItem.FinishedAt = dto.FinishedAt.Value;
    }

    public static RespuestaUserItemDto MapToDto(this UserItem userItem)
    {
        return new RespuestaUserItemDto
        {
            Id = userItem.Id,
            ItemId = userItem.ItemId,
            Titulo = userItem.Item?.Title ?? string.Empty,
            MediaType = userItem.Item?.MediaType?.Name ?? string.Empty,
            Format = userItem.Item?.Format?.Name ?? string.Empty,
            Platform = userItem.Item?.Platform?.Name,
            Genres = userItem.Item?.ItemGenres
                ?.Select(ig => ig.Genre?.Name ?? string.Empty)
                .Where(n => !string.IsNullOrEmpty(n))
                .ToList() ?? new List<string>(),
            Creators = userItem.Item?.ItemCreators
                ?.Select(ic => ic.Creator?.Name ?? string.Empty)
                .Where(n => !string.IsNullOrEmpty(n))
                .ToList() ?? new List<string>(),
            ImageUrl = userItem.Item?.ItemImages
                ?.OrderByDescending(i => i.IsPrimary)
                .Select(i => i.ImageUrl)
                .FirstOrDefault(),
            Status = userItem.Status,
            Progress = userItem.Progress,
            IsFavorite = userItem.IsFavorite,
            PersonalRating = userItem.PersonalRating,
            Review = userItem.Review,
            IsPrivate = userItem.IsPrivate,
            DateAdded = userItem.DateAdded,
            StartedAt = userItem.StartedAt,
            FinishedAt = userItem.FinishedAt,
            CreatedAt = userItem.CreatedAt,
            UpdatedAt = userItem.UpdatedAt,
        };
    }

    public static IEnumerable<RespuestaUserItemDto> MapToDto(this IEnumerable<UserItem>? items)
    {
        return items is null
            ? Enumerable.Empty<RespuestaUserItemDto>()
            : items.Select(item => item.MapToDto());
    }
}