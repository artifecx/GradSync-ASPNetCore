using Data.Models;
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
        Task<List<ApplicationStatusType>> GetApplicationStatusTypesAsync();
    }
}
