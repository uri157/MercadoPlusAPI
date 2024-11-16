using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

[ApiController]
[Route("api/wished-articles")]
public class WishedArticleController : ControllerBase
{
    private readonly IWishedArticleService _wishedArticleService;

    public WishedArticleController(IWishedArticleService wishedArticleService)
    {
        _wishedArticleService = wishedArticleService;
    }

    // Obtener todos los artículos deseados
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WishedArticleDTO>>> GetAllWishedArticles()
        {
        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
        {
            return Unauthorized("ID de usuario inválido.");
        }

        var wishedArticles = await _wishedArticleService.GetAllByUserIdAsync(userId);
        return Ok(wishedArticles);
    }

    // Obtener un artículo deseado por ID
    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<WishedArticleDTO>> GetById(int id)
    {
        var wishedArticle = await _wishedArticleService.GetByIdAsync(id);
        if (wishedArticle == null)
        {
            return NotFound("Artículo deseado no encontrado.");
        }
        return Ok(wishedArticle);
    }

    // Agregar un nuevo artículo a la lista de deseos
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<WishedArticleDTO>> NewWishedArticle([FromBody] WishedArticlePostPutDTO wishedArticleDto)
    {
        if (wishedArticleDto == null)
        {
            return BadRequest("Datos del artículo deseado inválidos.");
        }

        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
        {
            return Unauthorized("ID de usuario inválido.");
        }

        try
        {
            var newWishedArticle = await _wishedArticleService.CreateAsync(userId, wishedArticleDto);
            return CreatedAtAction(nameof(GetById), new { id = newWishedArticle.Id }, newWishedArticle);
        }
        catch (Exception ex)
        {
            // Manejo genérico de excepciones
            return StatusCode(500, $"Ocurrió un error: {ex.Message}");
        }
    }

    // Actualizar un artículo deseado existente
    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<WishedArticleDTO>> UpdateWishedArticle(int id, [FromBody] WishedArticlePostPutDTO wishedArticleToUpdate)
    {
        if (wishedArticleToUpdate == null)
        {
            return BadRequest("Datos del artículo deseado inválidos.");
        }

        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
        {
            return Unauthorized("ID de usuario inválido.");
        }

        var updatedWishedArticle = await _wishedArticleService.UpdateAsync(id, userId, wishedArticleToUpdate);
        if (updatedWishedArticle == null)
        {
            return NotFound("Artículo deseado no encontrado para actualizar.");
        }

        return Ok(updatedWishedArticle);
    }

    // Eliminar un artículo deseado por ID
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteWishedArticle(int id)
    {
        var wishedArticle = await _wishedArticleService.GetByIdAsync(id);
        if (wishedArticle == null)
        {
            return NotFound("Artículo deseado no encontrado para eliminar.");
        }

        await _wishedArticleService.DeleteAsync(id);
        return NoContent();
    }
}
