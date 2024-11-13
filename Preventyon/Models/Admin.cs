using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Preventyon.Models
{
    public class Admin
    {
    
        public int AdminId { get; set; }

        public int EmployeeId { get; set; }

        public int AssignedBy { get; set; }

        public DateTime AssignedOn { get; set; }

        public Boolean Status { get; set; }
        [JsonIgnore]
        public Employee Employee { get; set; }

        public Admin()
        {
            AssignedOn = DateTime.UtcNow;
        }
    }
}
