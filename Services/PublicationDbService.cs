using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

public class PublicationDbService : IPublicationService
{
    private readonly DbContext _context;
    private readonly IPhotoService _photoService;
    private readonly IPhotoPublicationService _photoPublicationService;

    public PublicationDbService(DbContext context, IPhotoService photoService, IPhotoPublicationService photoPublicationService)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _photoService = photoService ?? throw new ArgumentNullException(nameof(photoService));
        _photoPublicationService = photoPublicationService ?? throw new ArgumentNullException(nameof(photoPublicationService));
    }

    // Obtener todas las publicaciones
    public IEnumerable<PublicationDTO> GetAll()
    {
        // Primero obtenemos todas las publicaciones como una lista
        var publications = _context.Publications.ToList();

        // Luego transformamos cada publicación en un PublicationDTO
        var publicationDtos = publications.Select(publication =>
        {
            // Obtener las fotos asociadas a la publicación
            var associatedPhotos = _photoPublicationService.GetPhotosByPublicationId(publication.Id);

            return new PublicationDTO
            {
                Id = publication.Id,
                IdUsuario = publication.UserId,
                IdCategoria = publication.IdCategoria,
                Description = publication.Description,
                Price = publication.Price,
                Title = publication.Title,
                IdPublicationState = publication.IdPublicationState,
                IdProductState = publication.IdProductState,
                IdColor = publication.IdColor,
                Stock = publication.Stock,
                Photos = associatedPhotos.Select(p => new PhotoDTO
                {
                    Id = p.Id,
                    ImageData = p.ImageData
                }).ToList()
            };
        }).ToList();

        return publicationDtos;
    }


    // Obtener una publicación por ID
    public PublicationDTO? GetById(int id)
    {
        Publication pub = _context.Publications.Find(id);

        if (pub == null)
        {
            throw new Exception("Publication not found");
        }

        var associatedPhotos = _photoPublicationService.GetPhotosByPublicationId(pub.Id);
        return new PublicationDTO
        {
            Id = pub.Id,
            IdUsuario = pub.UserId,
            IdCategoria = pub.IdCategoria,
            Description = pub.Description,
            Price = pub.Price,
            Title = pub.Title,
            IdPublicationState = pub.IdPublicationState,
            IdProductState = pub.IdProductState,
            IdColor = pub.IdColor,
            Stock = pub.Stock,
            Photos = associatedPhotos.Select(p => new PhotoDTO
            {
                Id = p.Id,
                ImageData = p.ImageData // Suponiendo que Photo tiene esta propiedad
            }).ToList()
        };
    }

    public IEnumerable<PublicationDTO> GetByCategoryName(string categoryName)
    {
        // Buscar el ID de la categoría usando el nombre
        var categoryId = _context.Categories
                            .Where(c => c.Name == categoryName)
                            .Select(c => c.Id)
                            .FirstOrDefault();

        // Si no existe la categoría, retorna una lista vacía
        if (categoryId == null) {
            throw new Exception("Category not found");
        }

        // Obtener todas las publicaciones de esa categoría
        var publications = _context.Publications
                            .Where(p => p.IdCategoria == categoryId)
                            .ToList();

        // Transformar las publicaciones en PublicationDTO con las fotos asociadas
        var publicationDtos = publications.Select(p =>
        {
            var associatedPhotos = _photoPublicationService.GetPhotosByPublicationId(p.Id);

            return new PublicationDTO
            {
                Id = p.Id,
                IdUsuario = p.UserId,
                IdCategoria = p.IdCategoria,
                Price = p.Price,
                Title = p.Title,
                IdPublicationState = p.IdPublicationState,
                IdProductState = p.IdProductState,
                IdColor = p.IdColor,
                Stock = p.Stock,
                Photos = associatedPhotos.Select(photo => new PhotoDTO
                {
                    Id = photo.Id,
                    ImageData = photo.ImageData
                }).ToList()
            };
        }).ToList();

        return publicationDtos;
    }





    // Crear una nueva publicación
    public PublicationDTO Create(int userId, PublicationPostDTO publicationDto)
    {
        if (publicationDto == null)
            throw new ArgumentNullException(nameof(publicationDto));

        if (_photoService == null || _photoPublicationService == null)
            throw new InvalidOperationException("PhotoService or PhotoPublicationService is not properly initialized.");

        Publication publication = new()
        {
            UserId = userId,
            IdCategoria = publicationDto.IdCategoria,
            Description = publicationDto.Description,
            Price = publicationDto.Price,
            Title = publicationDto.Title,
            IdPublicationState = publicationDto.IdPublicationState,
            IdProductState = publicationDto.IdProductState,
            IdColor = publicationDto.IdColor,
            Stock = publicationDto.Stock
        };

        // Agregar la publicación al contexto y guardar cambios
        _context.Publications.Add(publication);
        _context.SaveChanges();

        // Validar que la publicación se guardó correctamente
        if (publication.Id == 0)
        {
            throw new Exception("Failed to save the publication.");
        }

        // Crear y asociar las fotos a la publicación
        List<PhotoDTO> photos = new();
        if (publicationDto.Photos != null && publicationDto.Photos.Any())
        {
            foreach (var photoDto in publicationDto.Photos)
            {
                if (photoDto == null)
                {
                    continue; // Ignorar fotos nulas
                }

                // Crear cada foto en la base de datos
                var createdPhoto = _photoService.Create(photoDto);

                // Asociar la foto con la publicación
                var photoPublicationDto = new PhotoPublicationPostPutDTO
                {
                    IdPublication = publication.Id,
                    IdPhoto = createdPhoto.Id
                };

                _photoPublicationService.Create(photoPublicationDto);

                // Añadir la foto a la lista de fotos de la publicación
                photos.Add(createdPhoto);
            }
        }

        // Obtener todas las fotos asociadas a la publicación recién creada
        var associatedPhotos = _photoPublicationService.GetPhotosByPublicationId(publication.Id);

        // Retornar la publicación creada junto con sus fotos asociadas
        return new PublicationDTO
        {
            Id = publication.Id,
            IdUsuario = publication.UserId,
            IdCategoria = publication.IdCategoria,
            Description = publication.Description,
            Price = publication.Price,
            Title = publication.Title,
            IdPublicationState = publication.IdPublicationState,
            IdProductState = publication.IdProductState,
            IdColor = publication.IdColor,
            Stock = publication.Stock,
            Photos = associatedPhotos.Select(p => new PhotoDTO
            {
                Id = p.Id,
                ImageData = p.ImageData // Suponiendo que Photo tiene esta propiedad
            }).ToList()
        };
    }

    public IEnumerable<PublicationDTO> SearchByTitle(string searchTerm)
    {
        var publications = _context.Publications
            .Where(p => EF.Functions.Like(p.Title, $"%{searchTerm}%"))
            .ToList();

        return publications.Select(p => new PublicationDTO
        {
            Id = p.Id,
            IdUsuario = p.UserId,
            IdCategoria = p.IdCategoria,
            Description = p.Description,
            Price = p.Price,
            Title = p.Title,
            IdPublicationState = p.IdPublicationState,
            IdProductState = p.IdProductState,
            IdColor = p.IdColor,
            Stock = p.Stock,
            Photos = _photoPublicationService.GetPhotosByPublicationId(p.Id).Select(photo => new PhotoDTO
            {
                Id = photo.Id,
                ImageData = photo.ImageData
            }).ToList()
        });
    }

    // Actualizar una publicación
    public PublicationDTO? Update(PublicationPutDTO publicationToUpdate)
    {
        var existingPublication = _context.Publications.Find(publicationToUpdate.Id);
        if (existingPublication == null)
        {
            throw new Exception("Publication not found");
        }

        // Actualizamos los campos que podrían haber cambiado solo si no son nulos
        if (publicationToUpdate.IdCategoria.HasValue)
        {
            existingPublication.IdCategoria = publicationToUpdate.IdCategoria.Value;
        }
        if (!string.IsNullOrEmpty(publicationToUpdate.Description))
        {
            existingPublication.Description = publicationToUpdate.Description;
        }
        if (publicationToUpdate.Price.HasValue)
        {
            existingPublication.Price = publicationToUpdate.Price.Value;
        }
        if (!string.IsNullOrEmpty(publicationToUpdate.Title))
        {
            existingPublication.Title = publicationToUpdate.Title;
        }
        if (publicationToUpdate.IdProductState.HasValue)
        {
            existingPublication.IdProductState = publicationToUpdate.IdProductState.Value;
        }
        if (publicationToUpdate.IdPublicationState.HasValue)
        {
            existingPublication.IdPublicationState = publicationToUpdate.IdPublicationState.Value;
        }
        if (publicationToUpdate.IdColor.HasValue)
        {
            existingPublication.IdColor = publicationToUpdate.IdColor.Value;
        }
        if (publicationToUpdate.Stock.HasValue)
        {
            existingPublication.Stock = publicationToUpdate.Stock.Value;
        }

        // Guardar cambios en la base de datos
        _context.Entry(existingPublication).State = EntityState.Modified;
        _context.SaveChanges();

        // Obtener las fotos asociadas a la publicación
        var associatedPhotos = _photoPublicationService.GetPhotosByPublicationId(publicationToUpdate.Id);

        // Retornar el DTO con las fotos
        return new PublicationDTO
        {
            Id = existingPublication.Id,
            IdUsuario = existingPublication.UserId,
            IdCategoria = existingPublication.IdCategoria,
            Description = existingPublication.Description,
            Price = existingPublication.Price,
            Title = existingPublication.Title,
            IdPublicationState = existingPublication.IdPublicationState,
            IdProductState = existingPublication.IdProductState,
            IdColor = existingPublication.IdColor,
            Stock = existingPublication.Stock,
            Photos = associatedPhotos.Select(photo => new PhotoDTO
            {
                Id = photo.Id,
                ImageData = photo.ImageData
            }).ToList()
        };
    }

    // Eliminar una publicación por ID
    public void Delete(int id)
    {
        var publication = _context.Publications.Find(id);
        if (publication == null)
        {
            throw new Exception("Failed to save the publication.");        
        }
        _context.Publications.Remove(publication);
        _context.SaveChanges();
    }
}
