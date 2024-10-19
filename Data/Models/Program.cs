using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class Program
{
    public string ProgramId { get; set; }

    public string Name { get; set; }

    public string ShortName { get; set; }

    public string DepartmentId { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Department Department { get; set; }

    public virtual ICollection<EducationalDetail> EducationalDetails { get; set; } = new List<EducationalDetail>();

    public virtual ICollection<JobProgram> JobPrograms { get; set; } = new List<JobProgram>();
}
