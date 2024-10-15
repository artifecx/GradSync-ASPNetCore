using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models
{
    public class JobDropdownPopulationModel
    {
        public List<EmploymentType> EmploymentTypes { get; set; }
        public List<SetupType> SetupTypes { get; set; }
    }
}
