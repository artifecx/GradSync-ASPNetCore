﻿using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class ApplicantSkill
{
    public string ApplicantSkillsId { get; set; }

    public string UserId { get; set; }

    public string SkillId { get; set; }

    public virtual Skill Skill { get; set; }

    public virtual Applicant User { get; set; }
}