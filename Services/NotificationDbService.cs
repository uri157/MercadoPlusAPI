using System.Collections.Generic;
using System.Linq;

public class NotificationDbService : INotificationService
{
    private readonly DbContext _context;
    private readonly IMailService _mailService;



    public NotificationDbService(DbContext context, IMailService mailService)
    {
        _context = context;
        _mailService = mailService;
    }

    // public NotificationDTO Create(NotificationPostDTO notificationDto)
    // {
    //     var notification = new Notification
    //     {
    //         IdUser = notificationDto.userId,// Asigna el UserId aquí, de acuerdo con el contexto,
    //         Text = notificationDto.Text,
    //         Readed = notificationDto.Readed
    //     };

    //     try
    //     {
    //         _context.Notifications.Add(notification);
    //         _context.SaveChanges();

    //         var htmlMessage = $@"
    //         <html>
    //             <body style='font-family: Arial, sans-serif; text-align: center; background-color: #f9f9f9; padding: 50px;'>
    //                 <p style='font-size: 18px; color: #555;'>{notificationDto.Text}:</p>
    //             </body>
    //         </html>";

    //         _mailService.SendEmailAsync(
    //             _userService.GetById(notificationDto.userId).Email,
    //             "Nueva Notificacion!",
    //             htmlMessage,
    //             "MercadoPlus Support <support@mercadoplus.xyz>"
    //         );

    //         return new NotificationDTO
    //         {
    //             Id = notification.Id,
    //             Text = notification.Text
    //         };
    //     }
    //     catch (Exception ex)
    //     {
    //         throw new Exception("Error al crear la notificación.", ex);
    //     }
    // }
    public NotificationDTO Create(NotificationPostDTO notificationDto)
    {
        var notification = new Notification
        {
            IdUser = notificationDto.userId,// Asigna el UserId aquí, de acuerdo con el contexto,
            Text = notificationDto.Text,
            Readed = notificationDto.Readed
        };
        _context.Notifications.Add(notification);
        _context.SaveChanges();

        return new NotificationDTO
        {
            Id = notification.Id,
            Text = notification.Text
        };
    }

    public void Delete(int id)
    {   
        var notification = _context.Notifications.Find(id);

        if (notification == null)
        {
            throw new Exception("No se encontro la notificacion.");
        }
        try
        {
            _context.Notifications.Remove(notification);
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            throw new Exception("Error al eliminar la notificación.", ex);
        }

    }

    public IEnumerable<NotificationDTO> GetAllByUserId(int userId)
    {
        return _context.Notifications
            .Where(n => n.IdUser == userId)
            .Select(n => new NotificationDTO
            {
                Id = n.Id,
                Text = n.Text
            })
            .ToList();
    }

    public NotificationDTO? GetById(int id)
    {
        var notification = _context.Notifications.Find(id);
        return notification == null ? null : new NotificationDTO
        {
            Id = notification.Id,
            Text = notification.Text
        };
    }

    public NotificationDTO? Update(NotificationPutDTO notificationToUpdate)
    {
        var notification = _context.Notifications.Find(notificationToUpdate.Id);
        if (notification == null)
        {
            throw new Exception("No se encontro la notificacion.");
        }

        notification.Text = notificationToUpdate.Text;
        try{
            _context.SaveChanges();
            return new NotificationDTO
            {
                Id = notification.Id,
                Text = notification.Text
            };
        }
        catch (Exception ex)
        {
            throw new Exception("Error al guardar los cambios en la notificación.", ex);
        }
    }
}
