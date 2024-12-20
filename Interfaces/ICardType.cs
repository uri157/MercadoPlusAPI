public interface ICardTypeService
{
    public IEnumerable<CardTypeDTO> GetAll(); // Obtener todos los tipos de tarjetas
    public CardType? GetById(int id); // Obtener un tipo de tarjeta por su ID
    public CardTypeDTO Create(CardTypePutDTO cardTypeDto); // Crear un nuevo tipo de tarjeta
    public void Delete(int id); // Eliminar un tipo de tarjeta
    public CardTypeDTO? Update(int id, CardTypePutDTO cardTypeDto); // Actualizar un tipo de tarjeta existente
}
