using System;
using System.Collections.Generic;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data;

public partial class GradSyncDbContext : DbContext
{
    public GradSyncDbContext()
    {
    }

    public GradSyncDbContext(DbContextOptions<GradSyncDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdditionalInformation> AdditionalInformations { get; set; }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Applicant> Applicants { get; set; }

    public virtual DbSet<Application> Applications { get; set; }

    public virtual DbSet<ApplicationStatusType> ApplicationStatusTypes { get; set; }

    public virtual DbSet<Avatar> Avatars { get; set; }

    public virtual DbSet<College> Colleges { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<CompanyLogo> CompanyLogos { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<EducationalDetail> EducationalDetails { get; set; }

    public virtual DbSet<EmploymentType> EmploymentTypes { get; set; }

    public virtual DbSet<Job> Jobs { get; set; }

    public virtual DbSet<MemorandumOfAgreement> MemorandumOfAgreements { get; set; }

    public virtual DbSet<Recruiter> Recruiters { get; set; }

    public virtual DbSet<Resume> Resumes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SavedJob> SavedJobs { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<SetupType> SetupTypes { get; set; }

    public virtual DbSet<Skill> Skills { get; set; }

    public virtual DbSet<StatusType> StatusTypes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<YearLevel> YearLevels { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdditionalInformation>(entity =>
        {
            entity.ToTable("AdditionalInformation");

            entity.HasIndex(e => e.Type, "IX_AdditionalInformation_Type");

            entity.HasIndex(e => e.UploadedDate, "IX_AdditionalInformation_UploadedDate");

            entity.Property(e => e.AdditionalInformationId).HasMaxLength(255);
            entity.Property(e => e.FileContent).IsRequired();
            entity.Property(e => e.FileName)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.FileType)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.UploadedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.ToTable("Admin");

            entity.Property(e => e.UserId).HasMaxLength(255);

            entity.HasOne(d => d.User).WithOne(p => p.Admin)
                .HasForeignKey<Admin>(d => d.UserId)
                .HasConstraintName("FK_Admin_User");
        });

        modelBuilder.Entity<Applicant>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.ToTable("Applicant");

            entity.Property(e => e.UserId).HasMaxLength(255);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.EducationalDetailsId).HasMaxLength(255);
            entity.Property(e => e.IdNumber).HasMaxLength(255);
            entity.Property(e => e.ResumeId).HasMaxLength(255);

            entity.HasOne(d => d.EducationalDetails).WithMany(p => p.Applicants)
                .HasForeignKey(d => d.EducationalDetailsId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Applicant_EducationalDetails");

            entity.HasOne(d => d.Resume).WithMany(p => p.Applicants)
                .HasForeignKey(d => d.ResumeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Applicant_Resume");

            entity.HasOne(d => d.User).WithOne(p => p.Applicant)
                .HasForeignKey<Applicant>(d => d.UserId)
                .HasConstraintName("FK_Applicant_User");

            entity.HasMany(d => d.Skills).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "ApplicantSkill",
                    r => r.HasOne<Skill>().WithMany()
                        .HasForeignKey("SkillsId")
                        .HasConstraintName("FK_ApplicantSkills_Skills"),
                    l => l.HasOne<Applicant>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_ApplicantSkills_Applicant"),
                    j =>
                    {
                        j.HasKey("UserId", "SkillsId");
                        j.ToTable("ApplicantSkills");
                        j.IndexerProperty<string>("UserId").HasMaxLength(255);
                        j.IndexerProperty<string>("SkillsId").HasMaxLength(255);
                    });
        });

        modelBuilder.Entity<Application>(entity =>
        {
            entity.ToTable("Application");

            entity.HasIndex(e => e.IsArchived, "IX_Application_IsArchived");

            entity.HasIndex(e => e.JobId, "IX_Application_JobId");

            entity.HasIndex(e => e.UserId, "IX_Application_UserId");

            entity.Property(e => e.ApplicationId).HasMaxLength(255);
            entity.Property(e => e.AdditionalInformationId).HasMaxLength(255);
            entity.Property(e => e.ApplicationStatusTypeId)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.JobId)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.UserId)
                .IsRequired()
                .HasMaxLength(255);

            entity.HasOne(d => d.ApplicationStatusType).WithMany(p => p.Applications)
                .HasForeignKey(d => d.ApplicationStatusTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Application_ApplicationStatusType");

            entity.HasOne(d => d.Job).WithMany(p => p.Applications)
                .HasForeignKey(d => d.JobId)
                .HasConstraintName("FK_Application_Job");

            entity.HasOne(d => d.User).WithMany(p => p.Applications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Application_User");
        });

        modelBuilder.Entity<ApplicationStatusType>(entity =>
        {
            entity.ToTable("ApplicationStatusType");

            entity.HasIndex(e => e.ApplicationStatusTypeId, "IX_Application_ApplicationStatusTypeId");

            entity.Property(e => e.ApplicationStatusTypeId).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
        });

        modelBuilder.Entity<Avatar>(entity =>
        {
            entity.ToTable("Avatar");

            entity.Property(e => e.AvatarId).HasMaxLength(255);
            entity.Property(e => e.FileContent).IsRequired();
            entity.Property(e => e.FileName)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.FileType)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.UploadedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<College>(entity =>
        {
            entity.ToTable("College");

            entity.HasIndex(e => e.IsDeleted, "IX_College_IsDeleted");

            entity.HasIndex(e => e.Name, "IX_College_Name");

            entity.HasIndex(e => e.ShortName, "IX_College_ShortName");

            entity.Property(e => e.CollegeId).HasMaxLength(255);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.ShortName)
                .IsRequired()
                .HasMaxLength(255);
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.ToTable("Company");

            entity.HasIndex(e => e.IsArchived, "IX_Company_IsArchived");

            entity.HasIndex(e => e.IsVerified, "IX_Company_IsVerified");

            entity.HasIndex(e => e.MemorandumOfAgreementId, "IX_Company_MOAId");

            entity.Property(e => e.CompanyId).HasMaxLength(255);
            entity.Property(e => e.Address)
                .IsRequired()
                .HasMaxLength(500);
            entity.Property(e => e.CompanyLogoId).HasMaxLength(255);
            entity.Property(e => e.ContactEmail)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.ContactNumber).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.MemorandumOfAgreementId).HasMaxLength(255);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);

            entity.HasOne(d => d.CompanyLogo).WithMany(p => p.Companies)
                .HasForeignKey(d => d.CompanyLogoId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Company_CompanyLogo");

            entity.HasOne(d => d.MemorandumOfAgreement).WithMany(p => p.Companies)
                .HasForeignKey(d => d.MemorandumOfAgreementId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Company_MOA");
        });

        modelBuilder.Entity<CompanyLogo>(entity =>
        {
            entity.ToTable("CompanyLogo");

            entity.Property(e => e.CompanyLogoId).HasMaxLength(255);
            entity.Property(e => e.FileContent).IsRequired();
            entity.Property(e => e.FileName)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.FileType)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.UploadedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("Department");

            entity.HasIndex(e => e.CollegeId, "IX_Department_CollegeId");

            entity.HasIndex(e => e.IsDeleted, "IX_Department_IsDeleted");

            entity.HasIndex(e => e.Name, "IX_Department_Name");

            entity.HasIndex(e => e.ShortName, "IX_Department_ShortName");

            entity.Property(e => e.DepartmentId).HasMaxLength(255);
            entity.Property(e => e.CollegeId)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.ShortName)
                .IsRequired()
                .HasMaxLength(255);

            entity.HasOne(d => d.College).WithMany(p => p.Departments)
                .HasForeignKey(d => d.CollegeId)
                .HasConstraintName("FK_Department_College");
        });

        modelBuilder.Entity<EducationalDetail>(entity =>
        {
            entity.HasKey(e => e.EducationalDetailsId);

            entity.HasIndex(e => e.IdNumber, "IX_EducationalDetails_IdNumber");

            entity.HasIndex(e => e.IsGraduate, "IX_EducationalDetails_IsGraduate");

            entity.Property(e => e.EducationalDetailsId).HasMaxLength(255);
            entity.Property(e => e.DepartmentId)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.IdNumber)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.YearLevelId)
                .IsRequired()
                .HasMaxLength(255);
        });

        modelBuilder.Entity<EmploymentType>(entity =>
        {
            entity.ToTable("EmploymentType");

            entity.Property(e => e.EmploymentTypeId).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
        });

        modelBuilder.Entity<Job>(entity =>
        {
            entity.ToTable("Job");

            entity.HasIndex(e => e.EmploymentTypeId, "IX_Job_EmploymentTypeId");

            entity.HasIndex(e => e.IsArchived, "IX_Job_IsArchived");

            entity.HasIndex(e => e.Location, "IX_Job_Location");

            entity.HasIndex(e => e.PostedById, "IX_Job_PostedById");

            entity.HasIndex(e => e.Salary, "IX_Job_Salary");

            entity.HasIndex(e => e.ScheduleId, "IX_Job_ScheduleId");

            entity.HasIndex(e => e.SetupTypeId, "IX_Job_SetupTypeId");

            entity.HasIndex(e => e.StatusTypeId, "IX_Job_StatusTypeId");

            entity.HasIndex(e => e.Title, "IX_Job_Title");

            entity.HasIndex(e => e.YearLevelId, "IX_Job_YearLevelId");

            entity.Property(e => e.JobId).HasMaxLength(255);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(800);
            entity.Property(e => e.EmploymentTypeId)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.Location)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.PostedById)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.Salary)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.ScheduleId)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.SetupTypeId)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.StatusTypeId)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.UpdatedDate).HasColumnType("datetime");
            entity.Property(e => e.YearLevelId)
                .IsRequired()
                .HasMaxLength(255);

            entity.HasOne(d => d.EmploymentType).WithMany(p => p.Jobs)
                .HasForeignKey(d => d.EmploymentTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Job_EmploymentType");

            entity.HasOne(d => d.PostedBy).WithMany(p => p.Jobs)
                .HasForeignKey(d => d.PostedById)
                .HasConstraintName("FK_Job_Recruiter");

            entity.HasOne(d => d.Schedule).WithMany(p => p.Jobs)
                .HasForeignKey(d => d.ScheduleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Job_Schedule");

            entity.HasOne(d => d.SetupType).WithMany(p => p.Jobs)
                .HasForeignKey(d => d.SetupTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Job_SetupType");

            entity.HasOne(d => d.StatusType).WithMany(p => p.Jobs)
                .HasForeignKey(d => d.StatusTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Job_StatusType");

            entity.HasOne(d => d.YearLevel).WithMany(p => p.Jobs)
                .HasForeignKey(d => d.YearLevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Job_YearLevel");

            entity.HasMany(d => d.Departments).WithMany(p => p.Jobs)
                .UsingEntity<Dictionary<string, object>>(
                    "JobDepartment",
                    r => r.HasOne<Department>().WithMany()
                        .HasForeignKey("DepartmentId")
                        .HasConstraintName("FK_JobDepartment_Department"),
                    l => l.HasOne<Job>().WithMany()
                        .HasForeignKey("JobId")
                        .HasConstraintName("FK_JobDepartment_Job"),
                    j =>
                    {
                        j.HasKey("JobId", "DepartmentId");
                        j.ToTable("JobDepartment");
                        j.IndexerProperty<string>("JobId").HasMaxLength(255);
                        j.IndexerProperty<string>("DepartmentId").HasMaxLength(255);
                    });

            entity.HasMany(d => d.Skills).WithMany(p => p.Jobs)
                .UsingEntity<Dictionary<string, object>>(
                    "JobSkill",
                    r => r.HasOne<Skill>().WithMany()
                        .HasForeignKey("SkillsId")
                        .HasConstraintName("FK_JobSkills_Skills"),
                    l => l.HasOne<Job>().WithMany()
                        .HasForeignKey("JobId")
                        .HasConstraintName("FK_JobSkills_Job"),
                    j =>
                    {
                        j.HasKey("JobId", "SkillsId");
                        j.ToTable("JobSkills");
                        j.IndexerProperty<string>("JobId").HasMaxLength(255);
                        j.IndexerProperty<string>("SkillsId").HasMaxLength(255);
                    });
        });

        modelBuilder.Entity<MemorandumOfAgreement>(entity =>
        {
            entity.ToTable("MemorandumOfAgreement");

            entity.HasIndex(e => e.ValidityEnd, "IX_MOA_ValidityEnd");

            entity.HasIndex(e => e.ValidityStart, "IX_MOA_ValidityStart");

            entity.Property(e => e.MemorandumOfAgreementId).HasMaxLength(255);
            entity.Property(e => e.FileContent).IsRequired();
            entity.Property(e => e.FileName)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.FileType)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.UploadedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ValidityEnd).HasColumnType("datetime");
            entity.Property(e => e.ValidityStart)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Recruiter>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.ToTable("Recruiter");

            entity.HasIndex(e => e.CompanyId, "IX_Recruiter_CompanyId");

            entity.Property(e => e.UserId).HasMaxLength(255);
            entity.Property(e => e.CompanyId).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Company).WithMany(p => p.Recruiters)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Recruiter_Company");

            entity.HasOne(d => d.User).WithOne(p => p.Recruiter)
                .HasForeignKey<Recruiter>(d => d.UserId)
                .HasConstraintName("FK_Recruiter_User");
        });

        modelBuilder.Entity<Resume>(entity =>
        {
            entity.ToTable("Resume");

            entity.HasIndex(e => e.UploadedDate, "IX_Resume_UploadedDate");

            entity.Property(e => e.ResumeId).HasMaxLength(255);
            entity.Property(e => e.FileContent).IsRequired();
            entity.Property(e => e.FileName)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.FileType)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.UploadedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.HasIndex(e => e.Name, "IX_Role_Name");

            entity.Property(e => e.RoleId).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
        });

        modelBuilder.Entity<SavedJob>(entity =>
        {
            entity.ToTable("SavedJob");

            entity.Property(e => e.SavedJobId).HasMaxLength(255);
            entity.Property(e => e.JobId)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.SaveDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId)
                .IsRequired()
                .HasMaxLength(255);

            entity.HasOne(d => d.Job).WithMany(p => p.SavedJobs)
                .HasForeignKey(d => d.JobId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SavedJob_Job");

            entity.HasOne(d => d.User).WithMany(p => p.SavedJobs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SavedJob_User");
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.ToTable("Schedule");

            entity.HasIndex(e => e.Days, "IX_Schedule_Days");

            entity.HasIndex(e => e.Hours, "IX_Schedule_Hours");

            entity.Property(e => e.ScheduleId).HasMaxLength(255);
            entity.Property(e => e.Days)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.Hours).HasMaxLength(255);
        });

        modelBuilder.Entity<SetupType>(entity =>
        {
            entity.ToTable("SetupType");

            entity.Property(e => e.SetupTypeId).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
        });

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.HasKey(e => e.SkillsId);

            entity.HasIndex(e => e.Name, "IX_Skills_Name");

            entity.HasIndex(e => e.Type, "IX_Skills_Type");

            entity.Property(e => e.SkillsId).HasMaxLength(255);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(255);
        });

        modelBuilder.Entity<StatusType>(entity =>
        {
            entity.ToTable("StatusType");

            entity.Property(e => e.StatusTypeId).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.HasIndex(e => e.AvatarId, "IX_User_AvatarId");

            entity.HasIndex(e => e.Email, "IX_User_Email");

            entity.HasIndex(e => e.FirstName, "IX_User_FirstName");

            entity.HasIndex(e => e.IsDeleted, "IX_User_IsDeleted");

            entity.HasIndex(e => e.IsVerified, "IX_User_IsVerified");

            entity.HasIndex(e => e.LastName, "IX_User_LastName");

            entity.HasIndex(e => e.RoleId, "IX_User_RoleId");

            entity.HasIndex(e => e.Token, "IX_User_Token");

            entity.Property(e => e.UserId).HasMaxLength(255);
            entity.Property(e => e.AvatarId).HasMaxLength(255);
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.JoinDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LastLoginDate).HasColumnType("datetime");
            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.MiddleName).HasMaxLength(100);
            entity.Property(e => e.Password)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.RoleId)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.Suffix).HasMaxLength(100);
            entity.Property(e => e.Token).HasMaxLength(255);
            entity.Property(e => e.TokenExpiry).HasColumnType("datetime");

            entity.HasOne(d => d.Avatar).WithMany(p => p.Users)
                .HasForeignKey(d => d.AvatarId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_User_Avatar");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Role");
        });

        modelBuilder.Entity<YearLevel>(entity =>
        {
            entity.ToTable("YearLevel");

            entity.HasIndex(e => e.Name, "IX_YearLevel_Name");

            entity.HasIndex(e => e.Year, "IX_YearLevel_Year");

            entity.Property(e => e.YearLevelId).HasMaxLength(255);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
