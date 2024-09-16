using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class CategoryType
{
    public string CategoryTypeId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();
}
