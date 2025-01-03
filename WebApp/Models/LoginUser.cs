﻿using Data.Models;
using static Resources.Constants.Enums;

namespace WebApp.Models
{
    /// <summary>
    /// Login User Model
    /// </summary>
    public class LoginUser
    {
        /// <summary>
        /// Login Result
        /// </summary>
        public LoginResult loginResult { get; set; }
        /// <summary>
        /// Message
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// Access Token
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// Expires In
        /// </summary>
        public int expires_in { get; set; }
        /// <summary>
        /// User Data
        /// </summary>
        public User userData { get; set; }
    }
}
