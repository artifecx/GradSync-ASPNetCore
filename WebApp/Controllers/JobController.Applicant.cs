﻿using Data.Models;
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
        [Authorize(Policy = "Applicant")]
        [Route("all")]
        public async Task<IActionResult> GetAllJobsApplicant(FilterServiceModel filters)
        {
            return await HandleExceptionAsync(async () =>
            {
                filters.UserRole = UserRole;
                filters.UserId = UserId;
                var jobs = await _jobService.GetAllJobsAsync(filters);

                await InitializeValues(filters);
                await InitializeValues(filters.FilterByEmploymentType, filters.FilterByWorkSetup);
                ViewBag.UserId = UserId;

                return View("IndexApplicant", jobs);
            }, "GetAllJobsApplicant");
        }
        #endregion
    }
}
