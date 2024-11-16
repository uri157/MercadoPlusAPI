using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class ShoppingCartDbService : IShoppingCartService
{
    private readonly DbContext _context;
    private readonly IShoppingCartItemService _shoppingCartItemService;

    public ShoppingCartDbService(DbContext context, IShoppingCartItemService shoppingCartItemService)
    {
        _context = context;
        _shoppingCartItemService = shoppingCartItemService;
    }

    // Implementación de GetCartByUserIdAsync
    public async Task<ShoppingCartDTO> GetCartByUserIdAsync(int userId)
    {
        var cart = await _context.ShoppingCarts
            .Where(cart => cart.IdUser == userId)
            .Include(cart => cart.Items)
                .ThenInclude(item => item.Publication)
            .Select(cart => new ShoppingCartDTO
            {
                Id = cart.Id,
                IdUser = cart.IdUser,
                Total = cart.Items.Sum(item => item.Publication.Price * item.Quantity),
                Items = cart.Items.Select(item => new ShoppingCartItemDTO
                {
                    Id = item.Id,
                    ShoppingCartId = item.ShoppingCartId,
                    PublicationId = item.PublicationId,
                    Title = item.Publication.Title,
                    Price = item.Publication.Price,
                    Quantity = item.Quantity
                }).ToList()
            })
            .FirstOrDefaultAsync();

        return cart;
    }

    // Implementación de AddToCartAsync
    public async Task<ShoppingCartDTO> AddToCartAsync(int userId, ShoppingCartItemPostDTO shoppingCartItemDTO)
    {
        if (shoppingCartItemDTO.Quantity <= 0)
        {
            throw new ArgumentException("Quantity must be higher than 0", nameof(shoppingCartItemDTO.Quantity));
        }

        // Obtener el carrito del usuario
        var cart = await _context.ShoppingCarts
            .Include(c => c.Items)
                .ThenInclude(item => item.Publication)
            .FirstOrDefaultAsync(c => c.IdUser == userId);

        // Si el carrito no existe, crearlo
        if (cart == null)
        {
            cart = new ShoppingCart { IdUser = userId };
            _context.ShoppingCarts.Add(cart);
            await _context.SaveChangesAsync(); // Después de esto, cart.Id debería tener el valor generado
        }

        // Asignar el ShoppingCartId al DTO
        shoppingCartItemDTO.ShoppingCartId = cart.Id;

        // Buscar si el ítem ya existe en el carrito
        var existingItem = cart.Items.FirstOrDefault(item => item.PublicationId == shoppingCartItemDTO.PublicationId);

        if (existingItem != null)
        {
            // Actualizar la cantidad
            var updatedDto = new ShoppingCartItemPutDTO
            {
                Quantity = existingItem.Quantity + shoppingCartItemDTO.Quantity
            };
            await _shoppingCartItemService.UpdateAsync(userId, shoppingCartItemDTO.PublicationId, updatedDto);
        }
        else
        {
            // Crear un nuevo ítem en el carrito
            await _shoppingCartItemService.CreateAsync(shoppingCartItemDTO);
        }

        // Retornar el carrito actualizado
        return await GetCartByUserIdAsync(userId);
    }

    // Implementación de RemoveFromCartAsync
    public async Task<bool> RemoveFromCartAsync(int userId, int publicationId)
    {
        var cart = await _context.ShoppingCarts
            .FirstOrDefaultAsync(c => c.IdUser == userId);

        if (cart == null)
        {
            throw new KeyNotFoundException($"No se encontró un carrito para el usuario con ID {userId}");
        }

        await _shoppingCartItemService.DeleteAsync(userId, publicationId);

        return true;
    }

    // Implementación de ClearCartAsync
    public async Task<bool> ClearCartAsync(int userId)
    {
        var shoppingCart = await _context.ShoppingCarts
            .FirstOrDefaultAsync(cart => cart.IdUser == userId);

        if (shoppingCart == null) return false;

        var cartItems = await _context.ShoppingCartItems
            .Where(item => item.ShoppingCartId == shoppingCart.Id)
            .ToListAsync();

        if (!cartItems.Any()) return false;

        foreach (var item in cartItems)
        {
            await _shoppingCartItemService.DeleteAsync(userId, item.PublicationId);
        }

        return true;
    }
}
