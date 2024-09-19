using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class SavedJob
{
    public string SavedJobId { get; set; }

    public DateTime SaveDate { get; set; }

    public string UserId { get; set; }

    public string JobId { get; set; }

    public virtual Job Job { get; set; }

    public virtual Applicant User { get; set; }
}
