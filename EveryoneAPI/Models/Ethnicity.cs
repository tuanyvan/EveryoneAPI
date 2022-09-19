using System;
using System.Collections.Generic;

namespace EveryoneAPI.Models
{
    public partial class Ethnicity
    {
        public Ethnicity()
        {
            Employees = new HashSet<Employee>();
        }

        public int EthnicityId { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<Employee> Employees { get; set; }
    }
}
