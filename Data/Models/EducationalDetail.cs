﻿using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class EducationalDetail
{
    public string EducationalDetailId { get; set; }

    public string IdNumber { get; set; }

    public string ProgramId { get; set; }

    public string DepartmentId { get; set; }

    public string CollegeId { get; set; }

    public string YearLevelId { get; set; }

    public bool IsGraduate { get; set; }

    public virtual ICollection<Applicant> Applicants { get; set; } = new List<Applicant>();

    public virtual College College { get; set; }

    public virtual Department Department { get; set; }

    public virtual Program Program { get; set; }

    public virtual YearLevel YearLevel { get; set; }
}
