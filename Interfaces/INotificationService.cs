public interface INotificationService
{
    NotificationDTO Create(NotificationPutPostDTO notificationDto);
    void Delete(int id);
    IEnumerable<NotificationDTO> GetAllByUserId(int userId);
    NotificationDTO? GetById(int id);
    NotificationDTO? Update(int id, NotificationPutPostDTO notificationToUpdate);
}