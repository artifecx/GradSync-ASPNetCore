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
using Microsoft.IdentityModel.Tokens;

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
                TokenValidationParametersFactory tokenValidationParametersFactory,
                TokenProviderOptionsFactory tokenProviderOptionsFactory
            ) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _userService = userService;
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
                    TempData["SuccessMessage"] = "Successfully added user!";
                    return Json(new { success = true });
                }
                TempData["ErrorMessage"] = "An error occurred while adding user.";
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
        public async Task<IActionResult> Update(UserViewModel model)
        {
            return await HandleExceptionAsync(async () =>
            {
                if (ModelState.IsValid)
                {
                    await _userService.UpdateUserAsync(model);
                    TempData["SuccessMessage"] = "User updated successfully!";
                    return Json(new { success = true });
                }
                TempData["ErrorMessage"] = "An error occurred while updating user.";
                return Json(new { success = false });
            }, "Update");
        }

        /// <summary>
        /// Resets a user's password
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
                if (!id.IsNullOrEmpty())
                {
                    await _userService.ResetUserPasswordAsync(id);
                    TempData["SuccessMessage"] = "Password reset successfully!";
                    return Json(new { success = true });
                }
                TempData["ErrorMessage"] = "An error occurred while resetting password.";
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
                    TempData["SuccessMessage"] = "User deleted successfully!";
                    return Json(new { success = true });
                }
                TempData["ErrorMessage"] = "An error has occured while deleting user.";
                return Json(new { success = false });
            }, "Delete");
        }
        #endregion POST Methods
    }
}
