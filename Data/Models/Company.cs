using System;
using System.Collections.Generic;

namespace Data.Models
{
    public partial class Company
    {
        public Company()
        {
            Recruiters = new HashSet<Recruiter>();
        }

        public string CompanyId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IndustryFieldId { get; set; }
        public string CompanyLogoId { get; set; }
        public string MemorandumOfAgreementId { get; set; }

        public virtual CompanyLogo CompanyLogo { get; set; }
        public virtual IndustryField IndustryField { get; set; }
        public virtual MemorandumOfAgreement MemorandumOfAgreement { get; set; }
        public virtual ICollection<Recruiter> Recruiters { get; set; }
    }
}
