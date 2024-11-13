namespace Preventyon.Models.DTO
{
    public class PermissionDTO
    {
        public int Id { get; set; }
        public string PermissionName { get; set; }

        public Boolean IncidentManagement { get; set; }

        public Boolean UserManagement { get; set; }


        public Boolean IncidentCreateOnly { get; set; }
    }
}
