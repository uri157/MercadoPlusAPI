using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/publications")]
public class PublicationController : ControllerBase
{
    private readonly IPublicationService _publicationService;

    private readonly IPublicationVisitedService _publicationVisitedService;

    private readonly IPhotoService _photoService;

    private readonly IPhotoPublicationService _photoPublicationService;


    public PublicationController(IPublicationService publicationService, IPublicationVisitedService publicationVisitedService)
    {
        _publicationVisitedService = publicationVisitedService;
        _publicationService = publicationService;

    }

    // Obtener todas las publicaciones
    [AllowAnonymous]
    [HttpGet]
    public ActionResult<IEnumerable<PublicationDTO>> GetAllPublications()
    {
        return Ok(_publicationService.GetAll());
    }

    // Obtener todas las publicaciones, con la opción de filtrar por nombre de categoría
    [AllowAnonymous]
    //[HttpGet]
    [HttpGet("by-category")]
    public IEnumerable<PublicationDTO> GetPublicationsByCategoryName(string categoryName)
    {
        return _publicationService.GetByCategoryName(categoryName);
    }


    // Obtener una publicación por ID
    [AllowAnonymous]
    [HttpGet("{id}")]
    public ActionResult<PublicationDTO> GetById(int id)
    {
        var publication = _publicationService.GetById(id);

        // Verificar si el usuario está autenticado
        if (User.Identity.IsAuthenticated)
        {
            int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            // Registrar la visita a la publicación
            _publicationVisitedService.AddPublicationVisit(userId, id);
        }


        return Ok(publication);
    }

    // Crear una nueva publicación
    [Authorize]
    [HttpPost]
    public ActionResult<PublicationDTO> NewPublication(PublicationPostDTO publicationDto)
    {
        // Obtén el ID del usuario autenticado desde los claims
        int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var newPublication = _publicationService.Create(userId, publicationDto);

    //     // Procesar las fotos y asociarlas a la publicación
    // if (publicationDto.Photos != null && publicationDto.Photos.Any())
    // {
    //     foreach (var photoDto in publicationDto.Photos)
    //     {
    //         // Crear la foto en la base de datos y obtener su ID
    //         var createdPhoto = _photoService.Create(photoDto);

    //         // Crear la relación PhotoPublication entre la foto y la publicación
    //         var photoPublicationDto = new PhotoPublicationPostPutDTO
    //         {
    //             IdPublication = newPublication.Id,
    //             IdPhoto = createdPhoto.Id
    //         };
    //         _photoPublicationService.Create(photoPublicationDto);
    //     }
    // }

        return CreatedAtAction(nameof(GetById), new { id = newPublication.Id }, newPublication);
    }

    // Actualizar una publicación por ID
    [Authorize]
    [HttpPut("{id}")]
    public ActionResult<PublicationDTO> UpdatePublication(PublicationPutDTO publicationToUpdate)
    {
        var updatedPublication = _publicationService.Update(publicationToUpdate);
        return Ok(updatedPublication);
    }

    // Eliminar una publicación por ID
    [Authorize(Roles = "admin")]
    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        _publicationService.Delete(id);
        return NoContent();
    }
}
