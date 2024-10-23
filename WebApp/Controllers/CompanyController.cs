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
using System.Threading.Tasks;

namespace WebApp.Controllers
{
    /// <summary>
    /// Controller for handling job-related operations.
    /// </summary>
    [Route("companies")]
    public class CompanyController : ControllerBase<CompanyController>
    {
        private readonly ICompanyService _companyService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyController"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="companyService">The team service.</param>
        /// <param name="tokenValidationParametersFactory">The token validation parameters factory.</param>
        /// <param name="tokenProviderOptionsFactory">The token provider options factory.</param>
        public CompanyController(
            IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IMapper mapper,
            ICompanyService companyService,
            TokenValidationParametersFactory tokenValidationParametersFactory,
            TokenProviderOptionsFactory tokenProviderOptionsFactory) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _companyService = companyService;
        }

        #region GET Methods 
        [HttpGet]
        [Authorize(Policy = "Admin")]
        [Route("all")]
        public async Task<IActionResult> GetAllCompanies(
            string sortBy, 
            string search,
            bool? verified,
            bool? hasValidMOA,
            int pageIndex = 1)
        {
            return await HandleExceptionAsync(async () =>
            {
                var companies = await _companyService.GetAllCompaniesAsync(sortBy, search, verified, hasValidMOA, pageIndex, 5);

                await InitializeValues(sortBy, search, verified, hasValidMOA);

                return View("Index", companies);
            }, "GetAllCompanies");
        }

        private async Task InitializeValues(
            string sortBy,
            string search,
            bool? verified,
            bool? hasValidMOA)
        {
            ViewData["Search"] = search;
            ViewData["SortBy"] = sortBy;
            ViewData["Verified"] = verified;
            ViewData["HasValidMOA"] = hasValidMOA;
        }

        [HttpGet]
        [Authorize]
        [Route("view")]
        public async Task<IActionResult> GetCompany(string id, string showModal = null)
        {
            return await HandleExceptionAsync(async () =>
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["ErrorMessage"] = "Invalid company id!";
                    return RedirectToAction("GetAllCompanies");
                }

                var company = await _companyService.GetCompanyByIdAsync(id);
                if (company == null)
                {
                    TempData["ErrorMessage"] = "Company not found!";
                    return RedirectToAction("GetAllCompanies");
                }

                ViewBag.ShowModal = showModal;

                return View("ViewCompany", company);
            }, "GetCompany");
        }

        [HttpGet]
        [Authorize(Policy = "Recruiter")]
        [Route("/company/view")]
        public async Task<IActionResult> GetCompanyRecruiter()
        {
            return await HandleExceptionAsync(async () =>
            {
                var company = await _companyService.GetCompanyByRecruiterId(UserId);
                if (company == null)
                {
                    TempData["ErrorMessage"] = "Company not found!";
                    return RedirectToAction("RegisterCompany");
                }

                return View("CompanyDetails", company);
            }, "GetCompanyRecruiter");
        }

        [HttpGet]
        [Authorize(Policy = "Recruiter")]
        [Route("/company/register")]
        public async Task<IActionResult> RegisterCompany()
        {
            return await HandleExceptionAsync(async () =>
            {
                var company = await _companyService.GetCompanyByRecruiterId(UserId);
                if(company != null) return RedirectToAction("GetCompanyRecruiter");

                return View(new CompanyViewModel());
            }, "RegisterCompany");
        }

        [HttpPost]
        [Authorize(Policy = "Recruiter")]
        [Route("/company/register")]
        public async Task<IActionResult> RegisterCompany(CompanyViewModel model)
        {
            return await HandleExceptionAsync(async () =>
            {
                if (ModelState.IsValid)
                {
                    model.RecruiterId = UserId;
                    await _companyService.AddCompanyAsync(model);
                    TempData["SuccessMessage"] = "Company registered successfully!";
                    return Json(new { success = true });
                }
                TempData["ErrorMessage"] = "An error has occured while registering company.";
                return Json(new { success = false });
            }, "RegisterCompany");
        }

        [HttpPost]
        [Authorize(Policy = "AdminOrRecruiter")]
        [Route("add")]
        public async Task<IActionResult> AddCompany(CompanyViewModel company)
        {
            return await HandleExceptionAsync(async () =>
            {
                if (ModelState.IsValid)
                {
                    await _companyService.AddCompanyAsync(company);
                    TempData["SuccessMessage"] = "Company added successfully!";
                    return Json(new { success = true });
                }
                TempData["ErrorMessage"] = "An error has occured while adding company.";
                return Json(new { success = false });
            }, "AddCompany");
        }

        [HttpPost]
        [Authorize(Policy = "AdminOrRecruiter")]
        [Route("update")]
        public async Task<IActionResult> UpdateCompany(CompanyViewModel company)
        {
            return await HandleExceptionAsync(async () =>
            {
                if (ModelState.IsValid)
                {
                    await _companyService.UpdateCompanyAsync(company);
                    TempData["SuccessMessage"] = "Company updated successfully!";
                    return Json(new { success = true });
                }
                TempData["ErrorMessage"] = "An error has occured while updating company.";
                return Json(new { success = false });
            }, "UpdateCompany");
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        [Route("archive")]
        public async Task<IActionResult> Archive(string id)
        {
            return await HandleExceptionAsync(async () =>
            {
                if (!string.IsNullOrEmpty(id))
                {
                    await _companyService.ArchiveCompanyAsync(id);
                    TempData["SuccessMessage"] = "Company archived successfully!";
                    return Json(new { success = true });
                }
                TempData["ErrorMessage"] = "An error has occured while archiving company.";
                return Json(new { success = false });
            }, "Archive");
        }
        #endregion
    }
}
