using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class Department
{
    public string DepartmentId { get; set; }

    public string Name { get; set; }

    public string ShortName { get; set; }

    public string CollegeId { get; set; }

    public bool IsDeleted { get; set; }

    public virtual College College { get; set; }

    public virtual ICollection<EducationalDetail> EducationalDetails { get; set; } = new List<EducationalDetail>();

    public virtual ICollection<JobDepartment> JobDepartments { get; set; } = new List<JobDepartment>();
}
