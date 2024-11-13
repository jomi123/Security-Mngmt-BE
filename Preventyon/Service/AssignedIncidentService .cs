using Microsoft.AspNetCore.SignalR;
using Preventyon.Hubs;
using Preventyon.Models;
using Preventyon.Repository.IRepository;
using Preventyon.Service.IService;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Preventyon.Service
{
    public class AssignedIncidentService : IAssignedIncidentService
    {
        private readonly IAssignedIncidentRepository _assignedIncidentRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IIncidentRepository _incidentRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IHubContext<IncidentHub> _hubContext;
        public AssignedIncidentService(
            IAssignedIncidentRepository assignedIncidentRepository,
            IEmployeeRepository employeeRepository,
            IIncidentRepository incidentRepository,
            INotificationRepository notificationRepository,
            IHubContext<IncidentHub> hubContext)
        {
            _assignedIncidentRepository = assignedIncidentRepository;
            _employeeRepository = employeeRepository;
            _incidentRepository = incidentRepository;
            _notificationRepository = notificationRepository;
            _hubContext = hubContext;
        }

        public async Task AssignIncidentToEmployeesAsync(int incidentId, AssignIncidentRequest request, bool bulkUpload)
        {

            var incident = await _incidentRepository.GetIncidentById(incidentId);
            if (incident == null)
            {
                throw new KeyNotFoundException("Incident not found");
            }
            if (!bulkUpload)
            {
                incident.IncidentStatus = "progress";
            }
            incident.Remarks = request.Remarks;
            var existingAssignment = await _assignedIncidentRepository.GetAssignmentByIncidentIdAsync(incidentId);
            List<int> updatedEmployeeIds;

            if (existingAssignment != null)
            {
                var existingEmployeeIds = JsonSerializer.Deserialize<List<int>>(existingAssignment.AssignedTo);
                updatedEmployeeIds = existingEmployeeIds.Union(request.AssignedEmployeeIds).ToList();
                await notificationCreate(incident, updatedEmployeeIds);
                existingAssignment.AssignedTo = JsonSerializer.Serialize(updatedEmployeeIds);
                await _assignedIncidentRepository.UpdateAssignmentAsync(existingAssignment);
            }
            else
            {
                updatedEmployeeIds = request.AssignedEmployeeIds.ToList();
                var newAssignment = new AssignedIncidents
                {
                    IncidentId = incidentId,
                    AssignedTo = JsonSerializer.Serialize(updatedEmployeeIds),
                };
                await notificationCreate(incident, updatedEmployeeIds);
                await _assignedIncidentRepository.AddAssignmentAsync(newAssignment);
            }
            await _incidentRepository.UpdateIncidentAsync(incident);
            await _hubContext.Clients.All.SendAsync("ReceiveIncidentUpdate");
        }

        public async Task<List<Incident>> GetAssignedIncidentsForEmployeeAsync(int employeeId)
        {
            var assignments = await _assignedIncidentRepository.GetAssignmentsByEmployeeIdAsync(employeeId);
            var incidentIds = assignments
                .Select(a => a.IncidentId)
                .Distinct()
                .ToList();

            return await _assignedIncidentRepository.GetIncidentsByIdsAsync(employeeId, incidentIds);
        }

        public async Task notificationCreate(Incident incident, List<int> updatedEmployeeIds)
        {
            var incidentDetailsJson = JsonSerializer.Serialize(incident);
            var jsonObject = JsonNode.Parse(incidentDetailsJson);
            jsonObject["Message"] = $"incident assigned";
            incidentDetailsJson = jsonObject.ToJsonString();
            foreach (var employeeId in updatedEmployeeIds)
            {
                var notification = new Notification
                {
                    EmployeeId = employeeId,
                    Message = incidentDetailsJson,
                    CreatedAt = DateTime.UtcNow
                };

                await _notificationRepository.AddNotification(notification);
            }
            await _notificationRepository.SaveChanges();
        }
    }
}
