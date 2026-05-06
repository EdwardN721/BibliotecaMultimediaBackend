using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BibliotecaMultimedia.Application.Interfaces;
using BibliotecaMultimedia.Application.DTOs.Peticion.Catalogos;
using BibliotecaMultimedia.Application.DTOs.Respuesta.Catalogos;

namespace BibliotecaMultimedia.API.Controllers.V1;

/// <summary>
/// Controlador que administra los generos
/// </summary>
[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[Controller]")]
public class GenresController : ControllerBase
{
    private readonly IGeneroService _generoService;

    public GenresController(IGeneroService generoService)
    {
        _generoService = generoService ?? throw new ArgumentNullException(nameof(generoService));
    }

    /// <summary>
    /// Obtener todos los generos
    /// </summary>
    /// <returns>Lista de generos</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<RespuestaGeneroDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerTodosGeneros()
    {
        IEnumerable<RespuestaGeneroDto> generos = await _generoService.ObtenerTodosAsync();
        return Ok(generos);
    }

    /// <summary>
    /// Obtener un genero por su Id
    /// </summary>
    /// <param name="id">Id del genero a buscar</param>
    /// <returns>Devuelve un genero</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(RespuestaGeneroDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerGeneroPorId([FromRoute] Guid id)
    {
        RespuestaGeneroDto genero = await _generoService.ObtenerPorIdAsync(id);
        return Ok(genero);
    }

    /// <summary>
    /// Agregar un nuevo genero, unicamente Admin puede agregar
    /// </summary>
    /// <param name="generoDto">Informacion para crear el genero.</param>
    /// <returns>Genero creado</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(RespuestaGeneroDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AgregarGenero([FromBody] PeticionCrearGeneroDto generoDto)
    {
        RespuestaGeneroDto generoNuevo = await _generoService.AgregarGeneroAsync(generoDto);
        return CreatedAtAction(nameof(ObtenerGeneroPorId), new { id = generoNuevo.Id }, generoNuevo);
    }

    /// <summary>
    /// Actualizar un genero por su id
    /// </summary>
    /// <param name="id">Id del genero para actualizar</param>
    /// <param name="generoDto">Informacion para actualizar</param>
    /// <returns>Estado de la actualización</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ActualizarGenero([FromRoute] Guid id,
        [FromBody] PeticionActualizarGeneroDto generoDto)
    {
        await _generoService.ActualizarGeneroAsync(id, generoDto);
        return NoContent();
    }

    /// <summary>
    /// Elimina un rol
    /// </summary>
    /// <param name="id">Id del rol a eliminar</param>
    /// <returns>Código de estado de la eliminación</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EliminarGenero([FromRoute] Guid id)
    {
        await _generoService.EliminarGeneroAsync(id);
        return NoContent();
    }
}