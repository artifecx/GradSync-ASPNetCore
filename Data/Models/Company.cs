using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class Company
{
    public string CompanyId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string Address { get; set; }

    public string ContactEmail { get; set; }

    public string ContactNumber { get; set; }

    public string CompanyLogoId { get; set; }

    public string MemorandumOfAgreementId { get; set; }

    public bool IsArchived { get; set; }

    public bool IsVerified { get; set; }

    public virtual CompanyLogo CompanyLogo { get; set; }

    public virtual MemorandumOfAgreement MemorandumOfAgreement { get; set; }

    public virtual Recruiter Recruiter { get; set; }
}
