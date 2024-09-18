using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class EducationalDetail
{
    public string EducationalDetailsId { get; set; }

    public string IdNumber { get; set; }

    public string DepartmentId { get; set; }

    public string YearLevelId { get; set; }

    public bool IsGraduate { get; set; }

    public virtual Applicant Applicant { get; set; }
}
