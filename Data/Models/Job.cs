using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class Job
{
    public string JobId { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public string Location { get; set; }

    public string YearLevelId { get; set; }

    public string SetupTypeId { get; set; }

    public string EmploymentTypeId { get; set; }

    public string StatusTypeId { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime UpdatedDate { get; set; }

    public int AvailableSlots { get; set; }

    public string SalaryLower { get; set; }

    public string SalaryUpper { get; set; }

    public string Schedule { get; set; }

    public string PostedById { get; set; }

    public string CompanyId { get; set; }

    public bool IsArchived { get; set; }

    public string EmbeddingJson { get; set; }

    public decimal? SkillWeights { get; set; }

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual Company Company { get; set; }

    public virtual EmploymentType EmploymentType { get; set; }

    public virtual ICollection<JobApplicantMatch> JobApplicantMatches { get; set; } = new List<JobApplicantMatch>();

    public virtual ICollection<JobProgram> JobPrograms { get; set; } = new List<JobProgram>();

    public virtual ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();

    public virtual Recruiter PostedBy { get; set; }

    public virtual ICollection<SavedJob> SavedJobs { get; set; } = new List<SavedJob>();

    public virtual SetupType SetupType { get; set; }

    public virtual StatusType StatusType { get; set; }

    public virtual YearLevel YearLevel { get; set; }
}
