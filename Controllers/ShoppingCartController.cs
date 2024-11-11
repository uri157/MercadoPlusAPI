using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        var cart = await _shoppingCartService.GetCartByUserId(userId);
        
        return Ok(cart);
    }

    // Agregar una publicación al carrito
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddToCart(ShoppingCartItemPostDTO shoppingCartItemDTO)
    {
        int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        // Agregar al carrito
        var cart = await _shoppingCartService.AddToCart(userId, shoppingCartItemDTO);

        return Ok(cart);
      
    }

    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> RemoveFromCart(int publicationId)
    {   
        int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        _shoppingCartService.RemoveFromCart(userId, publicationId);
        return Ok("Producto eliminado del carrito.");
    }

    // Vaciar el carrito
    [Authorize]
    [HttpDelete("/clear")]
    public async Task<IActionResult> ClearCart(int userId)
    {
        var result = await _shoppingCartService.ClearCart(userId);
        return Ok("Carrito vaciado.");
    }
}
