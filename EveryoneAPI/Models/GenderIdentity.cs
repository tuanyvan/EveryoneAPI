using System;
using System.Collections.Generic;

namespace EveryoneAPI.Models
{
    public partial class GenderIdentity
    {
        public GenderIdentity()
        {
            Employees = new HashSet<Employee>();
        }

        public int GenderId { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<Employee> Employees { get; set; }
    }
}
