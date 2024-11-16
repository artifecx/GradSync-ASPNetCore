using Data.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IReferenceDataService
    {
        Task<List<EmploymentType>> GetEmploymentTypesAsync();
        Task<List<StatusType>> GetStatusTypesAsync();
        Task<List<SetupType>> GetWorkSetupsAsync();
        Task<List<Program>> GetProgramsAsync();
        Task<List<YearLevel>> GetYearLevelsAsync();
        Task<List<Skill>> GetSkillsAsync();
        Task<List<Skill>> GetSoftSkillsAsync();
        Task<List<Skill>> GetTechnicalSkillsAsync();
        Task<List<Skill>> GetCertificationSkillsAsync();
        Task<List<ApplicationStatusType>> GetApplicationStatusTypesAsync();
        Task<List<Role>> GetUserRolesAsync();
    }
}
