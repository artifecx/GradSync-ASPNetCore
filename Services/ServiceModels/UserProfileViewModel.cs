using Data.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Services.ServiceModels
{
    public class UserProfileViewModel
    {
        public string UserId { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string Suffix { get; set; }
        [Required]
        public string Email { get; set; }
        public Dictionary<string, string> Preferences { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }


        public Applicant Applicant { get; set; }
        public Recruiter Recruiter { get; set; }
    }
}
