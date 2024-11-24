using Data.Models;
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
using System.Linq;
using System.Threading.Tasks;
using Resources.Messages;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;

namespace WebApp.Controllers
{
    /// <summary>
    /// Controller for handling job-related operations.
    /// </summary>
    [Route("jobs")]
    public partial class JobController : ControllerBase<JobController>
    {
        private readonly IJobService _jobService;
        private readonly ICompanyService _companyService;
        private readonly IReferenceDataService _referenceDataService;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobController"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="jobService">The team service.</param>
        /// <param name="tokenValidationParametersFactory">The token validation parameters factory.</param>
        /// <param name="tokenProviderOptionsFactory">The token provider options factory.</param>
        public JobController(
            IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IMapper mapper,
            IJobService jobService,
            ICompanyService companyService,
            IReferenceDataService referenceDataService,
            TokenValidationParametersFactory tokenValidationParametersFactory,
            TokenProviderOptionsFactory tokenProviderOptionsFactory) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _jobService = jobService;
            _companyService = companyService;
            _referenceDataService = referenceDataService;
        }

        #region GET Methods 
        [HttpGet]
        [Authorize]
        [Route("get-applicant-details")]
        public async Task<IActionResult> GetApplicantDetails(string id)
        {
            return await HandleExceptionAsync(async () =>
            {
                if (ModelState.IsValid && !string.IsNullOrEmpty(id))
                {
                    var model = await _jobService.GetApplicantDetailsAsync(id);
                    return PartialView("_ApplicantDetails", model);
                }
                return PartialView("_ApplicantDetails", new Applicant());
            }, "GetApplicantDetails");
        }

        /// <summary>
        /// Views the selected job.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="showModal">The show modal.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [Authorize]
        [Route("view")]
        public async Task<IActionResult> GetJob(string id, string showModal = null)
        {
            return await HandleExceptionAsync(async () =>
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["ErrorMessage"] = "Invalid job id!";
                    return RedirectToAction(RedirectString());
                }

                var job = await _jobService.GetJobByIdAsync(id);
                if (job == null)
                {
                    TempData["ErrorMessage"] = "Job not found!";
                    return RedirectToAction(RedirectString());
                }

                ViewBag.ShowModal = showModal;
                ViewBag.UserId = UserId;
                await PopulateViewBagsAsync();

                return View("ViewJob", job);
            }, "GetJob");
        }
        #endregion GET Methods

        #region POST Methods        
        /// <summary>
        /// Archives the selected job.
        /// </summary>
        /// <param name="id">The job identifier.</param>
        /// <returns>A task that represents the asynchronous operation. 
        /// The task result contains an <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Authorize(Policy = "AdminOrRecruiter")]
        [Route("archive")]
        public async Task<IActionResult> Archive(string id)
        {
            return await HandleExceptionAsync(async () =>
            {
                if (ModelState.IsValid)
                {
                    await _jobService.ArchiveJobAsync(id);
                    TempData["SuccessMessage"] = "Successfully archived the job.";
                    return Json(new { success = true });
                }
                TempData["ErrorMessage"] = "An error has occurred while archiving the job.";
                return Json(new { success = false });
            }, "Delete");
        }

        /// <summary>
        /// Updates the selected job.
        /// </summary>
        /// <param name="model">The job view model.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Authorize(Policy = "AdminOrRecruiter")]
        [Route("update")]
        public async Task<IActionResult> Update(JobViewModel model)
        {
            return await HandleExceptionAsync(async () =>
            {
                if (ModelState.IsValid && (string.Equals(model.PostedById, UserId) || User.IsInRole("NLO")))
                {
                    await _jobService.UpdateJobAsync(model);
                    TempData["SuccessMessage"] = "Successfully updated the job.";
                    return Json(new { success = true });
                }
                TempData["ErrorMessage"] = "An error has occurred while updating the job.";
                return Json(new { success = false });
            }, "Update");
        }

        [HttpPost]
        [Authorize(Policy = "AdminOrRecruiter")]
        [Route("unarchive")]
        public async Task<IActionResult> Unarchive(JobServiceModel model)
        {
            return await HandleExceptionAsync(async () =>
            {
                if (ModelState.IsValid)
                {
                    await _jobService.UnarchiveJobAsync(model);
                    TempData["SuccessMessage"] = "Successfully unarchived job!";
                    return Json(new { success = true });
                }
                TempData["ErrorMessage"] = "An error has occurred while removing the job from the archive.";
                return Json(new { success = false });
            }, "GetAllJobsRecruiter");
        }
        #endregion POST Methods
    }
}
