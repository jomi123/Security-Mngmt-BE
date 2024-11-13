using Preventyon.Models;
using Preventyon.Models.DTO.Notification;

namespace Preventyon.Service.IService
{
    public interface INotificationService
    {
        Task AddNotification(Notification notification);
        Task<IEnumerable<Notification>> GetNotificationsForEmployee(int employeeId);
        Task MarkNotificationsAsRead(int employeeId);
        Task MarkAsRead(int employeeId, int notificationId);

        Task<int> GetUnreadNotificationCount(int employeeId);
    }
}
