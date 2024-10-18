using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class JobApplicantMatch
{
    public string JobApplicantMatchId { get; set; }

    public string UserId { get; set; }

    public string JobId { get; set; }

    public double MatchPercentage { get; set; }

    public virtual Job Job { get; set; }

    public virtual Applicant User { get; set; }
}
