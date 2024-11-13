using System.ComponentModel.DataAnnotations;

namespace Preventyon.Models.DTO.Employee
{
    public class GetEmployeesDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }

        public string Designation { get; set; }
       
    }
}
