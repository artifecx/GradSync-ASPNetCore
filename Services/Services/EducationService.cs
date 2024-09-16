using Data.Interfaces;
using Data.Models;
using Services.Interfaces;
using Services.Manager;
using Services.ServiceModels;
using AutoMapper;
using System;
using System.IO;
using System.Linq;
using static Resources.Constants.Enums;
using static Services.Exceptions.UserExceptions;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Services.Services
{
    public class EducationService : IEducationService
    {
        private readonly IEducationRepository _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountService"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="mapper">The mapper.</param>
        public EducationService(IEducationRepository repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<EducationViewModel> GetAllAsync()
        {
            var model = new EducationViewModel();
            model.Colleges = await _repository.GetAllCollegesAsync();
            model.Departments = await _repository.GetAllDepartmentsAsync();
            model.YearLevels = await _repository.GetAllYearLevelsAsync();

            return model;
        }

        public async Task AddCollegeAsync(College college)
        {
            if (college == null)
            {
                throw new ArgumentNullException(nameof(college));
            }

            await _repository.AddCollegeAsync(college);
        }

        public async Task AddDepartmentAsync(Department department)
        {
            if (department == null)
            {
                throw new ArgumentNullException(nameof(department));
            }

            await _repository.AddDepartmentAsync(department);
        }
    }
}
