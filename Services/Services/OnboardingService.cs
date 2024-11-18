using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Data.Interfaces;
using Data.Models;
using Services.Interfaces;
using Services.ServiceModels;
using static Resources.Constants.UserRoles;
using static Resources.Messages.ErrorMessages;
using static Services.Exceptions.UserExceptions;
using System.IO;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Services.Services
{
    /// <summary>
    /// Service class for handling an <see cref="Applicant"/>'s first login.
    /// </summary>
    public class OnboardingService : IOnboardingService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPdfTextExtractorService _pdfTextExtractorService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OnboardingService"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="logger">The logger.</param>
        public OnboardingService
            (
                IUserRepository userRepository,
                IPdfTextExtractorService pdfTextExtractorService,
                ILogger<OnboardingService> logger
            )
        {
            _userRepository = userRepository;
            _pdfTextExtractorService = pdfTextExtractorService;
        }

        public async Task<User> CompleteOnboardingAsync(OnboardingViewModel model)
        {
            var user = await _userRepository.GetUserApplicantForFirstLoginAsync(model.UserId);
            ValidateUser(user);

            user.Applicant.Resume = await ProcessResumeAsync(model.Resume);

            user.Applicant.EducationalDetail = CreateEducationalDetails(model);
            user.Applicant.EducationalDetailId = user.Applicant.EducationalDetail.EducationalDetailId;
            user.Applicant.ApplicantSkills = new[]
            {
                new { Skills = model.SkillsT ?? Enumerable.Empty<Skill>(), Type = "Technical" },
                new { Skills = model.SkillsS ?? Enumerable.Empty<Skill>(), Type = "Cultural" },
                new { Skills = model.SkillsC ?? Enumerable.Empty<Skill>(), Type = "Certification" }
            }
            .SelectMany(group => group.Skills.Select(skill => new ApplicantSkill
            {
                ApplicantSkillId = Guid.NewGuid().ToString(),
                UserId = user.UserId,
                SkillId = skill.SkillId,
                Type = group.Type
            }))
            .ToList();

            user.FromSignUp = false;
            await _userRepository.UpdateUserAsync(user);

            return user;
        }

        private static void ValidateUser(User user)
        {
            if (user == null || user.RoleId != Role_Applicant)
            {
                throw new UserException("Applicant not found or invalid role!");
            }
        }

        private async Task<Resume> ProcessResumeAsync(IFormFile resume) =>
            resume != null && resume.Length > 0 
                ? await CreateResumeAsync(resume) 
                : throw new UserException("Resume cannot be empty!");

        private async Task<Avatar> ProcessAvatarAsync(IFormFile avatar) => 
            avatar != null && avatar.Length > 0 
                ? await CreateAvatarAsync(avatar) : null;

        private EducationalDetail CreateEducationalDetails(OnboardingViewModel model)
        {
            return new EducationalDetail
            {
                EducationalDetailId = Guid.NewGuid().ToString(),
                IdNumber = model.IdNumber,
                YearLevelId = model.YearLevelId,
                ProgramId = model.ProgramId,
                DepartmentId = model.DepartmentId,
                CollegeId = model.CollegeId,
            };
        }

        private async Task<Resume> CreateResumeAsync(IFormFile file)
        {
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                return new Resume
                {
                    ResumeId = Guid.NewGuid().ToString(),
                    FileName = file.FileName,
                    FileContent = stream.ToArray(),
                    FileType = file.ContentType,
                    ExtractedText = _pdfTextExtractorService.ExtractTextFromPdf(stream),
                    UploadedDate = DateTime.Now
                };
            }
        }

        private async Task<Avatar> CreateAvatarAsync(IFormFile file)
        {
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                return new Avatar
                {
                    AvatarId = Guid.NewGuid().ToString(),
                    FileName = file.FileName,
                    FileContent = stream.ToArray(),
                    FileType = file.ContentType,
                    UploadedDate = DateTime.Now
                };
            }
        }
    }
}
