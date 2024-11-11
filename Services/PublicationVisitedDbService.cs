using Microsoft.EntityFrameworkCore;

public class PublicationVisitedDbService : IPublicationVisitedService
{
    private readonly DbContext _context; // Cambia YourDbContext por el nombre de tu DbContext


    public PublicationVisitedDbService
    (DbContext context)
    {
        _context = context;
    }

    public void AddPublicationVisit(int userId, int publicationId)
    {
        // Verificar si ya existe la visita para evitar duplicados
        var existingVisit = _context.PublicationVisited
            .FirstOrDefault(pv => pv.IdUser == userId && pv.IdPublication == publicationId);

        if (existingVisit != null)
        {
            // Si la visita ya existe, no la agregamos
            return;
        }

        // Crear una nueva visita
        var visit = new PublicationVisited
        {
            IdUser = userId,
            IdPublication = publicationId
        };

        // Añadir la nueva visita a la base de datos
        _context.PublicationVisited.Add(visit);
        _context.SaveChanges();

        // Eliminar las visitas más antiguas si hay más de 10
        var visitsToRemove = _context.PublicationVisited
            .Where(pv => pv.IdUser == userId)
            .OrderBy(pv => pv.Id) // Asumiendo que el ID es incremental y refleja el orden de inserción
            .Skip(10)  // Saltar las 10 más recientes
            .ToList();

        // Eliminar las visitas más antiguas
        _context.PublicationVisited.RemoveRange(visitsToRemove);
        _context.SaveChanges();
    }

    // Obtener las últimas 10 publicaciones visitadas por un usuario
    public List<PublicationVisitedDTO> GetLastTenVisitedPublications(int userId)
    {
        return _context.PublicationVisited
            .Where(pv => pv.IdUser == userId)
            .OrderByDescending(pv => pv.Id) // Ordenar por ID de manera descendente
            .Take(10) // Limitar a las 10 más recientes
            .Select(pv => new PublicationVisitedDTO
            {
                Id = pv.Id,
                IdUser = pv.IdUser,
                IdPublication = pv.IdPublication
            })
            .ToList();
    }

    public string getLatestCategoryVisitedByUser(int userId)
    {
        var latestVisited = _context.PublicationVisited
                                    .Where(pv => pv.IdUser == userId)
                                    .OrderByDescending(pv => pv.Id) // Ordenar por ID de manera descendente
                                    .FirstOrDefault(); // Obtener solo la última visita

        if (latestVisited == null)
        {
            throw new Exception("Empty History"); // Si no se encuentra ninguna visita, devolver null o un valor por defecto
        }

        // Realizar el JOIN para obtener el nombre de la categoría de la publicación visitada
        var categoryName = _context.Publications
                                .Where(p => p.Id == latestVisited.IdPublication)
                                .Join(_context.Categories, 
                                        p => p.IdCategoria, 
                                        c => c.Id, 
                                        (p, c) => c.Name) // Seleccionar el nombre de la categoría
                                .FirstOrDefault(); // Obtener el nombre de la categoría de la publicación

        return categoryName;
    }
}

