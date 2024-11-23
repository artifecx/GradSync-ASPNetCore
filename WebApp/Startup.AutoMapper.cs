using AutoMapper;
using Data.Models;
using Services.ServiceModels;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace WebApp
{
    // AutoMapper configuration
    internal partial class StartupConfigurer
    {
        /// <summary>
        /// Configure auto mapper
        /// </summary>
        private void ConfigureAutoMapper()
        {
            var mapperConfiguration = new MapperConfiguration(config =>
            {
                config.AddProfile(new AutoMapperProfileConfiguration());
            });

            this._services.AddSingleton<IMapper>(sp => mapperConfiguration.CreateMapper());
        }

        private sealed class AutoMapperProfileConfiguration : Profile
        {
            public AutoMapperProfileConfiguration()
            {
                CreateMap<UserViewModel, User>().ReverseMap();
                CreateMap<AccountServiceModel, User>().ReverseMap();
                CreateMap<AccountServiceModel, UserViewModel>().ReverseMap();
                CreateMap<ApplicationViewModel, Application>()
                    .ReverseMap()
                    .ForMember(dest => dest.ApplicantId, opt => opt.MapFrom(src => src.UserId))
                    .ForMember(dest => dest.Applicant, opt => opt.MapFrom(src => src.User))
                    .ForMember(dest => dest.ApplicantName, opt => opt.MapFrom(src => 
                        $"{ src.User.User.FirstName } { src.User.User.MiddleName } { src.User.User.LastName } { src.User.User.Suffix}"))
                    .ForMember(dest => dest.RecruiterName, opt => opt.MapFrom(src => 
                        $"{ src.Job.PostedBy.User.FirstName } { src.Job.PostedBy.User.MiddleName } { src.Job.PostedBy.User.LastName } { src.Job.PostedBy.User.Suffix}"));
                CreateMap<JobViewModel, Job>()
                    .ForMember(dest => dest.JobId, opt => opt.Ignore())
                    .ForMember(dest => dest.PostedById, opt => opt.Ignore())
                    .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                    .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore())
                    .ForMember(dest => dest.EmploymentType, opt => opt.Ignore())
                    .ForMember(dest => dest.PostedBy, opt => opt.Ignore())
                    .ForMember(dest => dest.Salary, opt => opt.Ignore())
                    .ForMember(dest => dest.Schedule, opt => opt.Ignore())
                    .ForMember(dest => dest.SetupType, opt => opt.Ignore())
                    .ForMember(dest => dest.StatusTypeId, opt => opt.Ignore())
                    .ForMember(dest => dest.StatusType, opt => opt.Ignore())
                    .ForMember(dest => dest.YearLevel, opt => opt.Ignore())
                    .ReverseMap()
                    .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.PostedBy.Company))
                    .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.JobSkills.Select(s => s.Skill)))
                    .ForMember(dest => dest.Programs, opt => opt.MapFrom(src => src.JobPrograms.Select(s => s.Program)))
                    .ForMember(dest => dest.SkillWeights, opt => opt.MapFrom(src => src.SkillWeights));
                CreateMap<JobApplicantMatch, FeaturedJobsViewModel>()
                    .ForMember(dest => dest.JobId, opt => opt.MapFrom(src => src.JobId))
                    .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Job.Title))
                    .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Job.Location))
                    .ForMember(dest => dest.Salary, opt => opt.MapFrom(src => src.Job.Salary))
                    .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.Job.JobSkills.Select(s => s.Skill)))
                    .ForMember(dest => dest.EmploymentTypeName, opt => opt.MapFrom(src => src.Job.EmploymentType.Name))
                    .ForMember(dest => dest.SetupTypeName, opt => opt.MapFrom(src => src.Job.SetupType.Name))
                    .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Job.Company.Name))
                    .ForMember(dest => dest.MatchPercentage, opt => opt.MapFrom(src => src.MatchPercentage));
                CreateMap<CompanyViewModel, Company>()
                    .ForMember(dest => dest.CompanyId, opt => opt.Ignore())
                    .ForMember(dest => dest.CompanyLogoId, opt => opt.Ignore())
                    .ForMember(dest => dest.CompanyLogo, opt => opt.Ignore())
                    .ForMember(dest => dest.MemorandumOfAgreementId, opt => opt.Ignore())
                    .ForMember(dest => dest.MemorandumOfAgreement, opt => opt.Ignore())
                    .ForMember(dest => dest.Recruiters, opt => opt.Ignore())
                    .ForMember(dest => dest.IsVerified, opt => opt.Ignore())
                    .ForMember(dest => dest.IsArchived, opt => opt.Ignore())
                    .ReverseMap()
                    .ForMember(dest => dest.ActiveJobListings, opt => opt.MapFrom(src => src.Jobs.Count(j => !j.IsArchived)));
            }
        }
    }
}
