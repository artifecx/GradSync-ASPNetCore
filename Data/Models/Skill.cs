using System;
using System.Collections.Generic;

namespace Data.Models
{
    public partial class Skill
    {
        public Skill()
        {
            Applicants = new HashSet<Applicant>();
            Jobs = new HashSet<Job>();
        }

        public string SkillsId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        public virtual ICollection<Applicant> Applicants { get; set; }
        public virtual ICollection<Job> Jobs { get; set; }
    }
}
