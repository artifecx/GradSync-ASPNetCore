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

        public async Task<ApiResponseServiceModel> CalculateSimilarityAsync(string jobId, string applicantId)
        {
            var jobDetails = await _repository.GetJobDetailsByIdAsync(jobId);
            if (jobDetails == null) return new ApiResponseServiceModel();

            var applicantDetails = await _repository.GetApplicantDetailsByIdAsync(applicantId);
            if (applicantDetails == null) return new ApiResponseServiceModel();

            var payload = new
            {
                job = jobDetails,
                applicant = applicantDetails
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/calculate-similarity", content);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var matchPercentage = JsonSerializer.Deserialize<ApiResponseServiceModel>(responseString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return matchPercentage;
        }


        public async Task<ApiResponseServiceModel> CompareJobWithApplicantsAsync(string jobId)
        {
            var jobDetails = await _repository.GetJobDetailsByIdAsync(jobId);
            if (jobDetails == null) return new ApiResponseServiceModel();

            var applicants = await _repository.GetAllApplicantDetailsAsync();
            if (applicants == null) return new ApiResponseServiceModel();

            var payload = new
            {
                job = jobDetails,
                applicants
            };

            var results = await GetResultsFromApi(payload, "/compare-job-with-applicants");

            var applicantIds = applicants.Select(applicant => applicant.GetType().GetProperty("applicant_id")?.GetValue(applicant)?.ToString()).ToList();
            var matches = await GenerateJobApplicantMatchesAsync(results, "jobs", jobId, applicantIds);

            await _repository.AddJobApplicantMatchesAsync(matches);

            return results;
        }

        public async Task<ApiResponseServiceModel> CompareApplicantWithJobsAsync(string applicantId)
        {
            var applicantDetails = await _repository.GetApplicantDetailsByIdAsync(applicantId);
            if (applicantDetails == null) return new ApiResponseServiceModel();

            var jobs = await _repository.GetAllJobDetailsAsync();
            if (jobs == null) return new ApiResponseServiceModel();

            var payload = new
            {
                applicant = applicantDetails,
                jobs
            };

            var results = await GetResultsFromApi(payload, "/compare-applicant-with-jobs");

            var jobIds = jobs.Select(job => job.GetType().GetProperty("job_id")?.GetValue(job)?.ToString()).ToList();
            var matches = await GenerateJobApplicantMatchesAsync(results, "jobs", applicantId, jobIds);

            await _repository.AddJobApplicantMatchesAsync(matches);

            return results;
        }

        private async Task<ApiResponseServiceModel> GetResultsFromApi(object payload, string route)
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
                _logger.LogError(ex, "Error occurred while calling the external comparison service.");
                return new ApiResponseServiceModel();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error occurred while deserializing the response from the external comparison service.");
                return new ApiResponseServiceModel();
            }

            return results;
        }

        private Task<List<JobApplicantMatch>> GenerateJobApplicantMatchesAsync(
            ApiResponseServiceModel results,
            string matchType,
            string fixedId,
            List<string> ids)
        {
            if (results?.MatchPercentages == null || !results.MatchPercentages.Any())
            {
                return Task.FromResult(new List<JobApplicantMatch>());
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

                    return null;
                })
                .Where(match => match != null)
                .ToList();

            return Task.FromResult(jobApplicantMatches);
        }
    }
}