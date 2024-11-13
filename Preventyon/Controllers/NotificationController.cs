using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Preventyon.Models;
using Preventyon.Service.IService;

namespace Preventyon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // GET: api/notifications/{employeeId}
        [HttpGet("{employeeId}")]
        [Authorize(Policy = "AllowAll")]//all
        public async Task<ActionResult<IEnumerable<Notification>>> GetNotifications(int employeeId)
        {
            var notifications = await _notificationService.GetNotificationsForEmployee(employeeId);
            if (notifications == null || !notifications.Any())
            {
                return NotFound("No notifications found.");
            }
            return Ok(notifications);
        }

        // GET: api/notifications/count/{employeeId}
        [HttpGet("unread/count/{employeeId}")]
        [Authorize(Policy = "AllowAll")]//all
        public async Task<ActionResult<int>> GetUnreadNotificationsCount(int employeeId)
        {
            var count = await _notificationService.GetUnreadNotificationCount(employeeId);
            return Ok(count);
        }

        // PUT: api/notifications/{employeeId}
        [HttpPut("{employeeId}")]
        [Authorize(Policy = "AllowAll")]//all
        public async Task<IActionResult> MarkAllAsRead(int employeeId)
        {
            await _notificationService.MarkNotificationsAsRead(employeeId);
            return Ok();
        }

        // PUT: api/notifications/{employeeId}/{notificationId}
        [HttpPut("{employeeId}/{notificationId}")]
        [Authorize(Policy = "AllowAll")]//all
        public async Task<IActionResult> MarkAsRead(int employeeId, int notificationId)
        {
            try
            {
                await _notificationService.MarkAsRead(employeeId, notificationId);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
