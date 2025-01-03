﻿using System;
using System.Collections.Generic;

namespace Data.Models;

public partial class User
{
    public string UserId { get; set; }

    public string FirstName { get; set; }

    public string MiddleName { get; set; }

    public string LastName { get; set; }

    public string Suffix { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public string AvatarId { get; set; }

    public string RoleId { get; set; }

    public DateTime JoinDate { get; set; }

    public DateTime? LastLoginDate { get; set; }

    public bool FromSignUp { get; set; }

    public bool IsDeleted { get; set; }

    public bool IsVerified { get; set; }

    public string Token { get; set; }

    public DateTime? TokenExpiry { get; set; }

    public string Preferences { get; set; }

    public virtual Admin Admin { get; set; }

    public virtual Applicant Applicant { get; set; }

    public virtual Avatar Avatar { get; set; }

    public virtual ICollection<MessageParticipant> MessageParticipants { get; set; } = new List<MessageParticipant>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual Recruiter Recruiter { get; set; }

    public virtual Role Role { get; set; }
}
