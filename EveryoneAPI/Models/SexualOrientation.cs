using System;
using System.Collections.Generic;

namespace EveryoneAPI.Models
{
    public partial class SexualOrientation
    {
        public SexualOrientation()
        {
            Employees = new HashSet<Employee>();
        }

        public int OrientationId { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<Employee> Employees { get; set; }
    }
}
