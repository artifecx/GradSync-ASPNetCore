using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class Skill
{
    public string SkillId { get; set; }

    public string Name { get; set; }

    public string Type { get; set; }

    public virtual ICollection<ApplicantSkill> ApplicantSkills { get; set; } = new List<ApplicantSkill>();

    public virtual ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
}
