using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/notifications")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly IMailService _mailService;

    private readonly IUserService _userService;



    public NotificationController
    (
        INotificationService notificationService, 
        IMailService mailService, 
        IUserService userService
    )
    {
        _notificationService = notificationService;
        _mailService = mailService;
        _userService = userService;
    }


   
    [Authorize]
    [HttpPost]
    public ActionResult<NotificationDTO> Create([FromBody] NotificationPostDTO notificationDto)
    {
        var createdNotification = _notificationService.Create(notificationDto);

        if (createdNotification == null)
        {
            return NotFound("Not able to create notification");
        }

        var htmlMessage = $@"
        <html>
            <body style='font-family: Arial, sans-serif; text-align: center; background-color: #f9f9f9; padding: 50px;'>
                <p style='font-size: 18px; color: #555;'>{createdNotification.Text}:</p>
            </body>
        </html>";

            // _mailService.SendEmailAsync(
            // _userService.GetById(notificationDto.userId).Email,
            // "Nueva Notificacion!",
            // htmlMessage,
            // "MercadoPlus Support <support@mercadoplus.xyz>"

            try
            {
                _mailService.SendEmailAsync(
                    _userService.GetById(notificationDto.userId).Email,
                    "Confirmación de Registro",
                    htmlMessage,
                    "MercadoPlus Support <support@mercadoplus.xyz>"
                );
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error al enviar correo de confirmación.", Details = ex.Message });
            }
        

        return createdNotification;
    }



    // Eliminar una notificación por ID
    [Authorize(Roles = "admin")]
    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        _notificationService.Delete(id);
        return NoContent();
    }

    // Actualizar una notificación por ID
    [Authorize(Roles = "admin")]
    [HttpPut("{id}")]
    public ActionResult<NotificationDTO> UpdateNotification(NotificationPutDTO notificationToUpdate)
    {
        return _notificationService.Update(notificationToUpdate);
    }

    [Authorize]
    [HttpGet("User-Notifications")]
    public IEnumerable<NotificationDTO> getUserNotifications()
    {
        int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        return _notificationService.GetAllByUserId(userId);
    }
    [Authorize]
    [HttpGet("byId")]
    public IActionResult getNotificationById(int notificationId)
    {
        int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        var notification = _notificationService.GetById(notificationId);

        if (notification == null)
        {
            return NotFound("La notificación no fue encontrada.");
        }

        if (notification.userId != userId)
        {
            return Forbid("No tienes autorización para realizar esta operación.");
        }

        return Ok(notification);
    }
}
