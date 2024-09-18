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

    public string DepartmentId { get; set; }

    public string CategoryTypeId { get; set; }

    public string EmploymentTypeId { get; set; }

    public string StatusTypeId { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int AvailableSlots { get; set; }

    public string Salary { get; set; }

    public string ScheduleId { get; set; }

    public string PostedById { get; set; }

    public bool IsArchived { get; set; }

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual CategoryType CategoryType { get; set; }

    public virtual Department Department { get; set; }

    public virtual EmploymentType EmploymentType { get; set; }

    public virtual User PostedBy { get; set; }

    public virtual ICollection<SavedJob> SavedJobs { get; set; } = new List<SavedJob>();

    public virtual Schedule Schedule { get; set; }

    public virtual StatusType StatusType { get; set; }

    public virtual YearLevel YearLevel { get; set; }

    public virtual ICollection<Skill> Skills { get; set; } = new List<Skill>();
}
