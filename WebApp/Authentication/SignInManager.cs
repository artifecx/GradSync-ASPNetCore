﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using WebApp.Extensions.Configuration;
using WebApp.Models;
using Resources.Constants;
using Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using static Resources.Constants.Enums;
using Data.Interfaces;

namespace WebApp.Authentication
{
    /// <summary>
    /// SignInManager
    /// </summary>
    public class SignInManager
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        public LoginUser user { get; set; }

        /// <summary>
        /// Initializes a new instance of the SignInManager class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="accountService">The account service.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        public SignInManager(IConfiguration configuration,
                             IHttpContextAccessor httpContextAccessor, IUserRepository userRepository)
        {
            this._configuration = configuration;
            this._httpContextAccessor = httpContextAccessor;
            this._userRepository = userRepository;

            user = new LoginUser();
        }

        /// <summary>
        /// Gets the claims identity.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>The successfully completed task</returns>
        public Task<ClaimsIdentity> GetClaimsIdentity(string username, string password)
        {
            ClaimsIdentity claimsIdentity = null;
            User userData = new User();

            user.loginResult = LoginResult.Success;

            if (user.loginResult == LoginResult.Failed)
            {
                return Task.FromResult<ClaimsIdentity>(null);
            }

            user.userData = userData;
            bool isSuper = _userRepository.IsSuperAdmin(userData.UserId);
            claimsIdentity = CreateClaimsIdentity(userData, isSuper);
            return Task.FromResult(claimsIdentity);
        }

        /// <summary>
        /// Creates the claims identity.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Instance of ClaimsIdentity</returns>
        public ClaimsIdentity CreateClaimsIdentity(User user, bool isSuper)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId, ClaimValueTypes.String, Const.Issuer),
                new Claim(ClaimTypes.Name, user.FirstName, ClaimValueTypes.String, Const.Issuer),
                new Claim(ClaimTypes.Email, user.Email, ClaimValueTypes.String, Const.Issuer),
                new Claim(ClaimTypes.Role, user.RoleId, ClaimValueTypes.String, Const.Issuer),
                new Claim("UserId", user.UserId, ClaimValueTypes.String, Const.Issuer),
                new Claim("Name", user.FirstName, ClaimValueTypes.String, Const.Issuer),
                new Claim("LastName", user.LastName, ClaimValueTypes.String, Const.Issuer),
                new Claim("Email", user.Email, ClaimValueTypes.String, Const.Issuer),
                new Claim("Role", user.RoleId, ClaimValueTypes.String, Const.Issuer),
                new Claim("IsSuperAdmin", isSuper.ToString(), ClaimValueTypes.Boolean, Const.Issuer),
                new Claim("FromSignUp", user.FromSignUp.ToString(), ClaimValueTypes.Boolean, Const.Issuer)
            };
            return new ClaimsIdentity(claims, Const.AuthenticationScheme);
        }

        /// <summary>
        /// Creates the claims principal.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <returns>Created claims principal</returns>
        public IPrincipal CreateClaimsPrincipal(ClaimsIdentity identity)
        {
            var identities = new List<ClaimsIdentity>();
            identities.Add(identity);
            return this.CreateClaimsPrincipal(identities);
        }

        /// <summary>
        /// Creates the claims principal.
        /// </summary>
        /// <param name="identities">The identities.</param>
        /// <returns>Created claims principal</returns>
        public IPrincipal CreateClaimsPrincipal(IEnumerable<ClaimsIdentity> identities)
        {
            var principal = new ClaimsPrincipal(identities);
            return principal;
        }

        /// <summary>
        /// Signs in user asynchronously
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="isPersistent">if set to <c>true</c> [is persistent].</param>
        public async Task SignInAsync(User user, bool isPersistent = false)
        {
            bool isSuper = _userRepository.IsSuperAdmin(user.UserId);
            var claimsIdentity = this.CreateClaimsIdentity(user, isSuper);
            var principal = this.CreateClaimsPrincipal(claimsIdentity);
            await this.SignInAsync(principal, isPersistent);
        }

        /// <summary>
        /// Signs in user asynchronously
        /// </summary>
        /// <param name="principal">The principal.</param>
        /// <param name="isPersistent">if set to <c>true</c> [is persistent].</param>
        public async Task SignInAsync(IPrincipal principal, bool isPersistent = false)
        {
            var token = _configuration.GetTokenAuthentication();
            await _httpContextAccessor
                .HttpContext
                .SignInAsync(
                            Const.AuthenticationScheme,
                            (ClaimsPrincipal)principal,
                            new AuthenticationProperties
                            {
                                ExpiresUtc = DateTime.Now.AddMinutes(token.ExpirationMinutes),
                                IsPersistent = isPersistent,
                                AllowRefresh = false
                            });
        }

        /// <summary>
        /// Signs out user asynchronously
        /// </summary>
        public async Task SignOutAsync()
        {
            var token = _configuration.GetTokenAuthentication();
            await _httpContextAccessor.HttpContext.SignOutAsync(Const.AuthenticationScheme);
        }
    }
}
