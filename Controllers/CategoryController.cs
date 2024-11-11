using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

[ApiController]
[Route("api/categories")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    // Obtener todas las categorías
    [AllowAnonymous]
    [HttpGet]
    public ActionResult<List<CategoryDTO>> GetAllCategories()
    {
        var categories = _categoryService.GetAll();

        return Ok(categories);
    }

    // Obtener una categoría por ID
    [AllowAnonymous]
    [HttpGet("{id}")]
    public ActionResult<CategoryDTO> GetById(int id)
    {
        var category = _categoryService.GetById(id);

        return Ok(category);
    }

    // Crear una nueva categoría
    [Authorize(Roles = "admin")]
    [HttpPost]
    public ActionResult<CategoryDTO> NewCategory(CategoryPostDTO categoryDto)
    {
        var newCategory = _categoryService.Create(categoryDto);
        return CreatedAtAction(nameof(GetById), new { id = newCategory.Id }, newCategory);
    }

    // Eliminar una categoría por ID
    [Authorize(Roles = "admin")]
    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        _categoryService.Delete(id);
        return NoContent();
    }

    // Actualizar una categoría por ID
    [Authorize(Roles = "admin")]
    [HttpPut("{id}")]
    public ActionResult<CategoryDTO> UpdateCategory(CategoryDTO categoryDto)
    {

        var updatedCategory = _categoryService.Update(categoryDto);

        return CreatedAtAction(nameof(GetById), new { id = updatedCategory }, updatedCategory);
    }
}
