namespace Preventyon.Models.DTO.AdminDTO
{
    public class UpdateAdminDTO
    {
        public int AdminId { get; set; }

        public Boolean isIncidentMangenet { get; set; }
        public Boolean isUserMangenet { get; set; }
        public Boolean Status { get; set; }
    }
}
