namespace Preventyon.Models.DTO.Incidents
{
    public class CreateIncidentDTO
    {
        public string IncidentTitle { get; set; }
        public string IncidentDescription { get; set; }
        public DateTime IncidentOccuredDate { get; set; }
        public string? IncidentType { get; set; }
        public string? Category { get; set; }
        public string? Priority { get; set; }
        public bool IsDraft { get; set; }
        public int EmployeeId { get; set; }

        public List<IFormFile> DocumentUrls { get; set; }

    }



}
