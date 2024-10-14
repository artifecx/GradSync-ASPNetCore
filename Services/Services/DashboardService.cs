using Data.Interfaces;
using Data.Models;
using Services.Interfaces;
using Services.Manager;
using Services.ServiceModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Data.Repositories;
using System.Threading.Tasks;
using Resources.Messages;
using static Services.Exceptions.UserExceptions;

namespace Services.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJobRepository _jobRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardService"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="jobRepository">The job repository.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        public DashboardService(IUserRepository userRepository, IJobRepository jobRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _jobRepository = jobRepository;
            _mapper = mapper;
        }

        public async Task<DashboardViewModel> GetDashboardData()
        {
            var users = await _userRepository.GetAllUsersNoIncludesAsync();
            var applications = await _jobRepository.GetAllApplicationsNoIncludesAsync();
            var jobs = await _jobRepository.GetAllJobsDepartmentsIncludeAsync();

            var dashboardViewModel = new DashboardViewModel();

            SetUserData(dashboardViewModel, users);
            SetApplicationData(dashboardViewModel, applications);
            SetJobData(dashboardViewModel, jobs);

            return dashboardViewModel;
        }

        private static void SetUserData(DashboardViewModel model, List<User> users)
        {
            model.TotalUsers = users.AsParallel().Count();
            model.TotalAdmins = users.AsParallel().Count(u => u.RoleId == "NLO" || u.RoleId == "Admin");
            model.TotalApplicants = users.AsParallel().Count(u => u.RoleId == "Applicant");
            model.TotalRecruiters = users.AsParallel().Count(u => u.RoleId == "Recruiter");
        }

        private static void SetApplicationData(DashboardViewModel model, List<Application> applications)
        {
            model.TotalApplicationsAllTime = applications
                .AsParallel()
                .Count();
            model.TotalApplicationsThisYear = applications
                .AsParallel()
                .Count(a => a.CreatedDate.Year == DateTime.Now.Year);
            model.TotalApplicationsPerMonth = applications
                .AsParallel()
                .GroupBy(a => a.CreatedDate.Month)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        private static void SetJobData(DashboardViewModel model, List<Job> jobs)
        {
            model.TotalJobsAllTime = jobs.AsParallel().Count();
            model.TotalJobsPerMonth = jobs.GroupBy(j => j.CreatedDate.Month)
                .AsParallel()
                .ToDictionary(g => g.Key, g => g.Count());
            model.TotalJobsPerEmploymentType = jobs
                .AsParallel()
                .GroupBy(j => j.EmploymentTypeId)
                .ToDictionary(g => g.Key, g => g.Count());
            model.TotalJobsPerStatusType = jobs
                .AsParallel()
                .GroupBy(j => j.StatusTypeId)
                .ToDictionary(g => g.Key, g => g.Count());
            model.TotalJobsPerSetupType = jobs
                .AsParallel()
                .GroupBy(j => j.SetupTypeId)
                .ToDictionary(g => g.Key, g => g.Count());
            model.TotalJobsPerDepartment = jobs.SelectMany(j => j.Departments)
                .AsParallel()
                .GroupBy(d => d.ShortName)
                .ToDictionary(g => g.Key, g => g.Count());
            model.JobSalaryDistribution = jobs
                .AsParallel()
                .GroupBy(j => GetSalaryRangeFromRangeString(j.Salary))
                .ToDictionary(g => g.Key, g => g.Count());
        }

        private static string GetSalaryRangeFromRangeString(string salaryRange)
        {
            int lowerBound = ParseSalary(salaryRange);
            return GetSalaryRange(lowerBound);
        }

        private static int ParseSalary(string salaryRange)
        {
            var parts = salaryRange.Split('-');
            string lowerBoundStr = parts[0].Replace("Php", "").Replace(",", "").Trim();
            int lowerBound = int.Parse(lowerBoundStr);
            return lowerBound;
        }

        private static string GetSalaryRange(int salary)
        {
            if (salary < 10000)
                return "Php 0 - Php 9,999";
            else if(salary <= 20000)
                return "Php 10,000 - Php 20,000";
            else if (salary <= 30000)
                return "Php 20,001 - Php 30,000";
            else if (salary <= 40000)
                return "Php 30,001 - Php 40,000";
            else if (salary <= 50000)
                return "Php 40,001 - Php 50,000";
            else if (salary <= 60000)
                return "Php 50,001 - Php 60,000";
            else
                return "> Php 60,000";
        }
    }
}
