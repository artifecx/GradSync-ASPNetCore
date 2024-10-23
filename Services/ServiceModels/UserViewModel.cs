using Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Services.ServiceModels
{
    public class UserViewModel
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(100, ErrorMessage = "Maximum Length of first name is 100")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(100, ErrorMessage = "Maximum Length of last name is 100")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [StringLength(100, ErrorMessage = "Maximum Length of middle name is 100")]
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [StringLength(100, ErrorMessage = "Maximum Length of a suffix is 100")]
        public string Suffix { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [StringLength(100, ErrorMessage ="Maximum Length of an email is 100")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the role identifier.
        /// </summary>
        [Required(ErrorMessage = "RoleId is required.")]
        [Display(Name = "Role")]
        public string RoleId { get; set; }

        [Display(Name = "Is Verified")]
        public bool IsVerified { get; set; }

        /// <summary>
        /// Gets or sets the avatar identifier.
        /// </summary>
        [Display(Name = "AvatarId")]
        public string AvatarId { get; set; } // Add this line


        /// <summary>
        /// Gets or sets the roles.
        /// </summary>
        public List<Role> Roles { get; set; }
    }
}
