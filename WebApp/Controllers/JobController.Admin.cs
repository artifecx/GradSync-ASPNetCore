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
    public partial class JobController : ControllerBase<JobController>
    {
        #region GET Methods
        [HttpGet]
        [Authorize(Policy = "Admin")]
        [Route("admin/all")]
        public async Task<IActionResult> GetAllJobsAdmin(FilterServiceModel filters)
        {
            return await HandleExceptionAsync(async () =>
            {
                filters.UserRole = UserRole;
                var jobs = await _jobService.GetAllJobsAsync(filters);

                await InitializeValues(filters);
                await InitializeValues(filters.FilterByEmploymentType.FirstOrDefault(), filters.FilterByWorkSetup.FirstOrDefault());

                ViewBag.Companies = await _companyService.GetCompaniesWithListingsAsync();

                return View("Index", jobs);
            }, "GetAllJobsAdmin");
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
        #endregion
    }
}
