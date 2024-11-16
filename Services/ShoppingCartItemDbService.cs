using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class ShoppingCartItemDbService : IShoppingCartItemService
{
    private readonly DbContext _context;

    public ShoppingCartItemDbService(DbContext context)
    {
        _context = context;
    }

    // Implementación de CreateAsync
    public async Task<ShoppingCartItemDTO> CreateAsync(ShoppingCartItemPostDTO shoppingCartItemDto)
    {
        var shoppingCartItem = new ShoppingCartItem
        {
            ShoppingCartId = shoppingCartItemDto.ShoppingCartId,
            PublicationId = shoppingCartItemDto.PublicationId,
            Quantity = shoppingCartItemDto.Quantity
        };

        await _context.ShoppingCartItems.AddAsync(shoppingCartItem);
        await _context.SaveChangesAsync();

        // Cargar la publicación relacionada para obtener el título y precio
        await _context.Entry(shoppingCartItem).Reference(item => item.Publication).LoadAsync();

        return new ShoppingCartItemDTO
        {
            Id = shoppingCartItem.Id,
            ShoppingCartId = shoppingCartItem.ShoppingCartId,
            PublicationId = shoppingCartItem.PublicationId,
            Quantity = shoppingCartItem.Quantity,
            Title = shoppingCartItem.Publication.Title,
            Price = shoppingCartItem.Publication.Price
        };
    }

    // Implementación de DeleteAsync
    public async Task DeleteAsync(int userId, int publicationId)
    {
        var shoppingCart = await _context.ShoppingCarts
            .FirstOrDefaultAsync(cart => cart.IdUser == userId);

        if (shoppingCart == null)
        {
            throw new KeyNotFoundException($"No se encontró un carrito para el usuario con ID {userId}");
        }

        var shoppingCartItem = await _context.ShoppingCartItems
            .FirstOrDefaultAsync(item => item.ShoppingCartId == shoppingCart.Id && item.PublicationId == publicationId);

        if (shoppingCartItem == null)
        {
            throw new KeyNotFoundException($"ShoppingCartItem con PublicationId '{publicationId}' no encontrado en el carrito del usuario");
        }

        _context.ShoppingCartItems.Remove(shoppingCartItem);
        await _context.SaveChangesAsync();
    }

    // Implementación de GetAllAsync
    public async Task<IEnumerable<ShoppingCartItemDTO>> GetAllAsync(int userId)
    {
        var shoppingCart = await _context.ShoppingCarts
            .Include(cart => cart.Items)
                .ThenInclude(item => item.Publication)
            .FirstOrDefaultAsync(cart => cart.IdUser == userId);

        if (shoppingCart == null)
        {
            throw new KeyNotFoundException($"No se encontró un carrito para el usuario con ID {userId}");
        }

        var items = shoppingCart.Items.Select(item => new ShoppingCartItemDTO
        {
            Id = item.Id,
            ShoppingCartId = item.ShoppingCartId,
            PublicationId = item.PublicationId,
            Quantity = item.Quantity,
            Title = item.Publication.Title,
            Price = item.Publication.Price
        }).ToList();

        return items;
    }

    // Implementación de GetByIdAsync
    public async Task<ShoppingCartItemDTO?> GetByIdAsync(int userId, int publicationId)
    {
        var shoppingCart = await _context.ShoppingCarts
            .FirstOrDefaultAsync(cart => cart.IdUser == userId);

        if (shoppingCart == null)
        {
            throw new KeyNotFoundException($"No se encontró un carrito para el usuario con ID {userId}");
        }

        var shoppingCartItem = await _context.ShoppingCartItems
            .Include(item => item.Publication)
            .FirstOrDefaultAsync(item => item.ShoppingCartId == shoppingCart.Id && item.PublicationId == publicationId);

        if (shoppingCartItem == null)
        {
            throw new KeyNotFoundException($"ShoppingCartItem con PublicationId '{publicationId}' no encontrado en el carrito del usuario");
        }

        return new ShoppingCartItemDTO
        {
            Id = shoppingCartItem.Id,
            ShoppingCartId = shoppingCartItem.ShoppingCartId,
            PublicationId = shoppingCartItem.PublicationId,
            Quantity = shoppingCartItem.Quantity,
            Title = shoppingCartItem.Publication.Title,
            Price = shoppingCartItem.Publication.Price
        };
    }

    // Implementación de UpdateAsync
    public async Task<ShoppingCartItemDTO?> UpdateAsync(int userId, int publicationId, ShoppingCartItemPutDTO shoppingCartItemDto)
    {
        var shoppingCart = await _context.ShoppingCarts
            .FirstOrDefaultAsync(cart => cart.IdUser == userId);

        if (shoppingCart == null)
        {
            throw new KeyNotFoundException($"No se encontró un carrito para el usuario con ID {userId}");
        }

        var existingItem = await _context.ShoppingCartItems
            .FirstOrDefaultAsync(item => item.ShoppingCartId == shoppingCart.Id && item.PublicationId == publicationId);

        if (existingItem == null)
        {
            throw new KeyNotFoundException($"ShoppingCartItem con PublicationId '{publicationId}' no encontrado en el carrito del usuario");
        }

        existingItem.Quantity = shoppingCartItemDto.Quantity;
        _context.ShoppingCartItems.Update(existingItem);
        await _context.SaveChangesAsync();

        // Cargar la publicación relacionada para obtener el título y precio
        await _context.Entry(existingItem).Reference(item => item.Publication).LoadAsync();

        return new ShoppingCartItemDTO
        {
            Id = existingItem.Id,
            ShoppingCartId = existingItem.ShoppingCartId,
            PublicationId = existingItem.PublicationId,
            Quantity = existingItem.Quantity,
            Title = existingItem.Publication.Title,
            Price = existingItem.Publication.Price
        };
    }
}
