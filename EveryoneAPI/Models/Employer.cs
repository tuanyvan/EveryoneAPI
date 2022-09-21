using System;
using System.Collections.Generic;

namespace EveryoneAPI.Models
{
    public partial class Employer
    {
        public Employer()
        {
            Departments = new HashSet<Department>();
            Employees = new HashSet<Employee>();
        }

        public int EmployerId { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Uuid { get; set; } = null!;

        public partial class EmployerInfo
        {
            public string Email { get; set; } = null!;
            public string Password { get; set; } = null!;
        }

        public virtual ICollection<Department> Departments { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
    }
}
