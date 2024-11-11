// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using System.Collections.Generic;
// using System.Threading.Tasks;

// [Route("api/[controller]")]
// [ApiController]
// public class PhotoPublicationController : ControllerBase
// {
//     private readonly IPhotoPublicationService _photoPublicationService;

//     public PhotoPublicationController(IPhotoPublicationService photoPublicationService)
//     {
//         _photoPublicationService = photoPublicationService;
//     }

//     // GET: api/PhotoPublication
//     [AllowAnonymous]
//     [HttpGet]
//     public IEnumerable<PhotoPublicationDTO> GetPhotoPublications()
//     {
//         return _photoPublicationService.GetAll();
//     }

//     // GET: api/PhotoPublication/{id}
//     [AllowAnonymous]
//     [HttpGet("{id}")]
//     public PhotoPublicationDTO GetPhotoPublication(int id)
//     {
//         return _photoPublicationService.GetById(id);
//     }

//     // POST: api/PhotoPublication
//     [Authorize]
//     [HttpPost]
//     public PhotoPublicationDTO CreatePhotoPublication(PhotoPublicationPostPutDTO createDTO)
//     {
//         return _photoPublicationService.Create(createDTO);
//     }

//     // DELETE: api/PhotoPublication/{id}
//     [Authorize]
//     [HttpDelete("{id}")]
//     public IActionResult DeletePhotoPublication(int id)
//     {
//         _photoPublicationService.Delete(id);
//         return NoContent();
//     }
// }
