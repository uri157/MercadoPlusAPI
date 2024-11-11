using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/cards")]
public class CardController : ControllerBase
{
    private readonly ICardService _cardService;

    public CardController(ICardService cardService)
    {
        _cardService = cardService;
    }

    // Obtener todas las tarjetas del usuario autenticado
    [Authorize]
    [HttpGet("user")]
    public ActionResult<List<CardDTO>> GetAllUserCards()
    {
        // Obtener el ID del usuario autenticado
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        
        // Obtener solo las tarjetas del usuario autenticado
        return Ok(_cardService.GetAllUserCardsByUserId(userId));
    }

     // Obtener una tarjeta por ID si pertenece al usuario autenticado
    [Authorize]
    [HttpGet("{cardId}")]
    public ActionResult<Card> GetById(int cardId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var cardDto = _cardService.GetUserCardById(cardId, userId);

        if (cardDto == null)
        {
            return NotFound("Card not found or access denied");
        }

        return Ok(cardDto);
    }

    [Authorize]
    [HttpPost]
    public ActionResult<CardDTO> NewCard(CardPutPostDTO cardDto)
    {
        try
        {
            // Obtener el ID del usuario autenticado
            int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userId == null)
            {
                return Unauthorized("No se encontró el identificador del usuario.");
            }

            // Crear la tarjeta
            var newCard = _cardService.Create(cardDto, userId);
            return Ok(newCard);
        }
        catch (ArgumentException ex)
        {
            // Manejar errores específicos del servicio (ej., tipo de tarjeta inexistente)
            return BadRequest(new { Message = "Error en los datos de la tarjeta.", Details = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            var errorMessage = ex.InnerException?.Message ?? ex.Message;
            return Conflict(new { Message = "Conflicto al crear la tarjeta.", Details = errorMessage });
        }
        catch (Exception ex)
        {
            // Manejar cualquier otro error no previsto
            return StatusCode(500, new { Message = "Ocurrió un error interno en el servidor.", Details = ex.Message });
        }
    }



    // // Crear una nueva tarjeta para el usuario autenticado
    // [Authorize]
    // [HttpPost]
    // public ActionResult<CardDTO> NewCard(CardPutPostDTO cardDto)
    // {
    //     try
    //     {
    //         // Verificar que el usuario autenticado tenga un ID válido
    //         var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    //         if (userIdClaim == null)
    //         {
    //             return Unauthorized("No se encontró el identificador del usuario.");
    //         }

    //         if (!int.TryParse(userIdClaim, out var userId))
    //         {
    //             return BadRequest("El identificador de usuario es inválido.");
    //         }

    //         // Crear la tarjeta
    //         var newCard = _cardService.Create(cardDto, userId);
    //         return newCard;
    //     }
    //     catch (ArgumentException ex)
    //     {
    //         // Manejar errores específicos del servicio (ej., tipo de tarjeta inexistente)
    //         return BadRequest(new { Message = "Error en los datos de la tarjeta.", Details = ex.Message });
    //     }
    //     catch (InvalidOperationException ex)
    //     {
    //         var errorMessage = ex.InnerException?.Message ?? ex.Message;
    //         return Conflict(new { Message = "Conflicto al crear la tarjeta.", Details = errorMessage });
    //     }
    //     catch (Exception ex)
    //     {
    //         // Log del error (opcional)
    //         // _logger.LogError(ex, "Error no previsto en NewCard");

    //         // Manejar cualquier otro error no previsto
    //         return StatusCode(500, new { Message = "Ocurrió un error interno en el servidor.", Details = ex.Message });
    //     }
    // }


    // Eliminar una tarjeta por ID si pertenece al usuario autenticado
    [Authorize]
    [HttpDelete("{cardId}")]
    public ActionResult Delete(int cardId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        var card = _cardService.GetUserCardById(userId,cardId);

        if (card == null || card.UserId != userId)
        {
            return NotFound("Card not found or access denied");
        }

        _cardService.Delete(cardId);
        return NoContent();
    }

   // Actualizar una tarjeta por ID si pertenece al usuario autenticado
    [Authorize]
    [HttpPut("{cardId}")]
    public ActionResult<Card> UpdateCard(int cardId, CardPutPostDTO updatedCard)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        var card = _cardService.GetUserCardById(userId, cardId);

        if (card == null || card.UserId != userId)
        {
            return NotFound("Card not found or access denied");
        }

        var updated = _cardService.Update(cardId, updatedCard, userId);

        return CreatedAtAction(nameof(GetById), new { id = updated.Id }, updated);
    }


    // // Obtener todas las tarjetas del usuario autenticado
    // [Authorize]
    // [HttpGet("user")]
    // public ActionResult<List<CardDTO>> GetUserCards()
    // {
    //     // Obtener el ID del usuario autenticado
    //     var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        
    //     // Obtener solo las tarjetas del usuario autenticado
    //     return Ok(_cardService.GetByUserId(userId));
    // }

}