using System;
using System.Collections.Generic;

namespace EveryoneAPI.Models
{
    public partial class Department
    {
        public Department()
        {
            Employees = new HashSet<Employee>();
            Pods = new HashSet<Pod>();
        }

        public int DepartmentId { get; set; }
        public string Name { get; set; } = null!;
        public int EmployerId { get; set; }

        public virtual Employer Employer { get; set; } = null!;
        public virtual ICollection<Employee> Employees { get; set; }
        public virtual ICollection<Pod> Pods { get; set; }
    }

    public partial class DepartmentCRUDModel
    {
        public string Name { get; set; } = null!;
    }

}
