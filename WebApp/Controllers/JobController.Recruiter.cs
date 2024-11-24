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
        [Route("get-applicant-details")]
        public async Task<IActionResult> GetApplicantDetails(string id)
        {
            return await HandleExceptionAsync(async () =>
            {
                if (ModelState.IsValid && !string.IsNullOrEmpty(id))
                {
                    var model = await _jobService.GetApplicantDetailsAsync(id);
                    return PartialView("_ApplicantDetailsModal", model);
                }
                return PartialView("_ApplicantDetailsModal", new Applicant());
            }, "GetApplicantDetails");
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
        #endregion  

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
        #endregion
    }
}
