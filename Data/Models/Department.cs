using System;
using System.Collections.Generic;

namespace Data.Models
{
    public partial class Department
    {
        public Department()
        {
            Jobs = new HashSet<Job>();
        }

        public string DepartmentId { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string CollegeId { get; set; }

        public virtual College College { get; set; }
        public virtual ICollection<Job> Jobs { get; set; }
    }
}
