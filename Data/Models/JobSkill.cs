using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class JobSkill
{
    public string JobSkillId { get; set; }

    public string JobId { get; set; }

    public string SkillId { get; set; }

    public virtual Job Job { get; set; }

    public virtual Skill Skill { get; set; }
}
