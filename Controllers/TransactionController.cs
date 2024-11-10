using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

[Route("api/[controller]")]
[ApiController]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly ICardService _cardService;
    private readonly INotificationService _notificationService;
    private readonly IPublicationService _publicationService;


    public TransactionController(ITransactionService transactionService, ICardService cardService, INotificationService notificationService, IPublicationService publicationService)
    {
        _transactionService = transactionService;
        _cardService = cardService;
        _notificationService = notificationService;
        _publicationService = publicationService;
    }

    // Obtener todas las transacciones
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<TransactionDTO>>> GetAll()
    {
        var transactions = await _transactionService.GetAll();
        return Ok(transactions);
    }

    // Obtener una transacción por ID
    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<TransactionDTO>> GetById(int id)
    {
        var transaction = await _transactionService.GetById(id);
        if (transaction == null)
        {
            return NotFound(); // Retorna 404 si no se encuentra
        }
        return Ok(transaction);
    }

    // Crear una nueva transacción
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<TransactionDTO>> Create([FromBody] TransactionPostDTO transactionPostDto)
    {
        // Se asume que el userId es obtenido del contexto del usuario autenticado
        int UserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

         // Verificar que la tarjeta existe y pertenece al usuario
        var card = _cardService.GetById(transactionPostDto.IdCard);

        if (card == null || card.UserId != UserId)
        {
            throw new UnauthorizedAccessException("La tarjeta no pertenece al usuario autenticado.");
        }

        //Enviar notificacion al comprador
        _notificationService.Create(new NotificationPutPostDTO
            {
                Text = "Felicidades por tu nueva compra!",
                userId = UserId,
                Readed = false
            }
        );

        //Enviar notificacion al vendedor
        _notificationService.Create(new NotificationPutPostDTO
            {
                Text = "Felicidades por tu nueva venta!",
                userId = _publicationService.GetById(transactionPostDto.IdPublication).IdUsuario,
                Readed = false
            }
        );


        var transaction = await _transactionService.Create(UserId, transactionPostDto);



        return CreatedAtAction(nameof(GetById), new { id = transaction.Id }, transaction);
    }

    // Actualizar una transacción existente
    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] TransactionPutDTO transactionPutDto)
    {
        // Se asume que el userId es obtenido del contexto del usuario autenticado
        int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        var result = await _transactionService.Update(id, userId, transactionPutDto);
        if (!result)
        {
            return NotFound(); // Retorna 404 si no se encuentra
        }
        return NoContent(); // Retorna 204 si la actualización es exitosa
    }

    // Eliminar una transacción por ID
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _transactionService.Delete(id);
        if (!result)
        {
            return NotFound(); // Retorna 404 si no se encuentra
        }
        return NoContent(); // Retorna 204 si la eliminación es exitosa
    }

    

}
