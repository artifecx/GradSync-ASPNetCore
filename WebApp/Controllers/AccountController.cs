using Data.Models;
using Services.Interfaces;
using Services.Manager;
using Services.ServiceModels;
using WebApp.Authentication;
using WebApp.Models;
using WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using static Resources.Constants.Enums;
using static Resources.Constants.UserRoles;
using static Resources.Messages.SuccessMessages;
using static Resources.Messages.ErrorMessages;
using static Resources.Constants.Routes;
using Microsoft.AspNetCore.RateLimiting;

namespace WebApp.Controllers
{
    public class AccountController : ControllerBase<AccountController>
    {
        private readonly SessionManager _sessionManager;
        private readonly SignInManager _signInManager;
        private readonly IAccountService _accountService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="userService">The user service.</param>"
        /// <param name="signInManager">The sign in manager.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="mapper">The mapper.</param>
        public AccountController
            (
                IAccountService userService,
                SignInManager signInManager,
                IHttpContextAccessor httpContextAccessor,
                ILoggerFactory loggerFactory,
                IConfiguration configuration,
                IMapper mapper
            ) : 
            base (httpContextAccessor, loggerFactory, configuration, mapper)
        {
            this._sessionManager = new SessionManager(this._session);
            this._signInManager = signInManager;
            this._accountService = userService;
        }

        /// <summary>
        /// Gets the login view.
        /// </summary>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns>A task that represents the asynchronous operation. 
        /// The task result contains an <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessageLogin"] = Error_UserLoginDefault;
                return RedirectToAction(Account_Login, Controller_Account);
            }

            if (User.Identity.IsAuthenticated)
            {
                bool isAdminOrNLO = User.IsInRole(Role_Admin) || User.IsInRole(Role_NLO);
                if (isAdminOrNLO) return RedirectToAction(Home_Dashboard, Controller_Home);

                bool isRecruiter = User.IsInRole(Role_Recruiter);
                return isRecruiter ? 
                    RedirectToAction(Job_RecruiterGetAllJobs, Controller_Job) : 
                    RedirectToAction(Home_Index, Controller_Home);
            }

            return await HandleExceptionAsync(async () =>
            {
                TempData["returnUrl"] = returnUrl;
                this._sessionManager.Clear();
                this._session.SetString("SessionId", System.Guid.NewGuid().ToString());
                return this.View();
            }, "Login");
        }

        /// <summary>
        /// Authenticate user and signs the user in when successful.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns> Created response view </returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            return await HandleExceptionAsync(async () =>
            {
                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrEmpty(model.Username))
                    {
                        TempData["ErrorMessageLogin"] = Error_UserLoginDefault;
                        return View(model);
                    }

                    this._session.SetString("HasSession", "Exist");

                    User user = null;
                    var loginResult = _accountService.AuthenticateUser(model.Email, model.Password, ref user);
                    if (loginResult == LoginResult.Success)
                    {
                        await this._signInManager.SignInAsync(user);
                        this._session.SetString("Email", user.Email);
                        this._session.SetString("UserId", user.UserId);

                        bool isAdminOrNLO = string.Equals(user.RoleId, Role_Admin) || string.Equals(user.RoleId, Role_NLO);
                        if (isAdminOrNLO) return RedirectToAction(Home_Dashboard, Controller_Home);

                        bool isRecruiter = string.Equals(user.RoleId, Role_Recruiter);
                        return isRecruiter ?
                            RedirectToAction(Job_RecruiterGetAllJobs, Controller_Job) :
                            RedirectToAction(Home_Index, Controller_Home);
                    }
                    else
                    {
                        TempData["ErrorMessageLogin"] = Error_UserIncorrectLoginDetails;
                        return View(model);
                    }
                }
                TempData["ErrorMessageLogin"] = Error_UserLoginDefault;
                return View(model);
            }, "Login");
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="model">The account service model containing the required data.</param>
        /// <param name="FormLoadTime">The number of ticks it took to complete and submit the form.</param>
        /// <returns>A task that represents the asynchronous operation. 
        /// The task result contains an <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [AllowAnonymous]
        [EnableRateLimiting("EmailUsePolicy")]
        public async Task<IActionResult> Register(AccountServiceModel model, string FormLoadTime)
        {
            return await HandleExceptionAsync(async () =>
            {
                CheckFormSubmissionTime(FormLoadTime);

                if (ModelState.IsValid && (string.IsNullOrEmpty(model.Username) && string.IsNullOrEmpty(UserId)))
                {
                    _accountService.RegisterUser(model);
                    TempData["SuccessMessage"] = Success_UserRegistrationSuccess;
                    return Json(new { success = true });
                }
                TempData["ErrorMessage"] = Error_UserRegistrationDefault;
                return Json(new { success = false });
            }, "Register");
        }

        /// <summary>
        /// Signs the out user.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. 
        /// The task result contains an <see cref="IActionResult"/>.</returns>
        [AllowAnonymous]
        public async Task<IActionResult> SignOutUser()
        {
            await this._signInManager.SignOutAsync();
            return RedirectToAction(Account_Login, Controller_Account);
        }

        /// <summary>
        /// Sends a reset password request using the provided email
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns>A task that represents the asynchronous operation. 
        /// The task result contains an <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [AllowAnonymous]
        [EnableRateLimiting("EmailUsePolicy")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            return await HandleExceptionAsync(async () =>
            {
                if (ModelState.IsValid)
                {
                    await _accountService.ResetUserPasswordAsync(email);
                    TempData["SuccessMessage"] = Success_UserPasswordRequestSuccess;
                    return Json(new { success = true });
                }
                TempData["ErrorMessage"] = Error_Default;
                return Json(new { success = false });
            }, "ForgotPassword");
        }

        /// <summary>
        /// Verifies the user using a token
        /// </summary>
        /// <param name="token">The token identifier.</param>
        /// <returns>A task that represents the asynchronous operation. 
        /// The task result contains an <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("/verify")]
        public async Task<IActionResult> Verify(string token = null)
        {
            return await HandleExceptionAsync(async () =>
            {
                if (ModelState.IsValid)
                {
                    TempData["ErrorMessageLogin"] = await _accountService.VerifyUserEmail(token);
                    return RedirectToAction(Account_Login);
                }
                TempData["ErrorMessageLogin"] = Error_TokenInvalidDefault;
                return RedirectToAction(Account_Login);
            }, "Verify");
        }

        /// <summary>
        /// Completes a password reset request using the token
        /// </summary>
        /// <param name="token">The token identifier.</param>
        /// <returns>A task that represents the asynchronous operation. 
        /// The task result contains an <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("/resetpassword")]
        public async Task<IActionResult> ResetPassword(string token = null)
        {
            return await HandleExceptionAsync(async () =>
            {
                if (ModelState.IsValid)
                {
                    TempData["ErrorMessageLogin"] = await _accountService.CompleteUserPasswordRequestAsync(token);
                    return RedirectToAction(Account_Login);
                }
                TempData["ErrorMessageLogin"] = Error_TokenInvalidDefault;
                return RedirectToAction(Account_Login);
            }, "ResetPassword");
        }
    }
}
