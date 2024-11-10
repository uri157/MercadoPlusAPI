using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/notifications")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly IMailService _mailService;

    private readonly IUserService _userService;


    public NotificationController(INotificationService notificationService, IMailService mailService, IUserService userService)
    {
        _notificationService = notificationService;
        _mailService = mailService;
        _userService = userService;
    }


    [Authorize]
    [HttpPost]
    public ActionResult<NotificationDTO> Create([FromBody] NotificationPutPostDTO notificationDto)
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

        _mailService.SendEmailAsync(
            _userService.GetById(notificationDto.userId).Email,
            "Nueva Notificacion!",
            htmlMessage,
            "MercadoPlus Support <support@mercadoplus.xyz>"
        );

        return createdNotification;
    }


    // Eliminar una notificación por ID
    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        var notification = _notificationService.GetById(id);
        if (notification == null)
        {
            return NotFound("Notification not found");
        }

        _notificationService.Delete(id);
        return NoContent();
    }

    // Actualizar una notificación por ID
    [HttpPut("{id}")]
    public ActionResult<NotificationDTO> UpdateNotification(int id, NotificationPutPostDTO notificationToUpdate)
    {
        
        var updatedNotification = _notificationService.Update(id, notificationToUpdate);
        if (updatedNotification == null)
        {
            return NotFound("Notification not found");
        }

        return updatedNotification;
    }
}
