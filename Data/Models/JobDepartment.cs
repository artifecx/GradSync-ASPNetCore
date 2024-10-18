using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class JobDepartment
{
    public string JobDepartmentId { get; set; }

    public string JobId { get; set; }

    public string DepartmentId { get; set; }

    public virtual Department Department { get; set; }

    public virtual Job Job { get; set; }
}
