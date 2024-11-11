using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/photos")]
public class PhotoController : ControllerBase
{
    private readonly IPhotoService _photoService;

    public PhotoController(IPhotoService photoService)
    {
        _photoService = photoService;
    }

    // // Obtener todas las fotos
    // [Authorize]
    // [HttpGet]
    // public ActionResult<List<PhotoDTO>> GetAllPhotos()
    // {
    //     return Ok(_photoService.GetAll());
    // }

    // Obtener una foto por ID
    [AllowAnonymous]
    [HttpGet("{id}")]
    public PhotoDTO GetById(int id)
    {
        return _photoService.GetById(id);  // Retorna la cadena Base64
    }

    // Subir una nueva foto
    [Authorize]
    [HttpPost]
    public PhotoDTO NewPhoto([FromBody] PhotoPostPutDTO photoDto)
    {
        return _photoService.Create(photoDto);
    }

    // Eliminar una foto por ID
    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    { 
        _photoService.Delete(id);
        return NoContent();
    }
}
