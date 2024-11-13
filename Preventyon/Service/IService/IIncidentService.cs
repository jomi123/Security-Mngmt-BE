using Preventyon.Models;
using Preventyon.Models.DTO.Incidents;

namespace Preventyon.Service.IService
{
    public interface IIncidentService
    {
        Task<IEnumerable<Incident>> GetAllIncidents();
        Task<GetIncidentsByEmployeeID> GetIncidentsByEmployeeId(int employeeId);
        Task<Incident> GetIncidentById(int id);
        Task<Incident> CreateIncident(CreateIncidentDTO createIncidentDto);
        Task UpdateIncident(int id, UpdateIncidentDTO updateIncidentDto);
        Task UserUpdateIncident(int id, UpdateIncidentUserDto updateIncidentDto);

        Task<GetUserUpdateIncidentDTO> GetUserUpdateIncident(int id);

        Task<GetIncidentsByEmployeeID> GetIncidentsAdmins();
        Task DeleteIncidentById(int id);
        Task UploadIncident(IFormFile file);
    }
}
