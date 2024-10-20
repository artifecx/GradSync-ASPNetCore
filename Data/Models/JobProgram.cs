using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class JobProgram
{
    public string JobProgramId { get; set; }

    public string JobId { get; set; }

    public string ProgramId { get; set; }

    public virtual Job Job { get; set; }

    public virtual Program Program { get; set; }
}
