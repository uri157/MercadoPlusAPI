public interface INotificationService
{
    NotificationDTO Create(NotificationPostDTO notificationDto);
    void Delete(int id);
    IEnumerable<NotificationDTO> GetAllByUserId(int userId);
    NotificationDTO? GetById(int id);
    NotificationDTO? Update(NotificationPutDTO notificationToUpdate);
}