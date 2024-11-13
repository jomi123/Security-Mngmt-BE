using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Preventyon.Data;
using Preventyon.Models;
using Preventyon.Models.DTO.Incidents;
using Preventyon.Repository.IRepository;

namespace Preventyon.Repository
{
    public class IncidentRepository : IIncidentRepository
    {
        private readonly ApiContext _context;
        private readonly IMapper _mapper;

        public IncidentRepository(ApiContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Incident>> GetAllIncidents()
        {
            return await _context.Incident.ToListAsync();
        }

        public async Task<GetIncidentsByEmployeeID> GetIncidentsByEmployeeId(int employeeId)
        {
            var incidents = await _context.Incident
                .Where(i => i.EmployeeId == employeeId)
                .ToListAsync();

            var privacyIncidents = incidents.Where(i => i.IncidentType == "Privacy Incidents" && !i.IsDraft).ToList();
            var qualityIncidents = incidents.Where(i => i.IncidentType == "Quality Incidents" && !i.IsDraft).ToList();
            var securityIncidents = incidents.Where(i => i.IncidentType == "Security Incidents" && !i.IsDraft).ToList();

            int totalPrivacyIncidents = privacyIncidents.Count;
            int closedPrivacyIncidents = privacyIncidents.Count(i => i.IncidentStatus == "closed");
            int pendingPrivacyIncidents = totalPrivacyIncidents - closedPrivacyIncidents;

            int totalQualityIncidents = qualityIncidents.Count;
            int closedQualityIncidents = qualityIncidents.Count(i => i.IncidentStatus == "closed");
            int pendingQualityIncidents = totalQualityIncidents - closedQualityIncidents;

            int totalSecurityIncidents = securityIncidents.Count;
            int closedSecurityIncidents = securityIncidents.Count(i => i.IncidentStatus == "closed");
            int pendingSecurityIncidents = totalSecurityIncidents - closedSecurityIncidents;
            var allincidents = await _context.Incident
                            .Where(i => i.IsDraft == false)
                            .ToListAsync();
            foreach (var incident in allincidents)
            {
                incident.IsCorrectionFilled = (!string.IsNullOrEmpty(incident.Correction)) && (!string.IsNullOrEmpty(incident.CorrectiveAction));
            }
            var incidentStats = new GetIncidentsByEmployeeID
            {
                PrivacyTotalIncidents = totalPrivacyIncidents,
                PrivacyPendingIncidents = pendingPrivacyIncidents,
                PrivacyClosedIncidents = closedPrivacyIncidents,
                QualityTotalIncidents = totalQualityIncidents,
                QualityPendingIncidents = pendingQualityIncidents,
                QualityClosedIncidents = closedQualityIncidents,
                SecurityTotalIncidents = totalSecurityIncidents,
                SecurityPendingIncidents = pendingSecurityIncidents,
                SecurityClosedIncidents = closedSecurityIncidents,
                Incidents = _mapper.Map<List<TableFetchIncidentsDto>>(incidents),
            };

            return incidentStats;
        }

        public async Task<Incident> GetIncidentById(int id)
        {
            return await _context.Incident.FindAsync(id);
        }

        public async Task<Incident> AddIncident(Incident incident)
        {
            await _context.Incident.AddAsync(incident);
            await _context.SaveChangesAsync();
            return incident;
        }

        public async Task<Incident> UpdateIncident(Incident incident, UpdateIncidentDTO updateIncidentDto)
        {
            _mapper.Map(updateIncidentDto, incident);
            _context.Entry(incident).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return incident;
        }

        public async Task<Incident> UserUpdateIncident(Incident incident, UpdateIncidentUserDto updateIncidentDto)
        {
            _mapper.Map(updateIncidentDto, incident);
            _context.Entry(incident).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return incident;
        }

        public async Task<Incident> UpdateIncidentAsync(Incident incident)
        {

            _context.Entry(incident).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return incident;
        }

        public async Task<GetIncidentsByEmployeeID> GetAllIncidentsWithBarChart()
        {
            var incidents = await _context.Incident
                          .Where(i => i.IsDraft == false)
                          .ToListAsync();

            var privacyIncidents = incidents.Where(i => i.IncidentType == "Privacy Incidents").ToList();
            var qualityIncidents = incidents.Where(i => i.IncidentType == "Quality Incidents").ToList();

            var securityIncidents = incidents.Where(i => i.IncidentType == "Security Incidents").ToList();

            int totalPrivacyIncidents = privacyIncidents.Count;
            int closedPrivacyIncidents = privacyIncidents.Count(i => i.IncidentStatus == "closed");
            int pendingPrivacyIncidents = totalPrivacyIncidents - closedPrivacyIncidents;

            int totalQualityIncidents = qualityIncidents.Count;
            int closedQualityIncidents = qualityIncidents.Count(i => i.IncidentStatus == "closed");
            int pendingQualityIncidents = totalQualityIncidents - closedQualityIncidents;

            int totalSecurityIncidents = securityIncidents.Count;
            int closedSecurityIncidents = securityIncidents.Count(i => i.IncidentStatus == "closed");
            int pendingSecurityIncidents = totalSecurityIncidents - closedSecurityIncidents;

            foreach (var incident in incidents)
            {
                incident.IsCorrectionFilled = (!string.IsNullOrEmpty(incident.Correction)) && (!string.IsNullOrEmpty(incident.CorrectiveAction));
            }
            var fiveYearsAgo = DateTime.Now.AddYears(-5);
            var filteredIncidents = incidents.Where(i => i.IncidentOccuredDate >= fiveYearsAgo).ToList();

            var groupedIncidents = filteredIncidents
                .GroupBy(i => new { Year = i.IncidentOccuredDate.Year, Type = i.IncidentType })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Type = g.Key.Type,
                    Count = g.Count()
                })
                .OrderBy(g => g.Year)
                .ToList();

            var yearlyIncidentCounts = groupedIncidents
            .GroupBy(g => g.Year)
            .ToDictionary(
                g => g.Key,
                g => g.ToDictionary(
                x => x.Type,
                x => x.Count)
            );
            var incidentStats = new GetIncidentsByEmployeeID
            {
                PrivacyTotalIncidents = totalPrivacyIncidents,
                PrivacyPendingIncidents = pendingPrivacyIncidents,
                PrivacyClosedIncidents = closedPrivacyIncidents,
                QualityTotalIncidents = totalQualityIncidents,
                QualityPendingIncidents = pendingQualityIncidents,
                QualityClosedIncidents = closedQualityIncidents,
                SecurityTotalIncidents = totalSecurityIncidents,
                SecurityPendingIncidents = pendingSecurityIncidents,
                SecurityClosedIncidents = closedSecurityIncidents,
                Incidents = _mapper.Map<List<TableFetchIncidentsDto>>(incidents),
                YearlyIncidentCounts = yearlyIncidentCounts,
            };

            return incidentStats;
        }

        public async Task DeleteIncidentById(int id)
        {
            var incident = await _context.Incident.FindAsync(id);
            if (incident == null)
            {
                throw new ArgumentException("Incident not found");

            }

            _context.Incident.Remove(incident);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Incident>> UploadIncident(List<Incident> incidents)
        {
            if (incidents == null || incidents.Count == 0)
            {
                throw new ArgumentException("No incidents to upload.");
            }
            await _context.Incident.AddRangeAsync(incidents);
            await _context.SaveChangesAsync();
            return incidents.ToList();
        }

    }
}
