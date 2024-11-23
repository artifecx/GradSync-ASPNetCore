using Data.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IJobMatchingApiService
    {
        Task<ApiResponseServiceModel> CalculateSimilarityAsync(string jobId, string applicantId);
        Task<ApiResponseServiceModel> CompareJobWithApplicantsAsync(string jobId);
        Task<ApiResponseServiceModel> CompareApplicantWithJobsAsync(string applicantId);
    }
}
