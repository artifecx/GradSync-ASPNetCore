using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class Applicant
{
    public string UserId { get; set; }

    public string IdNumber { get; set; }

    public string ResumeId { get; set; }

    public string EducationalDetailId { get; set; }

    public string JobPreferences { get; set; }

    public string Address { get; set; }

    public string EmbeddingJson { get; set; }

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual EducationalDetail EducationalDetail { get; set; }

    public virtual ICollection<JobApplicantMatch> JobApplicantMatches { get; set; } = new List<JobApplicantMatch>();

    public virtual Resume Resume { get; set; }

    public virtual ICollection<SavedJob> SavedJobs { get; set; } = new List<SavedJob>();

    public virtual User User { get; set; }

    public virtual ICollection<Skill> Skills { get; set; } = new List<Skill>();
}
