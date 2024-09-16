using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class IndustryField
{
    public string IndustryFieldId { get; set; }

    public string Name { get; set; }

    public virtual ICollection<Company> Companies { get; set; } = new List<Company>();

    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();
}
