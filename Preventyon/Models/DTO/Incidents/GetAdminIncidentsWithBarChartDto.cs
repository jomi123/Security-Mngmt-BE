using System.ComponentModel.DataAnnotations.Schema;

namespace Preventyon.Models.DTO.Incidents
{
    public class GetAdminIncidentDTO
    {
        public int Id { get; set; }


        public string IncidentNo { set; get; }


        public string IncidentTitle { set; get; }


        public string IncidentDescription { set; get; }


        public string ReportedBy { set; get; }


        public string RoleOfReporter { set; get; }
        public DateTime IncidentOccuredDate { set; get; }
        public DateTime MonthYear { set; get; }


        public string IncidentType { set; get; }


        public string Category { set; get; }


        public string Priority { set; get; }




    }

    public class GetAdminIncidentsWithBarChartDTO
    {
        public Dictionary<int, Dictionary<string, int>> YearlyIncidentCounts { get; set; }
        public List<GetAdminIncidentDTO> Incidents { get; set; }

    }

}
