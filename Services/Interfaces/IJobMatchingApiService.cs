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
        Task MatchAndSaveApplicantJobsAsync(string userId);
        Task MatchAndSaveJobApplicantsAsync(string jobId);
        Task<JobApplicantMatch> CalculateSimilarityAsync(string jobId, string applicantId);
        Task<List<JobApplicantMatch>> CompareJobWithApplicantsAsync(string jobId);
        Task<List<JobApplicantMatch>> CompareApplicantWithJobsAsync(string applicantId);
    }
}
