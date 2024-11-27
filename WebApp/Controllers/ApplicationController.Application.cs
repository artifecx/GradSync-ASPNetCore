using WebApp.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static Resources.Constants.Routes;
using static Resources.Constants.Types;
using static Resources.Messages.ErrorMessages;
using static Resources.Messages.SuccessMessages;
using System;

namespace WebApp.Controllers
{
    public partial class ApplicationController : ControllerBase<ApplicationController>
    {
        /// <summary>
        /// Sends the application for the specified job, updating its status accordingly.
        /// </summary>
        /// <param name="jobId">The unique identifier of the job for which the application is being sent.</param>
        /// <returns>
        /// A JSON object indicating the success of the operation:
        /// <list type="bullet">
        ///     <item><description><c>success</c>: A boolean indicating whether the operation was successful.</description></item>
        ///     <item><description><c>message</c>: A string containing a success or error message.</description></item>
        /// </list>
        /// </returns>
        [HttpPost]
        [Authorize(Policy = "Applicant")]
        [Route("apply")]
        public async Task<IActionResult> SendApplication(string jobId)
        {
            return await HandleExceptionAsync(async () =>
            {
                if (ModelState.IsValid && !string.IsNullOrEmpty(jobId))
                {
                    await _applicationService.SendApplicationAsync(UserId, jobId);

                    TempData["SuccessMessage"] = string.Format(Success_ApplicationActionSuccess, "sent");
                    return Json(new { success = true });
                }
                TempData["ErrorMessage"] = string.Format(Error_ApplicationActionError, "sending");
                return Json(new { success = false });
            }, Application_ActionSend);
        }

        /// <summary>
        /// Updates the status of a specified application.
        /// </summary>
        /// <param name="applicationId">The unique identifier of the application to update.</param>
        /// <param name="appStatusId">The unique identifier of the new status to apply to the application.</param>
        /// <returns>
        /// A JSON object indicating the success of the operation:
        /// <list type="bullet">
        ///     <item><description><c>success</c>: A boolean indicating whether the operation was successful.</description></item>
        ///     <item><description><c>message</c>: A string containing a success or error message.</description></item>
        /// </list>
        /// </returns>
        [HttpPost]
        [Route("update-application")]
        [Authorize]
        public async Task<IActionResult> UpdateApplication(string applicationId, string appStatusId)
        {
            return await HandleExceptionAsync(async () =>
            {
                if (ModelState.IsValid && !string.IsNullOrEmpty(applicationId) && !string.IsNullOrEmpty(appStatusId))
                {
                    await _applicationService.UpdateApplicationAsync(UserId, applicationId, appStatusId);

                    TempData["SuccessMessage"] = string.Format(Success_ApplicationActionSuccess, "updated");
                    return Json(new { success = true });
                }
                TempData["ErrorMessage"] = string.Format(Error_ApplicationActionError, "updating");
                return Json(new { success = false });
            }, Application_ActionUpdate);
        }

        /// <summary>
        /// Withdraws the specified application, updating its status accordingly.
        /// </summary>
        /// <param name="applicationId">The unique identifier of the application to be withdrawn.</param>
        /// <returns>
        /// A JSON object indicating the success of the operation:
        /// <list type="bullet">
        ///     <item><description><c>success</c>: A boolean indicating whether the operation was successful.</description></item>
        ///     <item><description><c>message</c>: A string containing a success or error message.</description></item>
        /// </list>
        /// </returns>
        [HttpPost]
        [Authorize(Policy = "Applicant")]
        [Route("withdraw")]
        public async Task<IActionResult> WithdrawApplication(string applicationId)
        {
            return await HandleExceptionAsync(async () =>
            {
                if (ModelState.IsValid && !string.IsNullOrEmpty(applicationId))
                {
                    await _applicationService.UpdateApplicationAsync(UserId, applicationId, AppStatus_Withdrawn);

                    TempData["SuccessMessage"] = string.Format(Success_ApplicationActionSuccess, "withdrew");
                    return Json(new { success = true });
                }
                TempData["ErrorMessage"] = string.Format(Error_ApplicationActionError, "withdrawing");
                return Json(new { success = false });
            }, Application_ActionWithdraw);
        }
    }
}
