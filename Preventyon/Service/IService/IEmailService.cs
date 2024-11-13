using Preventyon.Models;

namespace Preventyon.Service.IService
{
    public interface IEmailService
    {
        Task<bool> SendNotificationAsync(int employeeId, Incident incident);
    }
}
