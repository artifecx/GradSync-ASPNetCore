using System;
using System.Collections.Generic;

namespace Data.Models
{
    public partial class Applicant
    {
        public string UserId { get; set; }
        public string IdNumber { get; set; }
        public string ResumeId { get; set; }
        public string SkillsId { get; set; }
        public string EducationalDetailsId { get; set; }
        public string JobPreferences { get; set; }

        public virtual EducationalDetail EducationalDetails { get; set; }
        public virtual Resume Resume { get; set; }
        public virtual Skill Skills { get; set; }
        public virtual User User { get; set; }
    }
}
