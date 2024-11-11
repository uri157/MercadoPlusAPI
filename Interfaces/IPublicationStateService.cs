using System.Collections.Generic;

public interface IPublicationStateService
{
    IEnumerable<PublicationStateDTO> GetAll(); // Obtener todos los estados de publicación
    PublicationStateDTO? GetById(int id); // Obtener un estado por su ID
    PublicationStateDTO Create(PublicationStatePostDTO publicationStateDto); // Crear un nuevo estado de publicación
    void Delete(int id); // Eliminar un estado por su ID
    PublicationStateDTO? Update(PublicationStateDTO publicationStateDto); // Actualizar un estado existente
}
