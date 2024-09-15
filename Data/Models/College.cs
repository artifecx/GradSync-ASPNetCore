using System;
using System.Collections.Generic;

namespace Data.Models
{
    public partial class College
    {
        public College()
        {
            Departments = new HashSet<Department>();
        }

        public string CollegeId { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<Department> Departments { get; set; }
    }
}
