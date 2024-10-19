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
                CreateMap<ApplicationViewModel, Application>().ReverseMap();
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
                    .ForMember(dest => dest.Departments, opt => opt.MapFrom(src => src.JobDepartments.Select(s => s.Department)));
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
                    .ForMember(dest => dest.ActiveJobListings, opt => opt.MapFrom(src => src.Recruiters.SelectMany(r => r.Jobs).Count(j => !j.IsArchived)));
            }
        }
    }
}
