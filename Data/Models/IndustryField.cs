using System;
using System.Collections.Generic;

namespace Data.Models
{
    public partial class IndustryField
    {
        public IndustryField()
        {
            Companies = new HashSet<Company>();
            Jobs = new HashSet<Job>();
        }

        public string IndustryFieldId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Company> Companies { get; set; }
        public virtual ICollection<Job> Jobs { get; set; }
    }
}
