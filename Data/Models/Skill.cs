using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class Skill
{
    public string SkillsId { get; set; }

    public string Name { get; set; }

    public string Type { get; set; }

    public virtual ICollection<Applicant> Applicants { get; set; } = new List<Applicant>();

    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();
}
