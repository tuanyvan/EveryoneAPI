using System;
using System.Collections.Generic;

namespace EveryoneAPI.Models
{
    public partial class Employee
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; } = null!;
        public int GenderIdentity { get; set; }
        public int SexualOrientation { get; set; }
        public int Ethnicity { get; set; }
        public int EmployerId { get; set; }
        public int? DepartmentId { get; set; }
        public int? PodId { get; set; }

        public virtual Department? Department { get; set; }
        public virtual Employer Employer { get; set; } = null!;
        public virtual Ethnicity EthnicityNavigation { get; set; } = null!;
        public virtual GenderIdentity GenderIdentityNavigation { get; set; } = null!;
        public virtual Pod? Pod { get; set; }
        public virtual SexualOrientation SexualOrientationNavigation { get; set; } = null!;
    }
}
