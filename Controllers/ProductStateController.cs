using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/product-states")]
public class ProductStateController : ControllerBase
{
    private readonly IProductStateService _productStateService;

    public ProductStateController(IProductStateService productStateService)
    {
        _productStateService = productStateService;
    }

    // Obtener todos los estados de producto
    [AllowAnonymous]
    [HttpGet]
    public ActionResult<List<ProductStateDTO>> GetAllProductStates()
    {
        return Ok(_productStateService.GetAll());
    }

    // Obtener un estado de producto por ID
    [AllowAnonymous]
    [HttpGet("{id}")]
    public ProductStateDTO GetById(int id)
    {
        return _productStateService.GetById(id);
    }

    // Crear un nuevo estado de producto
    [Authorize(Roles = "admin")]
    [HttpPost]
    public ActionResult<ProductStateDTO> NewProductState(ProductStatePostDTO productStateDto)
    {
        var newProductState = _productStateService.Create(productStateDto);
        return CreatedAtAction(nameof(GetById), new { id = newProductState.Id }, newProductState);
    }

    // Eliminar un estado de producto por ID
    [Authorize(Roles = "admin")]
    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        _productStateService.Delete(id);
        return NoContent();
    }

    // Actualizar un estado de producto por ID
    [Authorize(Roles = "admin")]
    [HttpPut("{id}")]
    public ActionResult<ProductStateDTO> UpdateProductState(ProductStateDTO productStateDto)
    {
        var updatedProductState = _productStateService.Update(productStateDto);

        return CreatedAtAction(nameof(GetById), new { id = updatedProductState.Id }, updatedProductState);
    }
}
