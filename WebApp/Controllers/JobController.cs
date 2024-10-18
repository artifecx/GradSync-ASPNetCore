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
            TokenValidationParametersFactory tokenValidationParametersFactory,
            TokenProviderOptionsFactory tokenProviderOptionsFactory) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _jobService = jobService;
        }

        #region GET Methods 
        [HttpGet]
        [Authorize(Policy = "Admin")]
        [Route("admin/all")]
        public async Task<IActionResult> GetAllJobsAdmin(
            string sortBy, 
            string search, 
            string filterByCompany,
            string filterByEmploymentType, 
            string filterByStatusType,
            string filterByWorkSetup, 
            int pageIndex = 1)
        {
            return await HandleExceptionAsync(async () =>
            {
                var filterByEmploymentTypeList = new List<string>();
                if (!string.IsNullOrEmpty(filterByEmploymentType))
                    filterByEmploymentTypeList.Add(filterByEmploymentType);
                var filterByWorkSetupList = new List<string>();
                if (!string.IsNullOrEmpty(filterByWorkSetup))
                    filterByWorkSetupList.Add(filterByWorkSetup);

                var jobs = await _jobService.GetAllJobsAsync(sortBy, search, filterByCompany, filterByEmploymentTypeList, filterByStatusType, filterByWorkSetupList, pageIndex, 10);

                await InitializeValues(sortBy, search, filterByCompany, filterByStatusType);
                await InitializeValues(filterByEmploymentType, filterByWorkSetup);

                ViewBag.Companies = await _jobService.GetCompaniesWithListingsAsync();

                return View("Index", jobs);
            }, "GetAllJobsAdmin");
        }

        [HttpGet]
        [Authorize(Policy = "Recruiter")]
        [Route("recruiter/all")]
        public async Task<IActionResult> GetAllJobsRecruiter(
            string sortBy,
            string search,
            string filterByCompany,
            string filterByEmploymentType,
            string filterByStatusType,
            string filterByWorkSetup,
            int pageIndex = 1)
        {
            return await HandleExceptionAsync(async () =>
            {
                var filterByEmploymentTypeList = new List<string>();
                if(!string.IsNullOrEmpty(filterByEmploymentType))
                    filterByEmploymentTypeList.Add(filterByEmploymentType);
                var filterByWorkSetupList = new List<string>();
                if (!string.IsNullOrEmpty(filterByWorkSetup))
                    filterByWorkSetupList.Add(filterByWorkSetup);

                var jobs = await _jobService.GetRecruiterJobsAsync(sortBy, search, filterByCompany, filterByEmploymentTypeList, filterByStatusType, filterByWorkSetupList, pageIndex, 10);

                await InitializeValues(sortBy, search, filterByCompany, filterByStatusType);
                await InitializeValues(filterByEmploymentType, filterByWorkSetup);

                return View("Index", jobs);
            }, "GetAllJobsRecruiter");
        }

        [HttpGet]
        [Authorize(Policy = "Applicant")]
        [Route("all")]
        public async Task<IActionResult> GetAllJobsApplicant(
            string sortBy,
            string search,
            string filterByCompany,
            List<string> filterByEmploymentType,
            string filterByStatusType,
            List<string> filterByWorkSetup,
            int pageIndex = 1,
            string filterByDatePosted = null,
            string filterBySalary = null)
        {
            return await HandleExceptionAsync(async () =>
            {
                var jobs = await _jobService.GetAllJobsAsync(sortBy, search, filterByCompany, filterByEmploymentType, 
                    filterByStatusType, filterByWorkSetup, pageIndex, 3, filterByDatePosted, filterBySalary);

                await InitializeValues(sortBy, search, filterByCompany, filterByStatusType, filterByDatePosted, filterBySalary);
                await InitializeValues(filterByEmploymentType, filterByWorkSetup);

                return View("IndexApplicant", jobs);
            }, "GetAllJobsApplicant");
        }

        private async Task InitializeValues(
            string sortBy,
            string search,
            string filterByCompany,
            string filterByStatusType,
            string filterByDatePosted = null,
            string filterBySalary = null)
        {
            ViewData["SortBy"] = sortBy;
            ViewData["Search"] = search;
            ViewData["FilterByCompany"] = filterByCompany;
            ViewData["FilterByStatusType"] = filterByStatusType;
            ViewData["FilterByDatePosted"] = filterByDatePosted;
            ViewData["FilterBySalary"] = filterBySalary;

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
            ViewBag.EmploymentTypes = await _jobService.GetEmploymentTypesAsync();
            ViewBag.StatusTypes = await _jobService.GetStatusTypesAsync();
            ViewBag.WorkSetups = await _jobService.GetWorkSetupsAsync();
            ViewBag.YearLevels = (await _jobService.GetYearLevelsAsync())
                .OrderByDescending(y => y.Year).ToList();
            ViewBag.Departments = await _jobService.GetDepartmentsAsync();
            ViewBag.Skills = await _jobService.GetSkillsAsync();
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
                if (ModelState.IsValid)
                {
                    // Implement
                    var m = model;
                    TempData["SuccessMessage"] = "Wowzers";
                    return Json(new { success = true });
                }
                TempData["ErrorMessage"] = "Ernk";
                return Json(new { success = false });
            }, "Update");
        }

        /// <summary>
        /// Deletes the selected job.
        /// </summary>
        /// <param name="id">The job identifier.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Authorize(Policy = "AdminOrRecruiter")]
        [Route("delete")]
        public async Task<IActionResult> Delete(string id)
        {
            return await HandleExceptionAsync(async () =>
            {
                return Json(new { success = false });
            }, "Delete");
        }
        #endregion POST Methods
    }
}
