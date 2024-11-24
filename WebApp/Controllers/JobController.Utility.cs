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
    /// Utility methods for the job controller.
    /// </summary>
    public partial class JobController : ControllerBase<JobController>
    {
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

            await Task.CompletedTask;
        }

        private async Task InitializeValues(List<string> filterByEmploymentType, List<string> filterByWorkSetup)
        {
            ViewData["FilterByEmploymentType"] = filterByEmploymentType;
            ViewData["FilterByWorkSetup"] = filterByWorkSetup;

            await Task.CompletedTask;
        }

        private async Task PopulateViewBagsAsync()
        {
            ViewBag.EmploymentTypes = await _referenceDataService.GetEmploymentTypesAsync();
            ViewBag.StatusTypes = await _referenceDataService.GetStatusTypesAsync();
            ViewBag.WorkSetups = await _referenceDataService.GetWorkSetupsAsync();
            ViewBag.YearLevels = await _referenceDataService.GetYearLevelsAsync();
            ViewBag.Programs = await _referenceDataService.GetProgramsAsync();
            ViewBag.SkillsSoft = await _referenceDataService.GetSoftSkillsAsync();
            ViewBag.SkillsTechnical = await _referenceDataService.GetTechnicalSkillsAsync();
            ViewBag.SkillsCertification = await _referenceDataService.GetCertificationSkillsAsync();
        }

        private string RedirectString()
        {
            bool isRecruiter = User.IsInRole("Recruiter");
            bool isApplicant = User.IsInRole("Applicant");
            if (isApplicant) return "GetAllJobsApplicant";

            return isRecruiter ? "GetAllJobsRecruiter" : "GetAllJobsAdmin";
        }
    }
}
