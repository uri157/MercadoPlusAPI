using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Collections.Generic;

[Route("api/[controller]")]
[ApiController]
public class PublicationVisitedController : ControllerBase
{
    private readonly IPublicationVisitedService _publicationVisitedService;

    public PublicationVisitedController(IPublicationVisitedService publicationVisitedService)
    {
        _publicationVisitedService = publicationVisitedService;
    }

    // Obtener las últimas diez publicaciones visitadas por el usuario autenticado
    [HttpGet("last-ten")]
    public ActionResult<IEnumerable<PublicationVisitedDTO>> GetLastTenVisited()
    {
        int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        var lastTenVisitedPublications = _publicationVisitedService.GetLastTenVisitedPublications(userId);
        return Ok(lastTenVisitedPublications);
    }

    // Registrar una nueva visita a una publicación
    [HttpPost]
    public IActionResult AddVisit(int publicationId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        _publicationVisitedService.AddPublicationVisit(userId, publicationId);
        return Ok("Publication visit registered successfully.");
    }
}
