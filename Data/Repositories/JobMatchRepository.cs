using Data.Dtos;
using Data.Interfaces;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class JobMatchRepository : BaseRepository, IJobMatchRepository
    {
        public JobMatchRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public async Task AddJobApplicantMatchesAsync(List<JobApplicantMatch> matches)
        {
            await this.GetDbSet<JobApplicantMatch>().AddRangeAsync(matches);
            await UnitOfWork.SaveChangesAsync();
        }

        public async Task DeleteJobApplicantMatchesByApplicantIdAsync(string applicantId) =>
            await this.GetDbSet<JobApplicantMatch>()
                .Where(jam => jam.UserId == applicantId)
                .ForEachAsync(jam => this.GetDbSet<JobApplicantMatch>().Remove(jam));

        public async Task DeleteJobApplicantMatchesByJobIdAsync(string jobId) =>
            await this.GetDbSet<JobApplicantMatch>()
                .Where(jam => jam.JobId == jobId)
                .ForEachAsync(jam => this.GetDbSet<JobApplicantMatch>().Remove(jam));

        public async Task<ApplicantDetailsDto> GetApplicantDetailsByIdAsync(string id) =>
            await this.GetDbSet<Applicant>()
                .Include(a => a.Resume)
                .Include(a => a.ApplicantSkills)
                    .ThenInclude(s => s.Skill)
                .Include(a => a.EducationalDetail)
                .Where(a => a.UserId == id 
                    && a.Resume != null 
                    && a.ApplicantSkills.Any())
                .Select(a => new ApplicantDetailsDto
                {
                    resume_text = a.Resume.ExtractedText,
                    technical_skills = a.ApplicantSkills
                        .Where(s => s.Type == "Technical" || s.Type == "Certification")
                        .Select(s => s.Skill.Name)
                        .ToList(),
                    cultural_skills = a.ApplicantSkills
                        .Where(s => s.Type == "Cultural")
                        .Select(s => s.Skill.Name)
                        .ToList(),
                    department_id = a.EducationalDetail.DepartmentId
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

        public async Task<List<ApplicantDetailsDto>> GetAllApplicantDetailsAsync(HashSet<string> departmentIds) =>
            await this.GetDbSet<Applicant>()
                .Include(a => a.Resume)
                .Include(a => a.ApplicantSkills)
                    .ThenInclude(s => s.Skill)
                .Include(a => a.EducationalDetail)
                .Include(a => a.JobApplicantMatches)
                .Where(a => a.Resume != null 
                    && a.ApplicantSkills.Any() 
                    && departmentIds.Contains(a.EducationalDetail.DepartmentId))
                .Select(a => new ApplicantDetailsDto
                {
                    applicant_id = a.UserId,
                    resume_text = a.Resume.ExtractedText,
                    technical_skills = a.ApplicantSkills
                        .Where(s => s.Type == "Technical" || s.Type == "Certification")
                        .Select(s => s.Skill.Name)
                        .ToList(),
                    cultural_skills = a.ApplicantSkills
                        .Where(s => s.Type == "Cultural")
                        .Select(s => s.Skill.Name)
                        .ToList(),
                })
                .AsNoTracking()
                .ToListAsync();

        public async Task<JobDetailsDto> GetJobDetailsByIdAsync(string id) =>
           await this.GetDbSet<Job>()
                .Include(j => j.JobSkills)
                    .ThenInclude(js => js.Skill)
                .Include(j => j.JobPrograms)
                    .ThenInclude(jp => jp.Program)
                .Where(j => !j.IsArchived 
                    && j.JobId == id 
                    && j.SkillWeights != null 
                    && j.JobSkills.Any())
                .Select(j => new JobDetailsDto
                {
                    title = j.Title,
                    description = j.Description,
                    technical_skills = j.JobSkills
                        .Where(s => s.Type == "Technical" || s.Type == "Certification")
                        .Select(s => s.Skill.Name)
                        .ToList(),
                    cultural_skills = j.JobSkills
                        .Where(s => s.Type == "Cultural")
                        .Select(s => s.Skill.Name)
                        .ToList(),
                    tech_cult_weight = j.SkillWeights,
                    department_ids = j.JobPrograms
                        .Select(jp => jp.Program.DepartmentId)
                        .Distinct()
                        .ToList()
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

        public async Task<List<JobDetailsDto>> GetAllJobDetailsAsync(string departmentId) =>
            await this.GetDbSet<Job>()
                .Include(j => j.JobSkills)
                    .ThenInclude(js => js.Skill)
                .Include(j => j.JobPrograms)
                    .ThenInclude(jp => jp.Program)
                .Where(j => !j.IsArchived 
                    && j.SkillWeights != null 
                    && j.JobSkills.Any() 
                    && j.JobPrograms.Any(jp => jp.Program.DepartmentId == departmentId))
                .Select(j => new JobDetailsDto
                {
                    job_id = j.JobId,
                    title = j.Title,
                    description = j.Description,
                    technical_skills = j.JobSkills
                        .Where(s => s.Type == "Technical" || s.Type == "Certification")
                        .Select(s => s.Skill.Name)
                        .ToList(),
                    cultural_skills = j.JobSkills
                        .Where(s => s.Type == "Cultural")
                        .Select(s => s.Skill.Name)
                        .ToList(),
                    tech_cult_weight = j.SkillWeights
                })
                .AsNoTracking()
                .ToListAsync();
    }
}
