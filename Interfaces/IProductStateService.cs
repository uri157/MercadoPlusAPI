public interface IProductStateService
{
    public IEnumerable<ProductStateDTO> GetAll(); // Obtener todos los estados de producto
    public ProductStateDTO? GetById(int id); // Obtener un estado de producto por su ID
    public ProductStateDTO Create(ProductStatePostDTO productStateDto); // Crear un nuevo estado de producto
    public void Delete(int id); // Eliminar un estado de producto
    public ProductStateDTO? Update(ProductStateDTO productStateDto); // Actualizar un estado de producto existente
}
