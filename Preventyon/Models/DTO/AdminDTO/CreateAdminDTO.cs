namespace Preventyon.Models.DTO.AdminDTO
{
    public class CreateAdminDTO
    {
        public int EmployeeId { get; set; }

        public int AssignedBy { get; set; }

        public Boolean isIncidentMangenet { get; set; }
        public Boolean isUserMangenet { get; set; }
        public Boolean Status { get; set; }
    }
}
