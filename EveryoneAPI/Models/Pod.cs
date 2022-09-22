using System;
using System.Collections.Generic;

namespace EveryoneAPI.Models
{
    public partial class Pod
    {
        public Pod()
        {
            Employees = new HashSet<Employee>();
        }

        public int PodId { get; set; }
        public string Name { get; set; } = null!;
        public int DepartmentId { get; set; }

        public virtual Department Department { get; set; } = null!;
        public virtual ICollection<Employee> Employees { get; set; }
    }

    public partial class PodFormModel
    {
        public string Name { get; set; }
        public int DepartmentId { get; set; }
    }
    
    public partial class PodEditModel
    {
        public string Name { get; set; }
    }

}
