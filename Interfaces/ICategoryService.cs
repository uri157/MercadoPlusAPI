public interface ICategoryService
{
    Category Create(CategoryPostDTO categoryDto);
    void Delete(int id);
    IEnumerable<CategoryDTO> GetAll();
    CategoryDTO? GetById(int id);
    CategoryDTO? Update(CategoryDTO categoryToUpdate);
}


