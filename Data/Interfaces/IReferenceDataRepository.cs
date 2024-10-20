using Data.Models;
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
        Task<List<YearLevel>> GetYearLevelsAsync();
        Task<List<Skill>> GetSkillsAsync();
        Task<List<ApplicationStatusType>> GetApplicationStatusTypesAsync();
        Task<List<Role>> GetUserRolesAsync();
    }
}
