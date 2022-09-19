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
        public string Name { get; set; } = null!;

        public virtual ICollection<Department> Departments { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
    }
}
