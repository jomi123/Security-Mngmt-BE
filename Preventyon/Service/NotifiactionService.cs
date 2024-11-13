using AutoMapper;
using Preventyon.Models;
using Preventyon.Repository.IRepository;
using Preventyon.Service.IService;

namespace Preventyon.Service
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task AddNotification(Notification notification)
        {
           
            await _notificationRepository.AddNotification(notification);
            await _notificationRepository.SaveChanges();
        }

        public async Task<IEnumerable<Notification>> GetNotificationsForEmployee(int employeeId)
        {
            return await _notificationRepository.GetNotificationsForEmployee(employeeId);
        }

        public async Task MarkNotificationsAsRead(int employeeId)
        {
            await _notificationRepository.MarkNotificationsAsRead(employeeId);
            await _notificationRepository.SaveChanges();
        }

        public async Task MarkAsRead(int employeeId, int notificationId)
        {
            await _notificationRepository.MarkAsRead(employeeId, notificationId);
            await _notificationRepository.SaveChanges();
        }
        public async Task<int> GetUnreadNotificationCount(int employeeId)
        {
            return await _notificationRepository.GetUnreadNotificationCount(employeeId);
        }
    }

}

