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
using Data.Models;
using System.Collections.Generic;

namespace WebApp.Controllers
{
    /// <summary>
    /// Controller for handling user preferences.
    /// </summary>
    [Authorize]
    [Route("applications")]
    public partial class ApplicationController : ControllerBase<ApplicationController>
    {
        private readonly IApplicationService _applicationService;
        private readonly IJobService _jobService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationController"/> class.
        /// </summary>
        /// <param name="applicationService">The application service.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="tokenValidationParametersFactory">The token validation parameters factory.</param>
        /// <param name="tokenProviderOptionsFactory">The token provider options factory.</param>
        public ApplicationController(
            IApplicationService applicationService,
            IJobService jobService,
            IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IMapper mapper,
            TokenValidationParametersFactory tokenValidationParametersFactory,
            TokenProviderOptionsFactory tokenProviderOptionsFactory) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            this._applicationService = applicationService;
            this._jobService = jobService;
        }

        private async Task PopulateViewBagsAsync()
        {
            ViewBag.Programs = await _jobService.GetProgramsAsync();
            ViewBag.WorkSetups = await _jobService.GetWorkSetupsAsync();
            ViewBag.AppStatusTypes = await _applicationService.GetApplicationStatusTypesAsync();
        }
    }
}
