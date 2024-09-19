using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class Recruiter
{
    public string UserId { get; set; }

    public string Title { get; set; }

    public string CompanyId { get; set; }

    public virtual Company Company { get; set; }

    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();

    public virtual User User { get; set; }
}
