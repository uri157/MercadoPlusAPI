using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

public class PhotoDbService : IPhotoService
{
    private readonly DbContext _context;
    private readonly IWebHostEnvironment _environment;

    public PhotoDbService(DbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    // // Obtener todas las fotos
    // public List<PhotoDTO> GetAll()
    // {
    //     var photos = _context.Photos.ToList();
    //     return photos.Select(p => new PhotoDTO
    //     {
    //         Id = p.Id,
    //         ImageData = p.ImageData
    //     }).ToList();
    // }

    // Obtener una foto por ID
    public PhotoDTO GetById(int id)
    {
        var photo = _context.Photos.Find(id);
        if (photo == null)
        {
            throw new Exception("Photo not found");
        }

        return new PhotoDTO
        {
            Id = photo.Id,
            ImageData = photo.ImageData
        };
    }

    // Crear una nueva foto
    public PhotoDTO Create(PhotoPostPutDTO photoDto)
    {
        if (string.IsNullOrEmpty(photoDto.ImageData))
        {
            throw new Exception("Invalid Image Data");
        }
        var photo = new Photo
        {
            ImageData = photoDto.ImageData
        };
        _context.Photos.Add(photo);
        _context.SaveChanges();

        return new PhotoDTO
        {
            Id = photo.Id,
            ImageData = photo.ImageData
        };
    }

    // Eliminar una foto
    public void Delete(int id)
    {
        var photo = _context.Photos.Find(id);
        if (photo == null)
        {
            throw new Exception("Photo Not Found");
        }

        // Eliminar el registro de la base de datos
        _context.Photos.Remove(photo);
        _context.SaveChanges();
    }

    // Actualizar una foto
    public PhotoDTO Update(int id, PhotoDTO photoDto)
    {
        var photo = _context.Photos.Find(id);
        if (photo == null)
        {
            throw new Exception("Photo Not Found");
        }

        photo.ImageData = photoDto.ImageData;

        _context.SaveChanges();

        return new PhotoDTO
        {
            Id = photo.Id,
            ImageData = photo.ImageData
        };
    }
}
