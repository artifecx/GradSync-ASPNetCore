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
                CreateMap<JobViewModel, Job>()
                    .ReverseMap()
                    .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.PostedBy.Company));
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
