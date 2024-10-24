using Services.Interfaces;
using Services.ServiceModels;
using WebApp.Authentication;
using WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using static Resources.Messages.ErrorMessages;
using static Resources.Messages.SuccessMessages;

namespace WebApp.Controllers
{
    /// <summary>
    /// Controller for handling user profiles.
    /// </summary>
    [Authorize]
    [Route("profile")]
    public class UserProfileController : ControllerBase<UserProfileController>
    {
        private readonly IUserProfileService _userProfileService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserProfileController"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="userProfileService">The user profiles service.</param>
        /// <param name="tokenValidationParametersFactory">The token validation parameters factory.</param>
        /// <param name="tokenProviderOptionsFactory">The token provider options factory.</param>
        public UserProfileController(
            IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IMapper mapper,
            IUserProfileService userProfileService,
            TokenValidationParametersFactory tokenValidationParametersFactory,
            TokenProviderOptionsFactory tokenProviderOptionsFactory) : base(httpContextAccessor, loggerFactory, configuration, mapper, userProfileService)
        {
            this._userProfileService = userProfileService;
        }

        /// <summary>
        /// Gets the user profiles.
        /// </summary>
        /// <returns>The user profiles view.</returns>
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetUserProfile()
        {
            var preferences = await _userProfileService.GetUserProfileAsync(UserId);
            return View("ViewProfile", preferences);
        }

        /// <summary>
        /// Updates the user profiles.
        /// </summary>
        /// <param name="model">The user profiles view model.</param>
        /// <returns>A JSON result indicating success or failure.</returns>
        [HttpPost]
        [Route("UpdateUserProfile")]
        public async Task<IActionResult> UpdateUserProfile(UserProfileViewModel model)
        {
            return await HandleExceptionAsync(async () =>
            {
                if (model.UserId != null)
                {
                    await _userProfileService.UpdateUserProfileAsync(model);
                    TempData["SuccessMessage"] = Success_UserProfileUpdate;
                    return Json(new { success = true });
                }
                TempData["ErrorMessage"] = Error_UserPreferenceUpdateDefault;
                return Json(new { success = false });
            }, "UpdateUserProfile");
        }

        /// <summary>
        /// Updates the user password.
        /// </summary>
        /// <param name="model">The user profiles view model.</param>
        /// <returns>A JSON result indicating success or failure.</returns>
        [HttpPost]
        [Route("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword(UserProfileViewModel model)
        {
            return await HandleExceptionAsync(async () =>
            {
                if (!string.IsNullOrEmpty(model.NewPassword) && !string.IsNullOrEmpty(model.OldPassword))
                {
                    model.UserId = UserId;
                    await _userProfileService.UpdateUserPassword(model);
                    TempData["SuccessMessage"] = Success_UserPasswordUpdate;
                    return Json(new { success = true });
                }
                TempData["ErrorMessage"] = Error_UserPasswordUpdateDefault;
                return Json(new { success = false });
            }, "UpdatePassword");
        }
    }
}
