using Preventyon.Models;
using Preventyon.Models.DTO.Incidents;

namespace Preventyon.Repository.IRepository
{
    public interface IIncidentRepository
    {
        Task<IEnumerable<Incident>> GetAllIncidents();
        Task<GetIncidentsByEmployeeID> GetIncidentsByEmployeeId(int employeeId);
        Task<Incident> GetIncidentById(int id);
        Task<Incident> AddIncident(Incident incident);
        Task<Incident> UpdateIncident(Incident incident, UpdateIncidentDTO updateIncidentDto);
        Task<Incident> UserUpdateIncident(Incident incident, UpdateIncidentUserDto updateIncidentDto);

        Task<Incident> UpdateIncidentAsync(Incident incident);

        Task<GetIncidentsByEmployeeID> GetAllIncidentsWithBarChart();
        Task DeleteIncidentById(int id);
        Task<List<Incident>> UploadIncident(List<Incident> incident);
    }
}
