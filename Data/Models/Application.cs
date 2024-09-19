using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class Application
{
    public string ApplicationId { get; set; }

    public string ApplicationStatusTypeId { get; set; }

    public string UserId { get; set; }

    public string JobId { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string AdditionalInformationId { get; set; }

    public bool IsArchived { get; set; }

    public virtual ApplicationStatusType ApplicationStatusType { get; set; }

    public virtual Job Job { get; set; }

    public virtual Applicant User { get; set; }
}
