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

namespace WebApp.Controllers
{
    /// <summary>
    /// Controller for handling user preferences.
    /// </summary>
    [Authorize]
    [Route("applications")]
    public class ApplicationController : ControllerBase<ApplicationController>
    {
        private readonly IApplicationService _applicationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserPreferencesController"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="userPreferencesService">The user preferences service.</param>
        /// <param name="tokenValidationParametersFactory">The token validation parameters factory.</param>
        /// <param name="tokenProviderOptionsFactory">The token provider options factory.</param>
        public ApplicationController(
            IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IMapper mapper,
            IApplicationService applicationService,
            TokenValidationParametersFactory tokenValidationParametersFactory,
            TokenProviderOptionsFactory tokenProviderOptionsFactory) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            this._applicationService = applicationService;
        }

        [Route("all")]
        public async Task<IActionResult> Index(int pageIndex = 1, string search = "", string statusFilter = "")
        {
            var pageSize = 10;
            var model = await _applicationService.GetApplicationsAsync(pageIndex, pageSize, search, statusFilter);

            ViewData["Search"] = search;
            ViewData["StatusFilter"] = statusFilter;

            return View(model);
        }
    }
}
