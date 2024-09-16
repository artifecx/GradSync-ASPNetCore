using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class Schedule
{
    public string ScheduleId { get; set; }

    public string Days { get; set; }

    public string Hours { get; set; }

    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();
}
