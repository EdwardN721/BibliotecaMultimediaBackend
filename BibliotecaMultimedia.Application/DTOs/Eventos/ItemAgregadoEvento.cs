namespace BibliotecaMultimedia.Application.DTOs.Eventos;

public class ItemAgregadoEvento
{
    public Guid ItemId { get; set; }
    public string Title { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public DateTimeOffset CreadoEn { get; set; }
}