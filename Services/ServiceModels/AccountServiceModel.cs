using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ServiceModels
{
    public class AccountServiceModel
    {
        public string UserId { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(50, ErrorMessage = "Maximum length of an email is 50")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, ErrorMessage = "Maximum length of a name is 50")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(50, ErrorMessage = "Password must be 6 characters minimum", MinimumLength = 6)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirmation Password is required.")]
        [Compare("Password", ErrorMessage = "Password and confirmation password must match.")]
        public string ConfirmPassword { get; set; }

        public bool AsRecruiter { get; set; }
    }
}
