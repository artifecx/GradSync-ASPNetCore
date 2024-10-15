using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class YearLevel
{
    public string YearLevelId { get; set; }

    public string Name { get; set; }

    public int Year { get; set; }

    public virtual ICollection<EducationalDetail> EducationalDetails { get; set; } = new List<EducationalDetail>();

    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();
}
