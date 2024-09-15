using System;
using System.Collections.Generic;

namespace Data.Models
{
    public partial class EmploymentType
    {
        public EmploymentType()
        {
            Jobs = new HashSet<Job>();
        }

        public string EmploymentTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Job> Jobs { get; set; }
    }
}
