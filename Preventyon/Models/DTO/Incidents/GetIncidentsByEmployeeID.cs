using Preventyon.Models;
namespace Preventyon.Models.DTO.Incidents
{
    public class GetIncidentsByEmployeeID
    {
        public int PrivacyTotalIncidents { get; set; }
        public int PrivacyPendingIncidents { get; set; }
        public int PrivacyClosedIncidents { get; set; }
        public int SecurityTotalIncidents { get; set; }
        public int SecurityPendingIncidents { get; set; }
        public int SecurityClosedIncidents { get; set; }
        public int QualityTotalIncidents { get; set; }
        public int QualityPendingIncidents { get; set; }
        public int QualityClosedIncidents { get; set; }

        public List<TableFetchIncidentsDto> Incidents { get; set; }

        public List<TableFetchIncidentsDto> AssignedIncidents { get; set; }


        public Dictionary<int, Dictionary<string, int>> YearlyIncidentCounts { get; set; }

    }
}
