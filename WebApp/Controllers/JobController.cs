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
    public class JobController : ControllerBase<JobController>
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
        [Authorize(Policy = "Admin")]
        [Route("admin/all")]
        public async Task<IActionResult> GetAllJobsAdmin(FilterServiceModel filters)
        {
            return await HandleExceptionAsync(async () =>
            {
                var jobs = await _jobService.GetAllJobsAsync(filters);

                await InitializeValues(filters);
                await InitializeValues(filters.FilterByEmploymentType.FirstOrDefault(), filters.FilterByWorkSetup.FirstOrDefault());

                ViewBag.Companies = await _companyService.GetCompaniesWithListingsAsync();

                return View("Index", jobs);
            }, "GetAllJobsAdmin");
        }

        [HttpGet]
        [Authorize(Policy = "Recruiter")]
        [Route("recruiter/all")]
        public async Task<IActionResult> GetAllJobsRecruiter(FilterServiceModel filters)
        {
            return await HandleExceptionAsync(async () =>
            {
                var company = await _companyService.GetCompanyByRecruiterId(UserId);
                if (company == null) return RedirectToAction("RegisterCompany", "Company");

                ViewBag.Verified = company.IsVerified;

                var jobs = await _jobService.GetRecruiterJobsAsync(filters);

                await InitializeValues(filters);
                await InitializeValues(filters.FilterByEmploymentType.FirstOrDefault(), filters.FilterByWorkSetup.FirstOrDefault());

                return View("Index", jobs);
            }, "GetAllJobsRecruiter");
        }

        [HttpGet]
        [Authorize(Policy = "Recruiter")]
        [Route("recruiter/archived")]
        public async Task<IActionResult> GetArchivedJobsRecruiter(FilterServiceModel filters)
        {
            return await HandleExceptionAsync(async () =>
            {
                if (filters.SortBy.IsNullOrEmpty()) filters.SortBy = "updated_desc";
                var jobs = await _jobService.GetRecruiterJobsAsync(filters, "archived");

                await InitializeValues(filters);

                return View("IndexArchived", jobs);
            }, "GetAllJobsRecruiter");
        }

        [HttpGet]
        [Authorize(Policy = "Admin")]
        [Route("/archived")]
        public async Task<IActionResult> GetArchivedJobs(FilterServiceModel filters)
        {
            return await HandleExceptionAsync(async () =>
            {
                if (filters.SortBy.IsNullOrEmpty()) filters.SortBy = "updated_desc";
                var jobs = await _jobService.GetAllJobsAsync(filters, "archived");

                await InitializeValues(filters);

                return View("IndexArchived", jobs);
            }, "GetAllJobsRecruiter");
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

        [HttpGet]
        [Authorize(Policy = "Applicant")]
        [Route("all")]
        public async Task<IActionResult> GetAllJobsApplicant(FilterServiceModel filters)
        {
            return await HandleExceptionAsync(async () =>
            {
                var jobs = await _jobService.GetAllJobsAsync(filters);

                await InitializeValues(filters);
                await InitializeValues(filters.FilterByEmploymentType, filters.FilterByWorkSetup);

                return View("IndexApplicant", jobs);
            }, "GetAllJobsApplicant");
        }

        private async Task InitializeValues(FilterServiceModel filters)
        {
            ViewData["SortBy"] = filters.SortBy;
            ViewData["Search"] = filters.Search;
            ViewData["FilterByCompany"] = filters.FilterByCompany;
            ViewData["FilterByStatusType"] = filters.FilterByStatusType;
            ViewData["FilterByDatePosted"] = filters.FilterByDatePosted;
            ViewData["FilterBySalary"] = filters.FilterBySalary;

            await PopulateViewBagsAsync();
        }

        private async Task InitializeValues(string filterByEmploymentType, string filterByWorkSetup)
        {
            ViewData["FilterByEmploymentType"] = filterByEmploymentType;
            ViewData["FilterByWorkSetup"] = filterByWorkSetup;
        }

        private async Task InitializeValues(List<string> filterByEmploymentType, List<string> filterByWorkSetup)
        {
            ViewData["FilterByEmploymentType"] = filterByEmploymentType;
            ViewData["FilterByWorkSetup"] = filterByWorkSetup;
        }

        private async Task PopulateViewBagsAsync()
        {
            ViewBag.EmploymentTypes = await _referenceDataService.GetEmploymentTypesAsync();
            ViewBag.StatusTypes = await _referenceDataService.GetStatusTypesAsync();
            ViewBag.WorkSetups = await _referenceDataService.GetWorkSetupsAsync();
            ViewBag.YearLevels = (await _referenceDataService.GetYearLevelsAsync())
                .OrderByDescending(y => y.Year).ToList();
            ViewBag.Programs = await _referenceDataService.GetProgramsAsync();
            ViewBag.SkillsSoft = await _referenceDataService.GetSoftSkillsAsync();
            ViewBag.SkillsTechnical = await _referenceDataService.GetTechnicalSkillsAsync();
            ViewBag.SkillsCertification = await _referenceDataService.GetCertificationSkillsAsync();
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

        private string RedirectString()
        {
            bool isRecruiter = User.IsInRole("Recruiter");
            bool isApplicant = User.IsInRole("Applicant");
            if (isApplicant) return "GetAllJobsApplicant";

            return isRecruiter ? "GetAllJobsRecruiter" : "GetAllJobsAdmin";
        }
        #endregion GET Methods

        #region POST Methods        
        /// <summary>
        /// Creates a new job.
        /// </summary>
        /// <param name="model">The job view model.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Authorize(Policy = "Recruiter")]
        [Route("create")]
        public async Task<IActionResult> Create(JobViewModel model)
        {
            return await HandleExceptionAsync(async () =>
            {
                if (ModelState.IsValid)
                {
                    model.PostedById = UserId;
                    await _jobService.AddJobAsync(model);
                    TempData["SuccessMessage"] = "Successfully created a new job!";
                    return Json(new { success = true });
                }
                TempData["ErrorMessage"] = "An error has occurred while creating a new job.";
                return Json(new { success = false });
            }, "Create");
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
        #endregion POST Methods
    }
}
