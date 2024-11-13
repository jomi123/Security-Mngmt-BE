using Microsoft.EntityFrameworkCore;
using Preventyon.Data;
using Preventyon.Models;
using Preventyon.Repository.IRepository;

namespace Preventyon.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApiContext _context;

        public NotificationRepository(ApiContext context)
        {
            _context = context;
        }

        public async Task AddNotification(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
        }

        public async Task<IEnumerable<Notification>> GetNotificationsForEmployee(int employeeId)
        {
            return await _context.Notifications
                .Where(n => n.EmployeeId == employeeId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkNotificationsAsRead(int employeeId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.EmployeeId == employeeId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                _context.Notifications.Update(notification);
            }
            await _context.SaveChangesAsync();
        }
        public async Task MarkAsRead(int employeeId, int notificationId)
        {
            var notification = await _context.Notifications.FirstOrDefaultAsync(e => e.EmployeeId == employeeId && e.Id == notificationId);
            if (notification == null)
            {
                throw new Exception("Notification not found");
            }
            notification.IsRead = true;
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
        }
        public async Task<int> GetUnreadNotificationCount(int employeeId)
        {
            return await _context.Notifications
                .Where(n => n.EmployeeId == employeeId && !n.IsRead)
                .CountAsync();
        }
        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }
    }
}
