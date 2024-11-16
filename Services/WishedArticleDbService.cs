using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class WishedArticleDbService : IWishedArticleService
{
    private readonly DbContext _context;

    public WishedArticleDbService(DbContext context)
    {
        _context = context;
    }

    // Crear un WishedArticle de manera asíncrona
    public async Task<WishedArticleDTO> CreateAsync(int idUser, WishedArticlePostPutDTO dto)
    {
        var newWishedArticle = new WishedArticle
        {
            IdUser = idUser,
            IdPublication = dto.IdPublication
        };

        _context.WishedArticles.Add(newWishedArticle);
        await _context.SaveChangesAsync();

        return new WishedArticleDTO
        {
            Id = newWishedArticle.Id,
            IdUser = newWishedArticle.IdUser,
            IdPublication = newWishedArticle.IdPublication
        };
    }

    // Obtener todos los WishedArticles de manera asíncrona
    public async Task<IEnumerable<WishedArticleDTO>> GetAllAsync()
    {
        return await _context.WishedArticles
            .Select(wa => new WishedArticleDTO
            {
                Id = wa.Id,
                IdUser = wa.IdUser,
                IdPublication = wa.IdPublication
            })
            .ToListAsync();
    }

    // Obtener un WishedArticle por ID de manera asíncrona
    public async Task<WishedArticleDTO?> GetByIdAsync(int id)
    {
        var wa = await _context.WishedArticles.FindAsync(id);
        if (wa == null)
        {
            return null;
        }

        return new WishedArticleDTO
        {
            Id = wa.Id,
            IdUser = wa.IdUser,
            IdPublication = wa.IdPublication
        };
    }

    // Eliminar un WishedArticle de manera asíncrona
    public async Task DeleteAsync(int id)
    {
        var wa = await _context.WishedArticles.FindAsync(id);
        if (wa != null)
        {
            _context.WishedArticles.Remove(wa);
            await _context.SaveChangesAsync();
        }
    }

    // Actualizar un WishedArticle de manera asíncrona
    public async Task<WishedArticleDTO?> UpdateAsync(int id, int idUser, WishedArticlePostPutDTO dto)
    {
        var wa = await _context.WishedArticles.FindAsync(id);
        if (wa != null)
        {
            wa.IdUser = idUser;
            wa.IdPublication = dto.IdPublication;

            _context.Entry(wa).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return new WishedArticleDTO
            {
                Id = wa.Id,
                IdUser = wa.IdUser,
                IdPublication = wa.IdPublication
            };
        }

        return null;
    }

    public async Task<IEnumerable<WishedArticleDTO>> GetAllByUserIdAsync(int userId)
    {
        return await _context.WishedArticles
            .Where(wa => wa.IdUser == userId)
            .Select(wa => new WishedArticleDTO
            {
                Id = wa.Id,
                IdUser = wa.IdUser,
                IdPublication = wa.IdPublication
            })
            .ToListAsync();
    }
}
