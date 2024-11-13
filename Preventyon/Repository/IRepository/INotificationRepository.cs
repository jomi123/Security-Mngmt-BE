using Preventyon.Models;

namespace Preventyon.Repository.IRepository
{
    public interface INotificationRepository
    {
        Task AddNotification(Notification notification);
        Task<IEnumerable<Notification>> GetNotificationsForEmployee(int employeeId);
        Task MarkNotificationsAsRead(int employeeId);

        Task<int> GetUnreadNotificationCount(int employeeId);
        Task SaveChanges();
        Task MarkAsRead(int notificationId, int employeeId);
    }
}
