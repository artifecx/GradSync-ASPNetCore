using Data.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Services.ServiceModels
{
    public class CompanyViewModel
    {
        public string CompanyId { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(256, ErrorMessage = "Maximum length of an email is 256")]
        [Display(Name = "Email")]
        public string ContactEmail { get; set; }

        [StringLength(256, ErrorMessage = "Maximum length of a contact number is 256")]
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; }

        [Required(ErrorMessage = "Company name is required.")]
        [StringLength(256, ErrorMessage = "Maximum Length of company name is 256")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Maximum Length of company description is 500")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(500, ErrorMessage = "Maximum Length of address is 500")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        public string CompanyLogoId { get; set; }
        public string MemorandumOfAgreementId { get; set; }

        [Display(Name = "Verified")]
        public bool IsVerified { get; set; }

        [Display(Name = "MOA")]
        public bool HasValidMOA { get; set; }

        public CompanyLogo CompanyLogo { get; set; }
        public MemorandumOfAgreement MemorandumOfAgreement { get; set; }

        public List<Recruiter> Recruiters { get; set; }

        [Display(Name = "Active Listings")]
        public int ActiveJobListings { get; set; }

        public string RecruiterId { get; set; }
    }
}
