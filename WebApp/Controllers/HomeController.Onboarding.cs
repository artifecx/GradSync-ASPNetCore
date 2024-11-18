using Services.ServiceModels;
using WebApp.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;

namespace WebApp.Controllers
{
    /// <summary>
    /// Home Controller | First Login Screen
    /// </summary>
    public partial class HomeController : ControllerBase<HomeController>
    {
        [Route("/welcome")]
        [HttpGet]
        [Authorize(Policy = "ApplicantOnboarding")]
        public async Task<IActionResult> Onboarding()
        {
            await PopulateViewBagsAsync();
            var model = new OnboardingViewModel
            {
                FirstName = UserName
            };
            
            return View(model);
        } 

        [Route("/welcome-complete")]
        [HttpPost]
        [Authorize(Policy = "ApplicantOnboarding")]
        public async Task<IActionResult> CompleteOnboarding(OnboardingViewModel model)
        {
            return await HandleExceptionAsync(async () =>
            {
                model.UserId = UserId;
                if (ModelState.IsValid)
                {
                    var user = await _welcomeService.CompleteOnboardingAsync(model);
                    await _signInManager.SignInAsync(user);

                    TempData["SuccessMessage"] = "Welcome to GradSync!";
                    return Json(new { success = true });
                }
                await PopulateViewBagsAsync();
                TempData["ErrorMessage"] = "An error has occurred.";
                return Json(new { success = false });
            }, "CompleteOnboarding");
        }

        private async Task PopulateViewBagsAsync()
        {
            ViewBag.YearLevels = await _referenceDataService.GetYearLevelsAsync();
            ViewBag.Programs = (await _referenceDataService.GetProgramsAsync())
                .Select(p => new Data.Models.Program { 
                    ProgramId = p.ProgramId, 
                    Name = p.Name, 
                    ShortName = p.ShortName, 
                    DepartmentId = p.DepartmentId 
                }).ToList();
            ViewBag.Departments = await _referenceDataService.GetDepartmentsAsync();
            ViewBag.Colleges = await _referenceDataService.GetCollegesAsync();
            ViewBag.SkillsSoft = await _referenceDataService.GetSoftSkillsAsync();
            ViewBag.SkillsTechnical = await _referenceDataService.GetTechnicalSkillsAsync();
            ViewBag.SkillsCertification = await _referenceDataService.GetCertificationSkillsAsync();
        }
    }
}
