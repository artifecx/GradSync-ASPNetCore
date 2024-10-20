using Data.Interfaces;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class ReferenceDataRepository : BaseRepository, IReferenceDataRepository
    {
        public ReferenceDataRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public async Task<List<EmploymentType>> GetEmploymentTypesAsync() =>
            await this.GetDbSet<EmploymentType>().AsNoTracking().ToListAsync();

        public async Task<List<StatusType>> GetStatusTypesAsync() =>
            await this.GetDbSet<StatusType>().AsNoTracking().ToListAsync();

        public async Task<List<SetupType>> GetWorkSetupsAsync() =>
            await this.GetDbSet<SetupType>().AsNoTracking().ToListAsync();

        public async Task<List<YearLevel>> GetYearLevelsAsync() =>
            await this.GetDbSet<YearLevel>().AsNoTracking().ToListAsync();

        public async Task<List<ApplicationStatusType>> GetApplicationStatusTypesAsync() =>
            await this.GetDbSet<ApplicationStatusType>().AsNoTracking().ToListAsync();

        public async Task<List<Program>> GetProgramsAsync() =>
            await this.GetDbSet<Program>()
                .Where(p => !p.IsDeleted)
                .Include(p => p.Department)
                .ThenInclude(d => d.College)
                .AsNoTracking()
                .ToListAsync();

        public async Task<List<Skill>> GetSkillsAsync() =>
            await this.GetDbSet<Skill>().AsNoTracking().ToListAsync();
    }
}
