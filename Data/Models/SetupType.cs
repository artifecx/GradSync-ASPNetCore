﻿using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class SetupType
{
    public string SetupTypeId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();
}