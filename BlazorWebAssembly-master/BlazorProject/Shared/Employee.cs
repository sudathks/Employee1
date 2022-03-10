using BlazorProject.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorProject.Shared
{
    public class Employee
    {
        public Guid EmployeeId { get; set; }
        [Required]
        [MinLength(2, ErrorMessage = "Minimum 2 characters required")]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [EmailAddress]
        //[AllowedEmailDomainAttribute]
        public string Email { get; set; }
        public DateTime? DateOfBrith { get; set; }
        public Gender Gender { get; set; }
        public int DepartmentId { get; set; }
        //public string PhotoPath { get; set; }
        public Department Department { get; set; }
        [Required]
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}
