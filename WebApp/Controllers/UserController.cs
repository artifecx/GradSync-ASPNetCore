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
using static Resources.Messages.SuccessMessages;
using static Resources.Messages.ErrorMessages;
using static Services.Exceptions.UserExceptions;
using System;
using System.Linq;
using System.Security.Claims;
using Services.Services;

namespace WebApp.Controllers
{
    /// <summary>
    /// Controller for handling user-related operations.
    /// </summary>
    [Route("users")]
    [Authorize(Policy = "Admin")]
    public class UserController : ControllerBase<UserController>
    {
        private readonly IUserService _userService;
        private readonly IAvatarService _avatarService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="userService">The team service.</param>
        /// <param name="tokenValidationParametersFactory">The token validation parameters factory.</param>
        /// <param name="tokenProviderOptionsFactory">The token provider options factory.</param>
        public UserController
            (
                IHttpContextAccessor httpContextAccessor,
                ILoggerFactory loggerFactory,
                IConfiguration configuration,
                IMapper mapper,
                IUserService userService,
                IAvatarService avatarService, // Add this line
                TokenValidationParametersFactory tokenValidationParametersFactory,
                TokenProviderOptionsFactory tokenProviderOptionsFactory
            ) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _userService = userService;
            _avatarService = avatarService;
        }

        #region GET Methods 
        /// <summary>
        /// Show all users.
        /// </summary>
        /// <param name="sortBy">The sort by option.</param>
        /// <param name="search">The search filter.</param>
        /// <param name="role">The role filter.</param>
        /// <param name="verified">The verified status filter.</param>
        /// <param name="pageIndex">The page index.</param>
        /// <returns>A task that represents the asynchronous operation. 
        /// The task result contains an <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [Route("all")]
        public async Task<IActionResult> GetAllUsers(string sortBy, string search, string role, bool? verified, int pageIndex = 1)
        {
            return await HandleExceptionAsync(async () =>
            {
                var users = await _userService.GetAllUsersAsync(sortBy, search, role, verified, pageIndex, 10);

                ViewData["Search"] = search;
                ViewData["SortBy"] = sortBy;
                ViewData["Role"] = role;
                ViewData["Verified"] = verified;
                ViewBag.Roles = (await _userService.GetRolesAsync());

                return View("Index", users);
            }, "GetAllUsers");
        }
        #endregion GET Methods

        #region POST Methods        
        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="model">The user view model.</param>
        /// <returns>A task that represents the asynchronous operation. 
        /// The task result contains an <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            return await HandleExceptionAsync(async () =>
            {
                if (ModelState.IsValid)
                {
                    await _userService.AddUserAsync(model);
                    TempData["SuccessMessage"] = string.Format(Success_UserActionSuccess, "added");
                    return Json(new { success = true });
                }
                TempData["ErrorMessage"] = string.Format(Error_UserActionError, "adding");
                return Json(new { success = false });
            }, "Create");
        }

        /// <summary>
        /// Updates the selected user.
        /// </summary>
        /// <param name="model">The user view model.</param>
        /// <returns>A task that represents the asynchronous operation. 
        /// The task result contains an <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Route("update")]
        [AllowAnonymous]
        public async Task<IActionResult> Update(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Log model state for debugging
                    Console.WriteLine($"Updating user: {model.UserId}");
                    Console.WriteLine($"AvatarFile: {model.AvatarFile?.FileName}");

                    // Update user information
                    await _userService.UpdateUserAsync(model);

                    // Handle avatar upload
                    if (model.AvatarFile != null && model.AvatarFile.Length > 0)
                    {
                        // Log the avatar upload process
                        Console.WriteLine($"Uploading avatar for user: {model.UserId}");
                        await _avatarService.UploadAvatarAsync(model.AvatarFile, model.UserId);
                    }

                    TempData["SuccessMessage"] = "User  updated successfully.";
                    return Json(new { success = true });
                }
                catch (Exception ex)
                {
                    // Log the exception for debugging
                    Console.WriteLine($"Error updating user: {ex.Message}");
                    TempData["ErrorMessage"] = "An error occurred while updating the user.";
                    return Json(new { success = false });
                }
            }

            TempData["ErrorMessage"] = "Error updating user.";
            return Json(new { success = false });
        }
        /// <summary>
        /// Resets a user's password through the admin panel.
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <returns>A task that represents the asynchronous operation. 
        /// The task result contains an <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Route("resetpassword")]
        public async Task<IActionResult> ResetPassword(string id)
        {
            return await HandleExceptionAsync(async () =>
            {
                if (ModelState.IsValid)
                {
                    await _userService.ResetUserPasswordAsync(id);
                    TempData["SuccessMessage"] = Success_UserPasswordReset;
                    return Json(new { success = true });
                }
                TempData["ErrorMessage"] = Error_UserPasswordResetDefault;
                return Json(new { success = false });
            }, "ResetPassword");
        }

        /// <summary>
        /// Deletes the selected user.
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <returns>A task that represents the asynchronous operation. 
        /// The task result contains an <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> Delete(string id)
        {
            return await HandleExceptionAsync(async () =>
            {
                if (ModelState.IsValid)
                {
                    await _userService.DeleteUserAsync(id);
                    TempData["SuccessMessage"] = string.Format(Success_UserActionSuccess, "deleted");
                    return Json(new { success = true });
                }
                TempData["ErrorMessage"] = TempData["ErrorMessage"] = string.Format(Error_UserActionError, "deleting");
                return Json(new { success = false });
            }, "Delete");
        }


        #endregion POST Methods
    }
}
