namespace Preventyon.Models
{
    public class Permission
    {
        public int Id { get; set; }
       
        public string PermissionName { get; set; }

        public bool IncidentManagement { get; set; }

        public bool UserManagement { get; set; }


        public bool IncidentCreateOnly { get; set; }

        public Role Role { get; set; }

    }
}
