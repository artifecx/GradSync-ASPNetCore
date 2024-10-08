using Services.Interfaces;
using Services.ServiceModels;
using WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebApp.Controllers
{
    /// <summary>
    /// Home Controller
    /// </summary>
    public class HomeController : ControllerBase<HomeController>
    {
        private readonly IDashboardService _dashboardService;
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
            IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _dashboardService = dashboardService;
        }

        /// <summary>
        /// Returns Home View.
        /// </summary>
        /// <returns> Home View </returns>
        /// 
        [Authorize(Policy = "Applicant")]
        public IActionResult Index()
        {
            return View();
        }
        
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Dashboard()
        {
            var dashboardData = await _dashboardService.GetDashboardData();
            return View(dashboardData);
        }

        public IActionResult InvalidAccess()
        {
            return View();
        }
    }
}
