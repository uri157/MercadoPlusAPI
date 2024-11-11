using Microsoft.EntityFrameworkCore;

public class CategoryDbService : ICategoryService
{
    private readonly DbContext _context;

    public CategoryDbService(DbContext context)
    {
        _context = context;
    }

    // Crear una nueva categoría
    public Category Create(CategoryPostDTO categoryDto)
    {
        Category category = new()
        {
            Name = categoryDto.Name
        };

        _context.Categories.Add(category);
        _context.SaveChanges(); // Guardar cambios en la base de datos
        return category;
    }

    // Eliminar una categoría por su ID
    public void Delete(int id)
    {
        var category = _context.Categories.Find(id);

        if (category == null)
        {
            throw new Exception($"Category with ID '{id}' not found");
        }
       
        _context.Categories.Remove(category);
        _context.SaveChanges(); // Guardar los cambios
    }

    // Obtener todas las categorías
    public IEnumerable<CategoryDTO> GetAll()
    {
        return _context.Categories
                   .Select(c => new CategoryDTO
                   {
                       Id = c.Id,
                       Name = c.Name
                   })
                   .ToList(); // Convertir a lista
    }

    // Obtener una categoría por su ID
    public CategoryDTO? GetById(int id)
    {
        var category = _context.Categories.Find(id);
    
        // Verificar si la categoría fue encontrada
        if (category == null)
        {
            throw new Exception($"Category with ID '{id}' not found");
        }

        // Mapear la entidad Category al DTO
        return new CategoryDTO
        {
            Id = category.Id,
            Name = category.Name
        };
    }

    // Actualizar una categoría existente
    public CategoryDTO? Update(CategoryDTO categoryDto)
    {

        var existingCategory = _context.Categories.Find(categoryDto.Id);

        if (existingCategory != null)
        {
            existingCategory.Name = categoryDto.Name;

            _context.Entry(existingCategory).State = EntityState.Modified;
            _context.SaveChanges(); // Guardar cambios
            return new CategoryDTO
            {
                Id = existingCategory.Id,
                Name = existingCategory.Name
            };
        }

        return null;
    }
}
