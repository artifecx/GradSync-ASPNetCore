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
using static Resources.Messages.ErrorMessages;
using static Resources.Messages.SuccessMessages;
using Services.Services;
using System.Security.Claims;

namespace WebApp.Controllers
{
    /// <summary>
    /// Controller for job application related operations
    /// </summary>
    public partial class ApplicationController : ControllerBase<ApplicationController>
    {
        [HttpGet]
        [Route("admin/all")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetAllApplicationsAdmin(ApplicationFilter filters)
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
                return RedirectToAction("InvalidAccess", "Home");
            }, "GetAllApplicationsAdmin");
        }
    }
}
