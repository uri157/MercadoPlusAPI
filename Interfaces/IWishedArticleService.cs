using System.Collections.Generic;
using System.Threading.Tasks;

public interface IWishedArticleService
{
    Task<WishedArticleDTO> CreateAsync(int idUser, WishedArticlePostPutDTO dto);
    Task<IEnumerable<WishedArticleDTO>> GetAllAsync();
    Task<WishedArticleDTO?> GetByIdAsync(int id);
    Task DeleteAsync(int id);
    Task<IEnumerable<WishedArticleDTO>> GetAllByUserIdAsync(int userId); // Nuevo método
    Task<WishedArticleDTO?> UpdateAsync(int id, int idUser, WishedArticlePostPutDTO dto);
}
