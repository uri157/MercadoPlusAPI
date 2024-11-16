using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class TransactionDbService : ITransactionService
{
    private readonly DbContext _context;
    private readonly ICardService _cardService;
    private readonly IPublicationService _publicationService;
    
    public TransactionDbService
    (
    DbContext context,
    ICardService cardService, 
    IPublicationService publicationService
    )
    {
        _context = context;
        _cardService = cardService;
        _publicationService = publicationService;
    }

    // // Obtener todas las transacciones
    // public async Task<List<TransactionDTO>> GetAll()
    // {
    //     return await _context.Set<Transaction>()
    //         .Include(t => t.Buyer)
    //         .Include(t => t.Publication)
    //         .Select(t => new TransactionDTO
    //         {
    //             Id = t.Id,
    //             IdCard = t.IdCard,
    //             IdBuyer = t.IdBuyer,
    //             IdPublication = t.IdPublication,
    //             TransactionDate = t.TransactionDate,
    //             Amount = t.Amount,
    //             Calification = t.Calification,
    //             ReviewText = t.ReviewText
    //         })
    //         .ToListAsync();
    // }

    // Obtener una transacción por ID
    public async Task<TransactionDTO> GetById(int id)
    {
        var transaction = await _context.Set<Transaction>()
            .Include(t => t.Buyer)
            .Include(t => t.Publication)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (transaction == null)
        {
            throw new Exception("Transaction not found"); // o lanzar una excepción si prefieres
        }

        return new TransactionDTO
        {
            Id = transaction.Id,
            IdBuyer = transaction.IdBuyer,
            IdPublication = transaction.IdPublication,
            TransactionDate = transaction.TransactionDate,
            Amount = transaction.Amount,
            Calification = transaction.Calification,
            ReviewText = transaction.ReviewText
        };
    }

    // Crear una nueva transacción
    public async Task<TransactionDTO> Create(int userId, TransactionPostDTO transactionPostDto)
    {
        // Consulta la publicación para obtener el Amount
        var publication = await _context.Set<Publication>()
            .FirstOrDefaultAsync(p => p.Id == transactionPostDto.IdPublication);

        if (publication == null)
        {
            // Maneja el caso de publicación no encontrada
            throw new Exception("Publication not found");
        }

        var transaction = new Transaction
        {
            IdCard = transactionPostDto.IdCard,
            IdBuyer = userId,
            IdPublication = transactionPostDto.IdPublication,
            TransactionDate = DateTime.UtcNow, // O la fecha que prefieras
            Amount = publication.Price
        };

        _context.Set<Transaction>().Add(transaction);
        await _context.SaveChangesAsync();

        return new TransactionDTO
        {
            IdCard = transactionPostDto.IdCard,
            Id = transaction.Id,
            IdBuyer = transaction.IdBuyer,
            IdPublication = transaction.IdPublication,
            TransactionDate = transaction.TransactionDate,
            Amount = transaction.Amount,
            Calification = transaction.Calification,
            ReviewText = transaction.ReviewText
        };
    }

    // Actualizar una transacción existente
    public async Task<bool> Update(int id, int userId, TransactionPutDTO transactionPutDto)
    {
        var transaction = await _context.Set<Transaction>().FindAsync(id);
        if (transaction == null || transaction.IdBuyer != userId)
        {
            throw new Exception("Transaction not found or unautorizhed"); // No se encontró la transacción o el usuario no tiene permiso para actualizarla
        }

        transaction.Calification = transactionPutDto.Calification;
        transaction.ReviewText = transactionPutDto.ReviewText;

        await _context.SaveChangesAsync();
        return true;
    }


    // Obtener transacciones por usuario
    public async Task<List<TransactionDTO>> GetTransactionsByUserIdAsync(int userId)
    {
        return await _context.Set<Transaction>()
            .Where(t => t.IdBuyer == userId)
            .Select(t => new TransactionDTO
            {
                Id = t.Id,
                IdBuyer = t.IdBuyer,
                IdPublication = t.IdPublication,
                TransactionDate = t.TransactionDate,
                Amount = t.Amount,
                Calification = t.Calification,
                ReviewText = t.ReviewText
            })
            .ToListAsync();
    }



    // Eliminar una transacción por ID
    public async Task<bool> Delete(int id)
    {
        var transaction = await _context.Set<Transaction>().FindAsync(id);
        if (transaction == null)
        {
            throw new Exception("Transaction not found"); // No se encontró la transacción
        }

        _context.Set<Transaction>().Remove(transaction);
        await _context.SaveChangesAsync();
        return true;
    }

    // Obtener transacciones por publicación
    public async Task<List<TransactionDTO>> GetTransactionsByPublicationIdAsync(int publicationId)
    {
        return await _context.Set<Transaction>()
            .Where(t => t.IdPublication == publicationId)
            .Select(t => new TransactionDTO
            {
                Id = t.Id,
                IdBuyer = t.IdBuyer,
                IdPublication = t.IdPublication,
                TransactionDate = t.TransactionDate,
                Amount = t.Amount,
                Calification = t.Calification,
                ReviewText = t.ReviewText
            })
            .ToListAsync();
    }


}
