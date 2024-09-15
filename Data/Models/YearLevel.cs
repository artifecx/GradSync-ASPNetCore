using System;
using System.Collections.Generic;

namespace Data.Models
{
    public partial class YearLevel
    {
        public YearLevel()
        {
            Jobs = new HashSet<Job>();
        }

        public string YearLevelId { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }

        public virtual ICollection<Job> Jobs { get; set; }
    }
}
