using System.Collections.Generic;
using System.Linq;

public class NotificationDbService : INotificationService
{
    private readonly DbContext _context;

    public NotificationDbService(DbContext context)
    {
        _context = context;
    }

    public NotificationDTO Create(NotificationPutPostDTO notificationDto)
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
        if (notification != null)
        {
            _context.Notifications.Remove(notification);
            _context.SaveChanges();
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

    public NotificationDTO? Update(int id, NotificationPutPostDTO notificationToUpdate)
    {
        var notification = _context.Notifications.Find(id);
        if (notification == null) return null;

        notification.Text = notificationToUpdate.Text;
        _context.SaveChanges();

        return new NotificationDTO
        {
            Id = notification.Id,
            Text = notification.Text
        };
    }
}
