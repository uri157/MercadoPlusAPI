using System.Threading.Tasks;

public interface IShoppingCartService
{
    // Obtener el carrito de compras de un usuario por su ID
    Task<ShoppingCartDTO> GetCartByUserIdAsync(int userId);

    // Agregar una publicación al carrito del usuario
    Task<ShoppingCartDTO> AddToCartAsync(int userId, ShoppingCartItemPostDTO shoppingCartItemDTO);

    // Remover un producto del carrito
    Task<bool> RemoveFromCartAsync(int userId, int publicationId);

    // Vaciar el carrito de un usuario
    Task<bool> ClearCartAsync(int userId);
}
