using System;
using System.Collections.Generic;

namespace Data.Models
{
    public partial class ApplicationStatusType
    {
        public ApplicationStatusType()
        {
            Applications = new HashSet<Application>();
        }

        public string ApplicationStatusTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Application> Applications { get; set; }
    }
}
