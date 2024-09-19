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
        [Route("all")]
        public async Task<IActionResult> GetAll(
            string SortBy, 
            string Search, 
            string FilterByCompany, 
            string FilterByEmploymentType, 
            string FilterByStatusType, 
            string FilterByWorkSetup, 
            int PageIndex = 1)
        {
            return await HandleExceptionAsync(async () =>
            {
                var jobs = await _jobService.GetAllJobsAsync(SortBy, Search, FilterByCompany, FilterByEmploymentType, FilterByStatusType, FilterByWorkSetup, PageIndex, 5);

                ViewData["SortBy"] = SortBy;
                ViewData["Search"] = Search;
                ViewData["FilterByCompany"] = FilterByCompany;
                ViewData["FilterByEmploymentType"] = FilterByEmploymentType;
                ViewData["FilterByStatusType"] = FilterByStatusType;
                ViewData["FilterByWorkSetup"] = FilterByWorkSetup;

                ViewBag.Companies = await _jobService.GetCompaniesWithListingsAsync();
                ViewBag.EmploymentTypes = await _jobService.GetEmploymentTypesAsync();
                ViewBag.StatusTypes = await _jobService.GetStatusTypesAsync();
                ViewBag.WorkSetups = await _jobService.GetWorkSetupsAsync();

                return View("ViewAll", jobs);
            }, "GetAll");
        }

        /// <summary>
        /// Views the selected team.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="showModal">The show modal.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [Authorize(Policy = "Admin")]
        [Route("view")]
        public async Task<IActionResult> ViewTeam(string id, string showModal = null)
        {
            return await HandleExceptionAsync(async () =>
            {
                return Json(new { success = false });
            }, "ViewTeam");
        }
        #endregion GET Methods

        #region POST Methods        
        /// <summary>
        /// Creates a new team.
        /// </summary>
        /// <param name="model">The team view model.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Authorize(Policy = "Admin")]
        [Route("create")]
        public async Task<IActionResult> Create(JobViewModel model)
        {
            return await HandleExceptionAsync(async () =>
            {
                if (ModelState.IsValid)
                {
                    // Implement
                    return Json(new { success = true });
                }
                TempData["ErrorMessage"] = Errors.ErrorCreateTeam;
                return Json(new { success = false });
            }, "Create");
        }

        /// <summary>
        /// Updates the selected team.
        /// </summary>
        /// <param name="model">The team view model.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Authorize(Policy = "Admin")]
        [Route("update")]
        public async Task<IActionResult> Update(JobViewModel model)
        {
            return await HandleExceptionAsync(async () =>
            {
                if (ModelState.IsValid)
                {
                    // Implement
                    TempData["SuccessMessage"] = "";
                    return Json(new { success = true });
                }
                TempData["ErrorMessage"] = "";
                return Json(new { success = false });
            }, "Update");
        }

        /// <summary>
        /// Deletes the selected team.
        /// </summary>
        /// <param name="id">The team identifier.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Authorize(Policy = "Admin")]
        [Route("delete")]
        public async Task<IActionResult> Delete(string id)
        {
            return await HandleExceptionAsync(async () =>
            {
                //if (ModelState.IsValid)
                //{
                //    await _teamService.DeleteAsync(id);
                //    TempData["SuccessMessage"] = Common.SuccessDeleteTeam;
                //    return Json(new { success = true });
                //}
                //TempData["ErrorMessage"] = Errors.ErrorDeleteTeam;
                return Json(new { success = false });
            }, "Delete");
        }

        /// <summary>
        /// Assigns an agent to a team.
        /// </summary>
        /// <param name="teamId">The team identifier.</param>
        /// <param name="agentId">The agent identifier.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Authorize(Policy = "Admin")]
        [Route("assignagent")]
        public async Task<IActionResult> AssignAgent(string teamId, string agentId)
        {
            return await HandleExceptionAsync(async () =>
            {
                //if (ModelState.IsValid)
                //{
                //    await _teamService.AddTeamMemberAsync(teamId, agentId);
                //    TempData["SuccessMessage"] = Common.SuccessAssignAgent;
                //    return Json(new { success = true });
                //}
                //TempData["ErrorMessage"] = Errors.ErrorAssignAgent;
                return Json(new { success = false });
            }, "AssignAgent");
        }

        /// <summary>
        /// Reassigns an agent to a different team.
        /// </summary>
        /// <param name="oldTeamId">The old team identifier.</param>
        /// <param name="newTeamId">The new team identifier.</param>
        /// <param name="agentId">The agent identifier.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [Authorize(Policy = "Admin")]
        [Route("reassignagent")]
        public async Task<IActionResult> ReassignAgent(string oldTeamId, string newTeamId, string agentId)
        {
            return await HandleExceptionAsync(async () =>
            {
                if (ModelState.IsValid)
                {
                    //await _teamService.RemoveTeamMemberAsync(oldTeamId, agentId);
                    //if (!string.IsNullOrEmpty(newTeamId))
                    //{
                    //    await _teamService.AddTeamMemberAsync(newTeamId, agentId);
                    //}

                    TempData["SuccessMessage"] = string.IsNullOrEmpty(newTeamId) ?
                        Common.SuccessUnassignAgent : Common.SuccessReassignAgent;
                    return Json(new { success = true });
                }
                TempData["ErrorMessage"] = Errors.ErrorReassignAgent;
                return Json(new { success = false });
            }, "ReassignAgent");
        }
        #endregion POST Methods
    }
}
