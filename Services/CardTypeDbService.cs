using Microsoft.EntityFrameworkCore;

public class CardTypeDbService : ICardTypeService
{
    private readonly DbContext _context; // Contexto de la base de datos

    public CardTypeDbService(DbContext context)
    {
        _context = context;
    }

    // Crear un nuevo tipo de tarjeta
    public CardTypeDTO Create(CardTypePutDTO cardTypeDto)
    {
        CardType cardType = new()
        {
            TypeName = cardTypeDto.TypeName,
            Description = cardTypeDto.Description
        };

        _context.CardTypes.Add(cardType);
        _context.SaveChanges(); // Guardar cambios en la base de datos
        return new CardTypeDTO
        {
            Id = cardType.Id,
            TypeName = cardType.TypeName,
            Description = cardType.Description
        };
    }

    // Eliminar un tipo de tarjeta por su ID
    public void Delete(int id)
    {
        var cardType = _context.CardTypes.Find(id);
        if (cardType != null)
        {
            _context.CardTypes.Remove(cardType);
            _context.SaveChanges(); // Guardar los cambios
        }
    }

    // Obtener todos los tipos de tarjetas
    public IEnumerable<CardTypeDTO> GetAll()
    {
        return _context.CardTypes
            .Select(ct => new CardTypeDTO
            {
                Id = ct.Id,
                TypeName = ct.TypeName,
                Description = ct.Description
            })
            .ToList();
    }

    // Obtener un tipo de tarjeta por su ID
    public CardType? GetById(int id)
    {
        var cardType = _context.CardTypes.Find(id);

        if (cardType == null)
        {
            throw new ArgumentException($"CardType with ID '{id}' not found");
        }

        return cardType;
    }

    // Actualizar un tipo de tarjeta existente
    public CardTypeDTO? Update(int id, CardTypePutDTO cardTypeDto)
    {
        var existingCardType = _context.CardTypes.Find(id);
        if (existingCardType != null)
        {
            existingCardType.TypeName = cardTypeDto.TypeName;
            existingCardType.Description = cardTypeDto.Description;

            _context.Entry(existingCardType).State = EntityState.Modified;
            _context.SaveChanges(); // Guardar cambios
            return new CardTypeDTO
            {
                Id = existingCardType.Id,
                TypeName = existingCardType.TypeName,
                Description = existingCardType.Description
            };
        }

        return null;
    }
}
