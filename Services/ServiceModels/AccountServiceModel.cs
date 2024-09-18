using System.ComponentModel.DataAnnotations;

namespace Services.ServiceModels
{
    public class AccountServiceModel
    {
        public string UserId { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(100, ErrorMessage = "Maximum length of an email is 100")]
        public string Email { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(100, ErrorMessage = "Maximum Length of first name is 100")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Maximum Length of last name is 100")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [StringLength(100, ErrorMessage = "Maximum Length of middle name is 100")]
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [StringLength(100, ErrorMessage = "Maximum Length of a suffix is 100")]
        public string Suffix { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(50, ErrorMessage = "Password must be 6 characters minimum", MinimumLength = 6)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirmation Password is required.")]
        [Compare("Password", ErrorMessage = "Password and confirmation password must match.")]
        public string ConfirmPassword { get; set; }

        public bool AsRecruiter { get; set; }
    }
}
