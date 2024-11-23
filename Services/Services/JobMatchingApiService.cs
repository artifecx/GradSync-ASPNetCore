using Data.Interfaces;
using Data.Models;
using Services.ServiceModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using static Services.Exceptions.JobMatchingExceptions;
using Data.Repositories;

namespace Services.Services
{
    public class JobMatchingApiService : IJobMatchingApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IJobMatchRepository _repository;
        private readonly ILogger<JobMatchingApiService> _logger;

        public JobMatchingApiService(
            HttpClient httpClient, 
            IConfiguration configuration, 
            IJobMatchRepository repository,
            IJobRepository jobRepository,
            ILogger<JobMatchingApiService> logger)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(configuration.GetConnectionString("JobMatchingAPI"));
            _repository = repository;
            _logger = logger;
        }

        public async Task MatchAndSaveApplicantJobsAsync(string userId)
        {
            var matches = await CompareApplicantWithJobsAsync(userId);
            if (matches != null && matches.Any())
            {
                await _repository.AddJobApplicantMatchesAsync(matches);
            }
        }

        public async Task MatchAndSaveJobApplicantsAsync(string jobId)
        {
            var matches = await CompareJobWithApplicantsAsync(jobId);
            if (matches != null && matches.Any())
            {
                await _repository.AddJobApplicantMatchesAsync(matches);
            }
        }

        public async Task UpdateMatchApplicantJobsAsync(string applicantId)
        {
            await _repository.DeleteJobApplicantMatchesByApplicantIdAsync(applicantId);
            await MatchAndSaveApplicantJobsAsync(applicantId);
        }

        public async Task UpdateMatchJobApplicantsAsync(string jobId)
        {
            await _repository.DeleteJobApplicantMatchesByJobIdAsync(jobId);
            await MatchAndSaveJobApplicantsAsync(jobId);
        }
        

        public async Task<JobApplicantMatch> CalculateSimilarityAsync(string jobId, string applicantId)
        {
            var jobDetails = await _repository.GetJobDetailsByIdAsync(jobId);
            if (jobDetails == null) return null;

            var applicantDetails = await _repository.GetApplicantDetailsByIdAsync(applicantId);
            if (applicantDetails == null) return null;

            var payload = new
            {
                job = jobDetails,
                applicant = applicantDetails
            };

            var result = await GetApiResponse(payload, "/calculate-similarity");
            var matchPercentage = await GenerateJobApplicantMatchAsync(result, jobId, applicantId);

            return matchPercentage;
        }

        public async Task<List<JobApplicantMatch>> CompareJobWithApplicantsAsync(string jobId)
        {
            var jobDetails = await _repository.GetJobDetailsByIdAsync(jobId);
            if (jobDetails == null) return null;

            var applicants = await _repository.GetAllApplicantDetailsAsync(jobDetails.department_ids.ToHashSet());
            if (applicants == null) return null;

            var payload = new
            {
                job = jobDetails,
                applicants
            };

            var results = await GetApiResponse(payload, "/compare-job-with-applicants");

            var applicantIds = applicants.Select(applicant => applicant.applicant_id).ToList();
            var matchPercentages = await GenerateJobApplicantMatchesAsync(results, "applicants", jobId, applicantIds);

            return matchPercentages;
        }

        public async Task<List<JobApplicantMatch>> CompareApplicantWithJobsAsync(string applicantId)
        {
            var applicantDetails = await _repository.GetApplicantDetailsByIdAsync(applicantId);
            if (applicantDetails == null) return null;

            var jobs = await _repository.GetAllJobDetailsAsync(applicantDetails.department_id);
            if (jobs == null) return null;

            var payload = new
            {
                applicant = applicantDetails,
                jobs
            };

            var results = await GetApiResponse(payload, "/compare-applicant-with-jobs");

            var jobIds = jobs.Select(job => job.job_id).ToList();
            var matchPercentages = await GenerateJobApplicantMatchesAsync(results, "jobs", applicantId, jobIds);

            return matchPercentages;
        }

        private async Task<ApiResponseServiceModel> GetApiResponse(object payload, string route)
        {
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            ApiResponseServiceModel results = null;

            try
            {
                var response = await _httpClient.PostAsync(route, content);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                results = JsonSerializer.Deserialize<ApiResponseServiceModel>(responseString, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (HttpRequestException ex)
            {
                var message = "Error occurred while calling the external comparison service.";
                _logger.LogError(ex, message);
                throw new JobMatchingException(message);
            }
            catch (JsonException ex)
            {
                var message = "Error occurred while deserializing the response from the external comparison service.";
                _logger.LogError(ex, message);
                throw new JobMatchingException(message);
            }

            return results;
        }

        private Task<JobApplicantMatch> GenerateJobApplicantMatchAsync(
            ApiResponseServiceModel result, 
            string jobId, 
            string applicantId)
        {
            if (result?.MatchPercentages == null || !result.MatchPercentages.Any())
            {
                throw new JobMatchingException("No match percentages found in the response.");
            }

            var match = result.MatchPercentages
                .Select(kvp =>
                {
                    if (int.TryParse(kvp.Key, out int index))
                    {
                        return new JobApplicantMatch
                        {
                            JobApplicantMatchId = Guid.NewGuid().ToString(),
                            UserId = applicantId,
                            JobId = jobId,
                            MatchPercentage = kvp.Value
                        };
                    }

                    throw new JobMatchingException("Invalid index found in the response.");
                })
                .FirstOrDefault(match => match != null);

            return Task.FromResult(match);
        }

        private Task<List<JobApplicantMatch>> GenerateJobApplicantMatchesAsync(
            ApiResponseServiceModel results,
            string matchType,
            string fixedId,
            List<string> ids)
        {
            if (results?.MatchPercentages == null || !results.MatchPercentages.Any())
            {
                throw new JobMatchingException("No match percentages found in the response.");
            }

            var jobApplicantMatches = results.MatchPercentages
                .Select(kvp =>
                {
                    if (int.TryParse(kvp.Key, out int index) &&
                        index >= 0 &&
                        index < ids.Count)
                    {
                        return new JobApplicantMatch
                        {
                            JobApplicantMatchId = Guid.NewGuid().ToString(),
                            UserId = matchType == "jobs" ? fixedId : ids[index],
                            JobId = matchType == "applicants" ? fixedId : ids[index],
                            MatchPercentage = kvp.Value
                        };
                    }

                    throw new JobMatchingException("Invalid index found in the response.");
                })
                .Where(match => match != null)
                .ToList();

            return Task.FromResult(jobApplicantMatches);
        }
    }
}