﻿using AutoMapper;
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

        private class AutoMapperProfileConfiguration : Profile
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
                    .ReverseMap()
                    .ForMember(dest => dest.ActiveJobListings, opt => opt.MapFrom(src => src.Recruiters.Count(r => r.Jobs.Any(j => !j.IsArchived))));
            }
        }
    }
}
