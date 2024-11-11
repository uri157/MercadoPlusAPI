using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/publication-states")]
public class PublicationStateController : ControllerBase
{
    private readonly IPublicationStateService _publicationStateService;

    public PublicationStateController(IPublicationStateService publicationStateService)
    {
        _publicationStateService = publicationStateService;
    }

    // Obtener todos los estados de publicación
    [AllowAnonymous]
    [HttpGet]
    public ActionResult<IEnumerable<PublicationStateDTO>> GetAllPublicationStates()
    {
        return Ok(_publicationStateService.GetAll());
    }

    // Obtener un estado de publicación por ID
    [AllowAnonymous]
    [HttpGet("{id}")]
    public ActionResult<PublicationStateDTO> GetById(int id)
    {
        return _publicationStateService.GetById(id);
    }

    // Crear un nuevo estado de publicación
    [Authorize(Roles = "admin")]
    [HttpPost]
    public ActionResult<PublicationStateDTO> NewPublicationState(PublicationStatePostDTO publicationStateDto)
    {
        var newPublicationState = _publicationStateService.Create(publicationStateDto);
        return CreatedAtAction(nameof(GetById), new { id = newPublicationState.Id }, newPublicationState);
    }

    // Actualizar un estado de publicación por ID
    [Authorize(Roles = "admin")]
    [HttpPut("{id}")]
    public ActionResult<PublicationStateDTO> UpdatePublicationState(PublicationStateDTO publicationStateDto)
    {

        var updatedPublicationState = _publicationStateService.Update(publicationStateDto);

        return Ok(updatedPublicationState);
    }

    // Eliminar un estado de publicación por ID
    [Authorize(Roles = "admin")]
    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        _publicationStateService.Delete(id);
        return NoContent();
    }
}
