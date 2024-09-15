using System;
using System.Collections.Generic;

namespace Data.Models
{
    public partial class EducationalDetail
    {
        public EducationalDetail()
        {
            Applicants = new HashSet<Applicant>();
        }

        public string EducationalDetailsId { get; set; }
        public string IdNumber { get; set; }
        public string DepartmentId { get; set; }
        public string YearLevelId { get; set; }
        public bool IsGraduate { get; set; }

        public virtual ICollection<Applicant> Applicants { get; set; }
    }
}
