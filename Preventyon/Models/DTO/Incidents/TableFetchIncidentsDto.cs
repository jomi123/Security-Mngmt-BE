namespace Preventyon.Models.DTO.Incidents
{
    public class TableFetchIncidentsDto
    {
        public int Id { get; set; }

        public string IncidentNo { set; get; }
        public string IncidentTitle { get; set; }
        public string IncidentType { get; set; }
        public string Category { get; set; }

        public DateTime IncidentOccuredDate { set; get; }
        public string ReportedBy { get; set; }
        public string Priority { get; set; }

       
        public string IncidentStatus { set; get; }

        public bool IsSubmittedForReview { get; set; }
        public bool IsCorrectionFilled { get; set; }

        public int? Accepted { get; set; }
        public bool IsDraft { get; set; }

        public DateTime createdAt { set; get; }
    }
}
