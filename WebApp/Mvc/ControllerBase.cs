using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Services.Exceptions.CompanyExceptions;
using static Services.Exceptions.UserExceptions;
using static Services.Exceptions.EmailExceptions;
using static Resources.Messages.ErrorMessages;
using Services.Interfaces;
using static Services.Exceptions.JobApplicationExceptions;
using static Services.Exceptions.JobExceptions;
using System.Diagnostics;
using Data.Models;
using WebApp.Models;

namespace WebApp.Mvc
{
    /// <summary>
    /// Declare ControllerBase.
    /// </summary>
    public class ControllerBase<TController> : Controller where TController : class
    {
        /// <summary>AppConfiguration</summary>
        protected readonly IConfiguration _configuration;

        /// <summary>HttpContextAccessor</summary>
        protected readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>Logger</summary>
        protected ILogger _logger;

        /// <summary>Session</summary>
        protected ISession _session => _httpContextAccessor.HttpContext.Session;

        protected IUserProfileService _userProfile { get; set; }

        /// <summary>
        /// Initializes a new instance of the ControllerBase{TController} class.
        /// </summary>
        /// <param name="httpContextAccessor">HTTP context accessor</param>
        /// <param name="localizer">Localizer</param>
        /// <param name="loggerFactory">Logger factory</param>
        /// <param name="configuration">Configuration</param>
        /// <param name="mapper">Mapper</param>
        public ControllerBase(
                                IHttpContextAccessor httpContextAccessor,
                                ILoggerFactory loggerFactory,
                                IConfiguration configuration,
                                IMapper mapper = null,
                                IUserProfileService userProfile = null)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._configuration = configuration;
            this._logger = loggerFactory.CreateLogger<TController>();
            this._configuration = configuration;
            this._mapper = mapper;
            this._userProfile = userProfile;
        }

        /// <summary>Mapper</summary>
        protected IMapper _mapper { get; set; }

        /// <summary>
        /// Get Email.
        /// </summary>
        public string UserId
        {
            get { return User.FindFirst(ClaimTypes.NameIdentifier)?.Value; }
        }

        /// <summary>
        /// Get Role.
        /// </summary>
        public string UserRole
        {
            get { return User.FindFirst(ClaimTypes.Role)?.Value; }
        }

        /// <summary>
        /// Get UserName.
        /// </summary>
        public string UserName
        {
            get { return User.Identity.Name; }
        }

        /// <summary>
        /// Get RoleId.
        /// </summary>
        public string Supervisor
        {
            get { return User.FindFirst(ClaimTypes.Role).Value; }
        }

        /// <summary>
        /// Get ClientId.
        /// </summary>
        public string ClientId
        {
            get { return User.FindFirst("ClientId").Value; }
        }

        /// <summary>
        /// Get ClientSystemId
        /// </summary>
        public string ClientSystemId
        {
            get { return User.FindFirst("ClientSystemId").Value; }
        }

        /// <summary>
        /// Get ClientSystemName
        /// </summary>
        public string ClientSystemName
        {
            get { return User.FindFirst("ClientSystemName").Value; }
        }

        /// <summary>
        /// Get ClientUserRole.
        /// </summary>
        public string ClientUserRole
        {
            get { return User.FindFirst("ClientUserRole").Value; }
        }

        /// <summary>
        /// OnActionExecuting.
        /// </summary>
        /// <param name="context">context</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // do nothing
        }

        /// <summary>
        /// OnActionExecuted.
        /// </summary>
        /// <param name="context">context</param>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            // do nothing
        }

        /// <summary>
        /// Write Log on Exception 
        /// </summary>
        protected void HandleExceptionLog(Exception ex, string request)
        {
            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
            string actionMethod = this.ControllerContext.RouteData.Values["action"].ToString();

            StringBuilder logContent = new StringBuilder();
            logContent.AppendLine($"\n======================================== start ========================================");
            logContent.AppendLine($"■ API Controller Name: \n\t{controllerName}");
            logContent.AppendLine($"■ API Action Method: \n\t{actionMethod}");
            logContent.AppendLine($"■ API Request Model: \n\t{request}");
            logContent.AppendLine($"■ Exception Message: \n\t{ex.Message}");
            logContent.AppendLine($"■ Exception StackTrace: \n\t{ex.StackTrace}");
            logContent.AppendLine($"========================================= end =========================================\r\n");

            this._logger.LogError(logContent.ToString());
        }

        /// <summary>
        /// Starts the log.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        protected void StartLog(object action, string methodName)
        {
            string controllerName = GetControllerNameFromAction(action);
            _logger.LogInformation("======= {Controller} : {Method} Started =======", controllerName, methodName);
        }

        /// <summary>
        /// Ends the log.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        protected void EndLog(object action, string methodName)
        {
            string controllerName = GetControllerNameFromAction(action);
            _logger.LogInformation("======= {Controller} : {Method} Ended=======", controllerName, methodName);
        }

        /// <summary>
        /// Gets the controller name from action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        private static string GetControllerNameFromAction(object action)
        {
            if (action is Delegate del)
            {
                var methodInfo = del.Method;
                var controllerType = methodInfo.DeclaringType;
                var controllerName = controllerType.DeclaringType?.Name != null ? 
                    controllerType.DeclaringType.Name : controllerType.Name;

                if (controllerName.EndsWith("Controller"))
                {
                    controllerName = controllerName.Substring(0, controllerName.Length - "Controller".Length);
                }

                return controllerName;
            }

            return "UnknownController";
        }

        #region Async Exception Handlers        
        /// <summary>
        /// Logs and sets an error message.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <returns>true</returns>
        private bool LogAndSetErrorMessage(Exception ex, string actionName)
        {
            TempData["ErrorMessage"] = string.Equals("Login", actionName) ? null : ex.Message.ToString();
            _logger.LogError(ex, "Error in {Action}", actionName);
            return true;
        }

        /// <summary>
        /// Handles an <see cref="IActionResult"/> exception asynchronously.
        /// Logs the exception and redirects to the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <returns></returns>
        protected async Task<IActionResult> HandleExceptionAsync(Func<Task<IActionResult>> action, string actionName)
        {
            try
            {
                StartLog(action, actionName);
                return await action();
            }
            catch (UserException ex) when (LogAndSetErrorMessage(ex, actionName))
            {
                return RedirectToAction(actionName);
            }
            catch (UserException ex) when (LogAndSetErrorMessage(ex, actionName))
            {
                return RedirectToAction(actionName, new { id = ex.Id });
            }
            catch (JobApplicationException ex) when (LogAndSetErrorMessage(ex, actionName))
            {
                return RedirectToAction(actionName);
            }
            catch (JobApplicationException ex) when (LogAndSetErrorMessage(ex, actionName))
            {
                return RedirectToAction(actionName, new { id = ex.Id });
            }
            catch (JobException ex) when (LogAndSetErrorMessage(ex, actionName))
            {
                return RedirectToAction(actionName);
            }
            catch (JobException ex) when (LogAndSetErrorMessage(ex, actionName))
            {
                return RedirectToAction(actionName, new { id = ex.Id });
            }
            catch (CompanyException ex) when (LogAndSetErrorMessage(ex, actionName))
            {
                return RedirectToAction(actionName);
            }
            catch (CompanyException ex) when (LogAndSetErrorMessage(ex, actionName))
            {
                return RedirectToAction(actionName, new { id = ex.Id });
            }
            catch (EmailException ex) when (LogAndSetErrorMessage(ex, actionName))
            {
                return RedirectToAction(actionName);
            }
            catch (EmailException ex) when (LogAndSetErrorMessage(ex, actionName))
            {
                return RedirectToAction(actionName, new { id = ex.Id });
            }
            catch (UserNotVerifiedException ex) when (LogAndSetErrorMessage(ex, actionName))
            {
                TempData["ErrorMessageLogin"] = ex.Message;
                return View(actionName, new LoginViewModel { Email = ex.Id });
            }
            catch (Exception ex) when (LogAndSetErrorMessage(ex, actionName))
            {
                _logger.LogError(ex, "Error in {Action}", actionName);
                return RedirectToAction(actionName);
            }
            finally
            {
                EndLog(action, actionName);
            }
        }

        /// <summary>
        /// Handles a <see cref="JsonResult"/> exception asynchronously.
        /// Sets a <see cref="TempData"/> error message and logs the exception. 
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <returns><see cref="JsonResult"/></returns>
        protected async Task<JsonResult> HandleExceptionAsync(Func<Task<JsonResult>> action, string actionName)
        {
            try
            {
                StartLog(action, actionName);
                return await action();
            }
            catch (CompanyException ex)
            {
                TempData["ErrorMessage"] = ex.Message.ToString();
                _logger.LogError(ex, "Error in {Action}", actionName);
                return new JsonResult(new { success = false, error = ex.Message });
            }
            catch (UserException ex)
            {
                TempData["ErrorMessage"] = ex.Message.ToString();
                _logger.LogError(ex, "Error in {Action}", actionName);
                return new JsonResult(new { success = false, error = ex.Message });
            }
            catch (EmailException ex)
            {
                TempData["ErrorMessage"] = ex.Message.ToString();
                _logger.LogError(ex, "Error in {Action}", actionName);
                return new JsonResult(new { success = false, error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message.ToString();
                _logger.LogError(ex, "Error in {Action}", actionName);
                return new JsonResult(new { success = false, error = ex.Message });
            }
            catch (JobApplicationException ex)
            {
                TempData["ErrorMessage"] = ex.Message.ToString();
                _logger.LogError(ex, "Error in {Action}", actionName);
                return new JsonResult(new { success = false, error = ex.Message });
            }
            catch (JobException ex)
            {
                TempData["ErrorMessage"] = ex.Message.ToString();
                _logger.LogError(ex, "Error in {Action}", actionName);
                return new JsonResult(new { success = false, error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in {Action}", actionName);
                return new JsonResult(new { success = false, error = "An error has occured. Please try again later." });
            }
            finally
            {
                EndLog(action, actionName);
            }
        }
        #endregion

        /// <summary>
        /// Checks the form submission time.
        /// </summary>
        /// <param name="FormLoadTime">The form load time.</param>
        /// <exception cref="UserException"></exception>
        protected static void CheckFormSubmissionTime(string FormLoadTime)
        {
            long ticks;
            if (long.TryParse(FormLoadTime, out ticks))
            {
                var timeTaken = TimeSpan.FromTicks(Stopwatch.GetTimestamp() - ticks);
                if (timeTaken.TotalSeconds < 8)
                {
                    throw new UserException(Error_UserRegistrationDefault);
                }
            }
        }   
    }
}
