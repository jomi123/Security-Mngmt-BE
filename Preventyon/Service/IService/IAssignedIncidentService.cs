using Preventyon.Models;

namespace Preventyon.Service.IService
{
    public interface IAssignedIncidentService
    {
        Task AssignIncidentToEmployeesAsync(int incidentId, AssignIncidentRequest request, bool bulkUpload);
        Task<List<Incident>> GetAssignedIncidentsForEmployeeAsync(int employeeId);
    }
}
