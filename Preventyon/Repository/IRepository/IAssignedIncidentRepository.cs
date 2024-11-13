using Preventyon.Models;

namespace Preventyon.Repository.IRepository
{
    public interface IAssignedIncidentRepository
    {
        Task AddAssignmentAsync(AssignedIncidents assignment);

        Task UpdateAssignmentAsync(AssignedIncidents assignment);
        Task<AssignedIncidents> GetAssignmentByIncidentIdAsync(int incidentId);
        Task<List<AssignedIncidents>> GetAssignmentsByEmployeeIdAsync(int employeeId );
        Task<Incident> GetIncidentByIdAsync(int id);
        Task<List<Incident>> GetIncidentsByIdsAsync(int employeeiD, List<int> ids);
    }
}
