using System;
using System.Collections.Generic;

namespace Data.Models
{
    public partial class StatusType
    {
        public StatusType()
        {
            Jobs = new HashSet<Job>();
        }

        public string StatusTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Job> Jobs { get; set; }
    }
}
