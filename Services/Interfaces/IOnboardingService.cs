using Data.Models;
using Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IOnboardingService
    {
        Task<Resume> GetApplicantResumeByIdAsync(string id);
        Task<User> CompleteOnboardingAsync(OnboardingViewModel model);
    }
}
