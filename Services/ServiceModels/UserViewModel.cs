﻿using Data.Models;
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
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, ErrorMessage = "Maximum Length of a name is 50")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [StringLength(50, ErrorMessage ="Maximum Length of an email is 50")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(20, ErrorMessage = "Password must be 5 characters minimum", MinimumLength = 5)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the role identifier.
        /// </summary>
        [Required(ErrorMessage = "RoleId is required.")]
        [Display(Name = "Role")]
        public string RoleId { get; set; }

        /// <summary>
        /// Gets or sets the roles.
        /// </summary>
        public List<Role> Roles { get; set; }
    }
}
