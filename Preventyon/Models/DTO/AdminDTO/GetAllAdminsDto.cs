namespace Preventyon.Models.DTO.AdminDTO
{
    public class GetAllAdminsDto
    {
        public int AdminId { get; set; }
        public string EmployeeName { get; set; }

        public DateTime AssignedOn { get; set; }
        public string AssignedBy { get; set; }

        public Boolean isIncidentMangenet { get; set; }
        public Boolean isUserMangenet { get; set; }
        public Boolean Status { get; set; }
    }
}
