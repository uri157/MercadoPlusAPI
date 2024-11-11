using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

public class CardDbService : ICardService
{
    private readonly DbContext _context; // Contexto de la base de datos
    private readonly ICardTypeService _cardTypeService;


    public CardDbService(DbContext context, ICardTypeService cardTypeService)
    {
        _context = context;
        _cardTypeService = cardTypeService;
    }

    public CardDTO Create(CardPutPostDTO cardDto, int userId)
    {
        var cardType = _cardTypeService.GetById(cardDto.CardTypeId);

        var card = new Card
        {
            UserId = userId,
            CardNumber = cardDto.CardNumber,
            CardholderName = cardDto.CardholderName,
            ExpirationDate = cardDto.ExpirationDate,
            CVV = cardDto.Cvv,
            CardTypeId = cardDto.CardTypeId
        };

        _context.Cards.Add(card);
        try
        {
            _context.SaveChanges();
        }
        catch (DbUpdateException dbEx)
        {
            var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
            throw new InvalidOperationException($"Error al guardar en la base de datos: {innerMessage}", dbEx);
        }

        return new CardDTO
        {
            Id = card.Id,
            UserId = userId,
            CardNumber = card.CardNumber,
            CardholderName = card.CardholderName,
            ExpirationDate = card.ExpirationDate,
            Cvv = card.CVV,
            CardTypeId = card.CardTypeId
        };
    }
    



    // // Crear una nueva tarjeta
    // public CardDTO Create(CardPutPostDTO cardDto, int userId)
    // {
    //     // Buscar el CardType en la base de datos por el ID del tipo de tarjeta
    //     var cardType = _context.CardTypes.FirstOrDefault(ct => ct.Id == cardDto.CardTypeId);

    //     if (cardType == null)
    //     {
    //         throw new ArgumentException($"CardType with ID '{cardDto.CardTypeId}' not found");
    //     }

    //     Card card = new()
    //     {
    //         UserId = userId,
    //         CardNumber = cardDto.CardNumber,
    //         CardholderName = cardDto.CardholderName,
    //         ExpirationDate = cardDto.ExpirationDate,
    //         CVV = cardDto.Cvv,
    //         CardTypeId = cardDto.CardTypeId,  // Asignar el objeto CardType encontrado
    //     };

    //     _context.Cards.Add(card);
    //     try
    //     {
    //         _context.SaveChanges();
    //     }
    //     catch (DbUpdateException dbEx)
    //     {
    //         var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
    //         throw new InvalidOperationException($"Error al guardar en la base de datos: {innerMessage}", dbEx);
    //     }



    //    return new CardDTO
    //     {
    //         Id = card.Id,
    //         UserId = userId,
    //         CardNumber = card.CardNumber,
    //         CardholderName = card.CardholderName,
    //         ExpirationDate = card.ExpirationDate,
    //         Cvv = card.CVV,
    //         CardTypeId = card.CardTypeId
    //     };
    // }

    // Eliminar una tarjeta por su ID
    public void Delete(int id)
    {
        var card = _context.Cards.Find(id);

        if (card != null)
        {
            _context.Cards.Remove(card);
            _context.SaveChanges(); // Guardar los cambios
        }

    }

    // Obtener todas las tarjetas
    // public IEnumerable<CardDTO> GetAll()
    // {
    //     return _context.Cards.Include(c => c.CardType);  // Incluir el tipo de tarjeta en la respuesta
    // }

    // Obtener una tarjeta por su ID si es del usuario autenticado
    public CardDTO? GetUserCardById(int cardId, int userId)
    {
        var card = _context.Cards
            .Include(c => c.CardType)
            .FirstOrDefault(c => c.Id == cardId && c.UserId == userId);

        if (card == null)
            return null;

        // Mapear la entidad Card a CardDTO
        return new CardDTO
        {
            Id = card.Id,
            UserId = card.UserId,
            CardNumber = card.CardNumber,
            CardholderName = card.CardholderName,
            ExpirationDate = card.ExpirationDate,
            Cvv = card.CVV,
            CardTypeId = card.CardType.Id
        };
    }

    // Actualizar una tarjeta existente
    public CardDTO? Update(int id, CardPutPostDTO cardToUpdate, int userId)
    {
        var existingCard = _context.Cards.Find(id);
        if (existingCard != null)
        {
            // Buscar el nuevo CardType si se está actualizando
           
            var cardType = _cardTypeService.GetById(cardToUpdate.CardTypeId);
            if (cardType == null)
            {
                throw new Exception($"CardType with ID '{cardToUpdate.CardTypeId}' not found");
            }

            existingCard.UserId = userId;
            existingCard.CardNumber = cardToUpdate.CardNumber;
            existingCard.CardholderName = cardToUpdate.CardholderName;
            existingCard.ExpirationDate = cardToUpdate.ExpirationDate;
            existingCard.CVV = cardToUpdate.Cvv;
            existingCard.CardType = cardType;  // Asignar el nuevo CardType

            _context.Entry(existingCard).State = EntityState.Modified;
            _context.SaveChanges(); // Guardar cambios
            return new CardDTO
            {
                Id = existingCard.Id,
                UserId = existingCard.UserId,
                CardNumber = existingCard.CardNumber,
                CardholderName = existingCard.CardholderName,
                ExpirationDate = existingCard.ExpirationDate,
                Cvv = existingCard.CVV,
                CardTypeId = existingCard.CardTypeId
            };
        }

        return null;
    }

    public IEnumerable<CardDTO> GetAllUserCardsByUserId(int userId)
    {
        return _context.Cards
            .Include(c => c.CardType) // Incluir el tipo de tarjeta en la respuesta
            .Where(c => c.UserId == userId) // Filtrar por UserId
            .Select(c => new CardDTO
            {
                Id = c.Id,
                UserId = c.UserId,
                CardNumber = c.CardNumber,
                CardholderName = c.CardholderName,
                ExpirationDate = c.ExpirationDate,
                Cvv = c.CVV,
                CardTypeId = c.CardTypeId
            })
            .ToList();
    }
}


