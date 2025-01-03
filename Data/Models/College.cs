﻿using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class College
{
    public string CollegeId { get; set; }

    public string Name { get; set; }

    public string ShortName { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();

    public virtual ICollection<EducationalDetail> EducationalDetails { get; set; } = new List<EducationalDetail>();
}
