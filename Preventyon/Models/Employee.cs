using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Preventyon.Models
{
    public class Employee
    {

        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string Department { get; set; }

        [Required]
        [StringLength(100)]
 
        public int RoleId { get; set; }

        public Role Role { get; set; }

        public string Designation { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<Incident> Incident { get; set; }

        public Admin Admin { get; set; }
        public Employee()
        {
            CreatedAt= DateTime.Now;
        }
    }
}
