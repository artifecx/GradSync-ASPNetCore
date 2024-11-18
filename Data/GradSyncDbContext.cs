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

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Applicant> Applicants { get; set; }

    public virtual DbSet<ApplicantSkill> ApplicantSkills { get; set; }

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

    public virtual DbSet<JobApplicantMatch> JobApplicantMatches { get; set; }

    public virtual DbSet<JobProgram> JobPrograms { get; set; }

    public virtual DbSet<JobSkill> JobSkills { get; set; }

    public virtual DbSet<MemorandumOfAgreement> MemorandumOfAgreements { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<MessageParticipant> MessageParticipants { get; set; }

    public virtual DbSet<MessageThread> MessageThreads { get; set; }

    public virtual DbSet<Program> Programs { get; set; }

    public virtual DbSet<Recruiter> Recruiters { get; set; }

    public virtual DbSet<Resume> Resumes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SavedJob> SavedJobs { get; set; }

    public virtual DbSet<SetupType> SetupTypes { get; set; }

    public virtual DbSet<Skill> Skills { get; set; }

    public virtual DbSet<StatusType> StatusTypes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<YearLevel> YearLevels { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
            entity.Property(e => e.EducationalDetailId).HasMaxLength(255);
            entity.Property(e => e.ResumeId).HasMaxLength(255);

            entity.HasOne(d => d.EducationalDetail).WithMany(p => p.Applicants)
                .HasForeignKey(d => d.EducationalDetailId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Applicant_EducationalDetail");

            entity.HasOne(d => d.Resume).WithMany(p => p.Applicants)
                .HasForeignKey(d => d.ResumeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Applicant_Resume");

            entity.HasOne(d => d.User).WithOne(p => p.Applicant)
                .HasForeignKey<Applicant>(d => d.UserId)
                .HasConstraintName("FK_Applicant_User");
        });

        modelBuilder.Entity<ApplicantSkill>(entity =>
        {
            entity.ToTable("ApplicantSkill");

            entity.HasIndex(e => e.Type, "IX_ApplicantSkill_Type");

            entity.Property(e => e.ApplicantSkillId).HasMaxLength(255);
            entity.Property(e => e.SkillId)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.UserId)
                .IsRequired()
                .HasMaxLength(255);

            entity.HasOne(d => d.Skill).WithMany(p => p.ApplicantSkills)
                .HasForeignKey(d => d.SkillId)
                .HasConstraintName("FK_ApplicantSkill_Skill");

            entity.HasOne(d => d.User).WithMany(p => p.ApplicantSkills)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_ApplicantSkill_Applicant");
        });

        modelBuilder.Entity<Application>(entity =>
        {
            entity.ToTable("Application");

            entity.HasIndex(e => e.IsArchived, "IX_Application_IsArchived");

            entity.HasIndex(e => e.JobId, "IX_Application_JobId");

            entity.HasIndex(e => e.UserId, "IX_Application_UserId");

            entity.Property(e => e.ApplicationId).HasMaxLength(255);
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
            entity.ToTable("EducationalDetail");

            entity.HasIndex(e => e.IdNumber, "IX_EducationalDetail_IdNumber");

            entity.HasIndex(e => e.IsGraduate, "IX_EducationalDetail_IsGraduate");

            entity.Property(e => e.EducationalDetailId).HasMaxLength(255);
            entity.Property(e => e.CollegeId)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.DepartmentId)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.IdNumber)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.ProgramId)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.YearLevelId)
                .IsRequired()
                .HasMaxLength(255);

            entity.HasOne(d => d.College).WithMany(p => p.EducationalDetails)
                .HasForeignKey(d => d.CollegeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EducationalDetail_College");

            entity.HasOne(d => d.Department).WithMany(p => p.EducationalDetails)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EducationalDetail_Department");

            entity.HasOne(d => d.Program).WithMany(p => p.EducationalDetails)
                .HasForeignKey(d => d.ProgramId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EducationalDetail_Program");

            entity.HasOne(d => d.YearLevel).WithMany(p => p.EducationalDetails)
                .HasForeignKey(d => d.YearLevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EducationalDetail_YearLevel");
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

            entity.HasIndex(e => e.CompanyId, "IX_Job_CompanyId");

            entity.HasIndex(e => e.EmploymentTypeId, "IX_Job_EmploymentTypeId");

            entity.HasIndex(e => e.IsArchived, "IX_Job_IsArchived");

            entity.HasIndex(e => e.Location, "IX_Job_Location");

            entity.HasIndex(e => e.PostedById, "IX_Job_PostedById");

            entity.HasIndex(e => e.Salary, "IX_Job_Salary");

            entity.HasIndex(e => e.Schedule, "IX_Job_Schedule");

            entity.HasIndex(e => e.SetupTypeId, "IX_Job_SetupTypeId");

            entity.HasIndex(e => e.StatusTypeId, "IX_Job_StatusTypeId");

            entity.HasIndex(e => e.Title, "IX_Job_Title");

            entity.HasIndex(e => e.YearLevelId, "IX_Job_YearLevelId");

            entity.Property(e => e.JobId).HasMaxLength(255);
            entity.Property(e => e.CompanyId)
                .IsRequired()
                .HasMaxLength(255);
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
            entity.Property(e => e.Schedule)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.SetupTypeId)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.SkillWeights).HasColumnType("decimal(2, 1)");
            entity.Property(e => e.StatusTypeId)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.UpdatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.YearLevelId)
                .IsRequired()
                .HasMaxLength(255);

            entity.HasOne(d => d.Company).WithMany(p => p.Jobs)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_Job_Company");

            entity.HasOne(d => d.EmploymentType).WithMany(p => p.Jobs)
                .HasForeignKey(d => d.EmploymentTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Job_EmploymentType");

            entity.HasOne(d => d.PostedBy).WithMany(p => p.Jobs)
                .HasForeignKey(d => d.PostedById)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Job_Recruiter");

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
        });

        modelBuilder.Entity<JobApplicantMatch>(entity =>
        {
            entity.ToTable("JobApplicantMatch");

            entity.HasIndex(e => e.MatchPercentage, "IX_JobApplicantMatch_MatchPercentage");

            entity.Property(e => e.JobApplicantMatchId).HasMaxLength(255);
            entity.Property(e => e.JobId)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.UserId)
                .IsRequired()
                .HasMaxLength(255);

            entity.HasOne(d => d.Job).WithMany(p => p.JobApplicantMatches)
                .HasForeignKey(d => d.JobId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_JobApplicantMatch_Job");

            entity.HasOne(d => d.User).WithMany(p => p.JobApplicantMatches)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_JobApplicantMatch_Applicant");
        });

        modelBuilder.Entity<JobProgram>(entity =>
        {
            entity.ToTable("JobProgram");

            entity.Property(e => e.JobProgramId).HasMaxLength(255);
            entity.Property(e => e.JobId)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.ProgramId)
                .IsRequired()
                .HasMaxLength(255);

            entity.HasOne(d => d.Job).WithMany(p => p.JobPrograms)
                .HasForeignKey(d => d.JobId)
                .HasConstraintName("FK_JobProgram_Job");

            entity.HasOne(d => d.Program).WithMany(p => p.JobPrograms)
                .HasForeignKey(d => d.ProgramId)
                .HasConstraintName("FK_JobProgram_Program");
        });

        modelBuilder.Entity<JobSkill>(entity =>
        {
            entity.ToTable("JobSkill");

            entity.HasIndex(e => e.Type, "IX_JobSkill_Type");

            entity.Property(e => e.JobSkillId).HasMaxLength(255);
            entity.Property(e => e.JobId)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.SkillId)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(255);

            entity.HasOne(d => d.Job).WithMany(p => p.JobSkills)
                .HasForeignKey(d => d.JobId)
                .HasConstraintName("FK_JobSkill_Job");

            entity.HasOne(d => d.Skill).WithMany(p => p.JobSkills)
                .HasForeignKey(d => d.SkillId)
                .HasConstraintName("FK_JobSkill_Skill");
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

        modelBuilder.Entity<Message>(entity =>
        {
            entity.ToTable("Message");

            entity.HasIndex(e => e.MessageThreadId, "IX_Message_MessageThreadId");

            entity.HasIndex(e => e.UserId, "IX_Message_SenderId");

            entity.Property(e => e.MessageId).HasMaxLength(255);
            entity.Property(e => e.Content)
                .IsRequired()
                .HasMaxLength(500);
            entity.Property(e => e.MessageThreadId)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId)
                .IsRequired()
                .HasMaxLength(255);

            entity.HasOne(d => d.MessageThread).WithMany(p => p.Messages)
                .HasForeignKey(d => d.MessageThreadId)
                .HasConstraintName("FK_Message_MessageThread");

            entity.HasOne(d => d.User).WithMany(p => p.Messages)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Message_User");
        });

        modelBuilder.Entity<MessageParticipant>(entity =>
        {
            entity.ToTable("MessageParticipant");

            entity.HasIndex(e => e.MessageThreadId, "IX_MessageParticipant_MessageThreadId");

            entity.HasIndex(e => e.UserId, "IX_MessageParticipant_UserId");

            entity.Property(e => e.MessageParticipantId).HasMaxLength(255);
            entity.Property(e => e.MessageThreadId)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.UserId)
                .IsRequired()
                .HasMaxLength(255);

            entity.HasOne(d => d.MessageThread).WithMany(p => p.MessageParticipants)
                .HasForeignKey(d => d.MessageThreadId)
                .HasConstraintName("FK_MessageParticipant_MessageThread");

            entity.HasOne(d => d.User).WithMany(p => p.MessageParticipants)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_MessageParticipant_User");
        });

        modelBuilder.Entity<MessageThread>(entity =>
        {
            entity.ToTable("MessageThread");

            entity.Property(e => e.MessageThreadId).HasMaxLength(255);
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(255);
        });

        modelBuilder.Entity<Program>(entity =>
        {
            entity.ToTable("Program");

            entity.HasIndex(e => e.DepartmentId, "IX_Program_CollegeId");

            entity.HasIndex(e => e.IsDeleted, "IX_Program_IsDeleted");

            entity.HasIndex(e => e.Name, "IX_Program_Name");

            entity.HasIndex(e => e.ShortName, "IX_Program_ShortName");

            entity.Property(e => e.ProgramId).HasMaxLength(255);
            entity.Property(e => e.DepartmentId)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.ShortName)
                .IsRequired()
                .HasMaxLength(255);

            entity.HasOne(d => d.Department).WithMany(p => p.Programs)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK_Program_Department");
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
            entity.ToTable("Skill");

            entity.HasIndex(e => e.Name, "IX_Skill_Name");

            entity.HasIndex(e => e.Type, "IX_Skill_Type");

            entity.Property(e => e.SkillId).HasMaxLength(255);
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
            entity.Property(e => e.FromSignUp).HasDefaultValue(true);
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
