using System.Collections.Generic;
using System.Threading.Tasks;

public interface IShoppingCartItemService
{
    // Crear un nuevo item en el carrito del usuario
    Task<ShoppingCartItemDTO> CreateAsync(ShoppingCartItemPostDTO shoppingCartItemDto);

    // Eliminar un item del carrito del usuario por su ID
    Task DeleteAsync(int userId, int publicationId);

    // Obtener todos los items del carrito del usuario
    Task<IEnumerable<ShoppingCartItemDTO>> GetAllAsync(int userId);

    // Obtener un item del carrito del usuario por su ID
    Task<ShoppingCartItemDTO?> GetByIdAsync(int userId, int publicationId);

    // Actualizar un item existente en el carrito del usuario
    Task<ShoppingCartItemDTO?> UpdateAsync(int userId, int publicationId, ShoppingCartItemPutDTO shoppingCartItemDto);
}
