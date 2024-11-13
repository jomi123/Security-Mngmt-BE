namespace Preventyon.Models
{
    public class AssignIncidentRequest
    {
        public List<int> AssignedEmployeeIds { get; set; }
        public string Remarks { get; set; }
    }
}
