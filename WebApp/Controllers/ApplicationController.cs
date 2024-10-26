using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WebApp.Mvc;
using Services.Interfaces;
using Services.ServiceModels;
using static Resources.Constants.Routes;
using static Resources.Messages.ErrorMessages;
using static Resources.Messages.SuccessMessages;
using System.Security.Claims;

namespace WebApp.Controllers
{
    /// <summary>
    /// Controller endpoints for job applications
    /// </summary>
    [Authorize]
    [Route("applications")]
    public partial class ApplicationController : ControllerBase<ApplicationController>
    {
        private readonly IApplicationService _applicationService;
        private readonly IReferenceDataService _referenceDataService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationController"/> class.
        /// </summary>
        /// <param name="applicationService">The service for managing application-related operations.</param>
        /// <param name="referenceDataService">The service for retrieving reference data used in the application.</param>
        /// <param name="httpContextAccessor">Accessor for HTTP context information.</param>
        /// <param name="loggerFactory">Factory for creating loggers.</param>
        /// <param name="configuration">Application configuration settings.</param>
        /// <param name="mapper">Object mapper for converting between data and service models.</param>
        public ApplicationController(
            IApplicationService applicationService,
            IReferenceDataService referenceDataService,
            IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IMapper mapper) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            this._applicationService = applicationService;
            this._referenceDataService = referenceDataService;
        }

        /// <summary>
        /// Retrieves all applications based on the specified filters.
        /// </summary>
        /// <param name="filters">The filters to apply when retrieving applications.</param>
        /// <returns>
        /// A view displaying the filtered list of applications or redirects in case of invalid model state.
        /// </returns>
        [HttpGet]
        [Route("all")]
        [Authorize]
        public async Task<IActionResult> GetAllApplications(ApplicationFilter filters)
        {
            return await HandleExceptionAsync(async () =>
            {
                if (ModelState.IsValid)
                {
                    filters.UserId = UserId;
                    filters.UserRole = UserRole;

                    var model = await _applicationService.GetAllApplicationsAsync(filters);

                    ViewData["Search"] = filters.Search;
                    ViewData["SortBy"] = filters.SortBy;
                    ViewData["ProgramFilter"] = filters.ProgramFilter;
                    ViewData["WorkSetupFilter"] = filters.WorkSetupFilter;
                    ViewData["StatusFilter"] = filters.StatusFilter;

                    await PopulateViewBagsAsync();

                    return View("Index", model);
                }
                return RedirectToAction(Home_InvalidAccess, Controller_Home);
            }, Application_GetAll);
        }

        /// <summary>
        /// Retrieves the details of a specific application by its identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the application to retrieve.</param>
        /// <returns>
        /// A view displaying the application details or redirects if the application is not found or if the model state is invalid.
        /// </returns>
        // In your ApplicationController
        [HttpGet]
        [Route("view")]
        [Authorize]
        public async Task<IActionResult> GetApplication(string id)
        {
            return await HandleExceptionAsync(async () =>
            {
                if (ModelState.IsValid && !string.IsNullOrEmpty(id))
                {
                    var model = await _applicationService.GetApplicationByIdAsync(id);
                    if (model == null)
                    {
                        TempData["ErrorMessage"] = Error_ApplicationNotFound;
                        return RedirectToAction(Application_GetAll);
                    }

                    ViewBag.UserId = UserId;

                    return View("ViewApplication", model);
                }
                TempData["ErrorMessage"] = string.Format(Error_ApplicationActionError, "retrieving");
                return RedirectToAction(Application_GetAll);
            }, Application_GetApplication);
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
        /// Populates the ViewBag with reference data needed for the views.
        /// </summary>
        private async Task PopulateViewBagsAsync()
        {
            ViewBag.Programs = await _referenceDataService.GetProgramsAsync();
            ViewBag.WorkSetups = await _referenceDataService.GetWorkSetupsAsync();
            ViewBag.AppStatusTypes = await _referenceDataService.GetApplicationStatusTypesAsync();
        }
    }
}
