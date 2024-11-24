using Services.Interfaces;
using WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WebApp.Authentication;
using System.Linq;

namespace WebApp.Controllers
{
    /// <summary>
    /// Home Controller
    /// </summary>
    [Route("home")]
    public partial class HomeController : ControllerBase<HomeController>
    {
        private readonly IDashboardService _dashboardService;
        private readonly IJobService _jobService;
        private readonly IOnboardingService _onboardingService;
        private readonly IReferenceDataService _referenceDataService;
        private readonly SignInManager _signInManager;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpContextAccessor"></param>  
        /// <param name="loggerFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="localizer"></param>
        /// <param name="mapper"></param>

        public HomeController(
            IDashboardService dashboardService,
            IJobService jobService,
            IOnboardingService onboardingService,
            IReferenceDataService referenceDataService,
            SignInManager signInManager,
            IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _dashboardService = dashboardService;
            _jobService = jobService;
            _onboardingService = onboardingService;
            _referenceDataService = referenceDataService;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Returns Home View.
        /// </summary>
        /// <returns> Home View </returns>
        [Route("/home")]
        [HttpGet]
        [Authorize(Policy = "ApplicantGateway")]
        public async Task<IActionResult> Index()
        {
            if (User.FindFirst("FromSignUp")?.Value == "true" && User.IsInRole("Applicant"))
                return RedirectToAction("Onboarding", "Home");

            var model = await _jobService.GetApplicantFeaturedJobsAsync(UserId);
            return View(model.OrderByDescending(m => m.MatchPercentage).ToList());
        }

        [Route("dashboard")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Dashboard()
        {
            var dashboardData = await _dashboardService.GetDashboardData();
            return View(dashboardData);
        }

        [Route("/contact-us")]
        [Authorize]
        public async Task<IActionResult> Contact()
        {
            return View();
        }

        [Route("/about-us")]
        [Authorize]
        public async Task<IActionResult> About()
        {
            return View();
        }

        [Route("/unauthorized")]
        public IActionResult InvalidAccess()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        [Route("/resume")]
        public async Task<IActionResult> GetResume(string id)
        {
            if (ModelState.IsValid && !string.IsNullOrEmpty(id))
            {
                var resume = await _onboardingService.GetApplicantResumeByIdAsync(id);
                if (resume == null)
                {
                    return NotFound();
                }

                return File(resume.FileContent, resume.FileType, resume.FileName);
            }
            return NotFound();
        }
    }
}
