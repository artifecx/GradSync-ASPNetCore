﻿using Data;
using Data.Interfaces;
using Data.Repositories;
using Services.Interfaces;
using Services.ServiceModels;
using Services.Services;
using WebApp.Authentication;
using WebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace WebApp
{
    // Other services configuration
    internal partial class StartupConfigurer
    {
        /// <summary>
        /// Configures the other services.
        /// </summary>
        private void ConfigureOtherServices()
        {
            // Framework
            this._services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            this._services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            // Common
            this._services.AddScoped<TokenProvider>();
            this._services.TryAddSingleton<TokenProviderOptionsFactory>();
            this._services.TryAddSingleton<TokenValidationParametersFactory>();
            this._services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Services
            this._services.TryAddSingleton<TokenValidationParametersFactory>();
            this._services.AddScoped<IAccountService, AccountService>();
            this._services.AddScoped<IUserService, UserService>();
            this._services.AddScoped<IJobService, JobService>();
            this._services.AddScoped<ICompanyService, CompanyService>();
            this._services.AddScoped<IDashboardService, DashboardService>();
            this._services.AddSingleton<IEmailQueue, EmailQueue>();
            this._services.AddSingleton<IEmailService, EmailService>();
            this._services.AddHostedService<EmailBackgroundService>();
            this._services.AddScoped<IUserPreferencesService, UserPreferencesService>();

            // Repositories
            this._services.AddScoped<IUserRepository, UserRepository>();
            this._services.AddScoped<IJobRepository, JobRepository>();
            this._services.AddScoped<ICompanyRepository, CompanyRepository>();
            this._services.AddScoped<IUserPreferencesRepository, UserPreferencesRepository>();

            // Manager Class
            this._services.AddScoped<SignInManager>();

            this._services.AddHttpClient();
        }
    }
}
