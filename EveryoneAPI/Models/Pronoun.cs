using System;
using System.Collections.Generic;

namespace EveryoneAPI.Models
{
    public partial class Pronoun
    {
        public Pronoun()
        {
            Employees = new HashSet<Employee>();
        }

        public int PronounId { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<Employee> Employees { get; set; }
    }
}
