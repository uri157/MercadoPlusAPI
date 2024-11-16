using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class ShoppingCartController : ControllerBase
{
    private readonly IShoppingCartService _shoppingCartService;

    public ShoppingCartController(IShoppingCartService shoppingCartService)
    {
        _shoppingCartService = shoppingCartService;
    }

    // Obtener el carrito de un usuario
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        // Obtención del ID de usuario
        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
        {
            return Unauthorized("Invalid user ID.");
        }

        var cart = await _shoppingCartService.GetCartByUserIdAsync(userId);

        if (cart == null)
        {
            return NotFound("No se encontró el carrito del usuario.");
        }

        return Ok(cart);
    }

    // Agregar una publicación al carrito
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddToCart([FromBody] ShoppingCartItemPostDTO shoppingCartItemDTO)
    {
        if (shoppingCartItemDTO == null)
        {
            return BadRequest("Datos del carrito inválidos.");
        }

        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
        {
            return Unauthorized("Invalid user ID.");
        }

        // Agregar al carrito
        try
        {
            var cart = await _shoppingCartService.AddToCartAsync(userId, shoppingCartItemDTO);
            return Ok(cart);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            // Manejo genérico de excepciones
            return StatusCode(500, $"Ocurrió un error: {ex.Message}");
        }
    }

    // Remover un producto del carrito
    [Authorize]
    [HttpDelete("{publicationId}")]
    public async Task<IActionResult> RemoveFromCart(int publicationId)
    {
        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
        {
            return Unauthorized("Invalid user ID.");
        }

        try
        {
            await _shoppingCartService.RemoveFromCartAsync(userId, publicationId);
            return Ok("Producto eliminado del carrito.");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            // Manejo genérico de excepciones
            return StatusCode(500, $"Ocurrió un error: {ex.Message}");
        }
    }

    // Vaciar el carrito
    [Authorize]
    [HttpDelete("clear")]
    public async Task<IActionResult> ClearCart()
    {
        if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
        {
            return Unauthorized("Invalid user ID.");
        }

        try
        {
            var result = await _shoppingCartService.ClearCartAsync(userId);
            if (result)
            {
                return Ok("Carrito vaciado.");
            }
            else
            {
                return NotFound("No se encontró el carrito del usuario o ya está vacío.");
            }
        }
        catch (Exception ex)
        {
            // Manejo genérico de excepciones
            return StatusCode(500, $"Ocurrió un error: {ex.Message}");
        }
    }
}
