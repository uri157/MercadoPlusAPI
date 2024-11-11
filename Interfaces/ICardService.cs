public interface ICardService
{
    //public IEnumerable<CardDTO> GetAll(); // Obtener todas las tarjetas
    public CardDTO? GetUserCardById(int cardId, int userId); // Obtener una tarjeta por su ID si es del usuario autenticado
    public IEnumerable<CardDTO> GetAllUserCardsByUserId(int id); // Obtener una tarjeta por su ID
    public CardDTO Create(CardPutPostDTO cardDto, int userId); // Crear una nueva tarjeta
    public void Delete(int id); // Eliminar una tarjeta
    public CardDTO? Update(int id, CardPutPostDTO card, int userId); // Actualizar una tarjeta existente
}
