using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
    public ActionResult<IEnumerable<WishedArticleDTO>> GetAllWishedArticles()
    {
        var wishedArticles = _wishedArticleService.GetAll();
        return Ok(wishedArticles);
    }

    // Obtener un artículo deseado por ID
    [Authorize]
    [HttpGet("{id}")]
    public ActionResult<WishedArticleDTO> GetById(int id)
    {
        var wishedArticle = _wishedArticleService.GetById(id);
        return Ok(wishedArticle);
    }

    // Agregar un nuevo artículo a la lista de deseos
    [Authorize]
    [HttpPost]
    public ActionResult<WishedArticleDTO> NewWishedArticle(WishedArticlePostPutDTO wishedArticleDto)
    {

        int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        var newWishedArticle = _wishedArticleService.Create(userId, wishedArticleDto); // Pasamos directamente el DTO al servicio
        
        return CreatedAtAction(nameof(GetById), new { id = newWishedArticle.Id }, newWishedArticle);
    }

    // Actualizar un artículo deseado existente
    [Authorize]
    [HttpPut("{id}")]
    public ActionResult<WishedArticleDTO> UpdateWishedArticle(int id, WishedArticlePostPutDTO wishedArticleToUpdate)
    {
        int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var updatedWishedArticle = _wishedArticleService.Update(id, userId, wishedArticleToUpdate);

        return Ok(updatedWishedArticle);
    }

    // Eliminar un artículo deseado por ID
    [Authorize]
    [HttpDelete("{id}")]
    public ActionResult DeleteWishedArticle(int id)
    {
        var wishedArticle = _wishedArticleService.GetById(id);

        _wishedArticleService.Delete(id);
        return NoContent();
    }
}
