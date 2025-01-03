﻿using Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data.Interfaces
{
    public interface IReferenceDataRepository
    {
        Task<List<EmploymentType>> GetEmploymentTypesAsync();
        Task<List<StatusType>> GetStatusTypesAsync();
        Task<List<SetupType>> GetWorkSetupsAsync();
        Task<List<Program>> GetProgramsAsync();
        Task<List<Department>> GetDepartmentsAsync();
        Task<List<College>> GetCollegesAsync();
        Task<List<YearLevel>> GetYearLevelsAsync();
        Task<List<Skill>> GetSkillsAsync();
        Task<List<Skill>> GetSoftSkillsAsync();
        Task<List<Skill>> GetTechnicalSkillsAsync();
        Task<List<Skill>> GetCertificationSkillsAsync();
        Task<List<ApplicationStatusType>> GetApplicationStatusTypesAsync();
        Task<List<Role>> GetUserRolesAsync();
    }
}
