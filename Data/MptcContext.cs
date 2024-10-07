using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MPTC_API.Models.Attendance;
using MPTC_API.Models.BigData;
using MPTC_API.Models.Education;
using MPTC_API.Services.Attendance;

namespace MPTC_API.Data;

public partial class MptcContext : IdentityDbContext<Member>
{
      public MptcContext()
    {
    }
    public MptcContext(DbContextOptions<MptcContext> options)
        : base(options)
    {
    }

     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
                optionsBuilder.UseNpgsql("Host=192.168.0.182;Database=mptc_db;Username=postgres;Password=root;");
                optionsBuilder.UseLazyLoadingProxies();
    }

      protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

         modelBuilder.HasSequence<int>("public.matriculesequence")
                    .StartsAt(1000)
                    .IncrementsBy(1);

        modelBuilder.Entity<Staff>()
            .HasOne(s => s.Venue)
            .WithMany(st => st.Staffs)
            .HasForeignKey(t => t.VenueId);
        modelBuilder.Entity<Staff>()
            .HasOne(s => s.Privilege)
            .WithMany(st => st.Staffs)
            .HasForeignKey(t => t.PrivilegeId);
        modelBuilder.Entity<Member>()
            .HasOne(s => s.Staff)
            .WithOne(st => st.Member)
            .HasForeignKey<Member>(t => t.StaffId);
        modelBuilder.Entity<TimeOff>()
            .HasOne(s => s.Staff)
            .WithMany(st => st.TimeOffs)
            .HasForeignKey(t => t.StaffId);
        modelBuilder.Entity<Sanction>()
            .HasOne(s => s.Staff)
            .WithMany(st => st.Sanctions)
            .HasForeignKey(t => t.StaffId);
        modelBuilder.Entity<Sanction>()
            .HasOne(s => s.Policy)
            .WithMany(st => st.Sanctions)
            .HasForeignKey(t => t.PolicyId);
        modelBuilder.Entity<Log>()
            .HasOne(s => s.Staff)
            .WithMany(st => st.Logs)
            .HasForeignKey(t => t.StaffId);
        modelBuilder.Entity<Schedule>()
            .HasOne(s => s.Staff)
            .WithMany(st => st.Schedules)
            .HasForeignKey(t => t.StaffId);
        modelBuilder.Entity<ProfSubject>()
            .HasOne(s => s.Staff)
            .WithMany(st => st.ProfSubjects)
            .HasForeignKey(t => t.StaffId);
        modelBuilder.Entity<ProfSubject>()
            .HasOne(s => s.Subject)
            .WithMany(st => st.ProfSubjects)
            .HasForeignKey(t => t.SubjectId);
        modelBuilder.Entity<Exam>()
            .HasOne(s => s.Subject)
            .WithMany(st => st.Exams)
            .HasForeignKey(t => t.SubjectId);
        modelBuilder.Entity<Resource>()
            .HasOne(s => s.Staff)
            .WithMany(st => st.Resources)
            .HasForeignKey(t => t.StaffId);
        modelBuilder.Entity<ResourceInteraction>()
            .HasOne(s => s.Resource)
            .WithMany(st => st.ResourceInteractions)
            .HasForeignKey(t => t.ResourceId);
        modelBuilder.Entity<Resource>()
            .HasOne(s => s.ResourceType)
            .WithMany(st => st.Resources)
            .HasForeignKey(t => t.ResourceTypeId);
        modelBuilder.Entity<Resource>()
            .HasOne(s => s.Category)
            .WithMany(st => st.Resources)
            .HasForeignKey(t => t.CategoryId);
        modelBuilder.Entity<SubjectSection>()
            .HasOne(s => s.Subject)
            .WithMany(st => st.SubjectSections)
            .HasForeignKey(t => t.SubjectId);
        modelBuilder.Entity<SubjectSection>()
            .HasOne(s => s.Section)
            .WithMany(st => st.SubjectSections)
            .HasForeignKey(t => t.SectionId);
        modelBuilder.Entity<Exam>()
            .HasOne(s => s.Level)
            .WithMany(st => st.Exams)
            .HasForeignKey(t => t.LevelId);
        modelBuilder.Entity<SubjectSection>()
            .HasOne(s => s.Level)
            .WithMany(st => st.SubjectSections)
            .HasForeignKey(t => t.LevelId);
        modelBuilder.Entity<StudentLevel>()
            .HasOne(s => s.Level)
            .WithMany(st => st.StudentLevels)
            .HasForeignKey(t => t.LevelId);
        modelBuilder.Entity<StudentLevel>()
            .HasOne(s => s.Student)
            .WithMany(st => st.StudentLevels)
            .HasForeignKey(t => t.StudentId);
        modelBuilder.Entity<ResultNoteSection>()
            .HasOne(s => s.SubjectSection)
            .WithMany(st => st.ResultNoteSections)
            .HasForeignKey(t => t.SubjectSectionId);
        modelBuilder.Entity<ResultNote>()
            .HasOne(s => s.Student)
            .WithMany(st => st.ResultNotes)
            .HasForeignKey(t => t.StudentId);
        modelBuilder.Entity<ResultNoteSection>()
            .HasOne(s => s.ResultNote)
            .WithMany(st => st.ResultNoteSections)
            .HasForeignKey(t => t.ResultNoteId);
        modelBuilder.Entity<Staff>()
            .HasOne(s => s.Nationality)
            .WithMany(st => st.Staffs)
            .HasForeignKey(t => t.NationalityId);
        modelBuilder.Entity<ProfLevel>()
            .HasOne(s => s.Period)
            .WithMany(st => st.ProfLevels)
            .HasForeignKey(t => t.PeriodId);
        modelBuilder.Entity<StudentLevel>()
            .HasOne(s => s.Period)
            .WithMany(st => st.StudentLevels)
            .HasForeignKey(t => t.PeriodId);
        modelBuilder.Entity<Exam>()
            .HasOne(s => s.Period)
            .WithMany(st => st.Exams)
            .HasForeignKey(t => t.PeriodId);
        modelBuilder.Entity<Exam>()
            .HasOne(s => s.ProfSubject)
            .WithMany(st => st.Exams)
            .HasForeignKey(t => t.ProfSubjectId);
        modelBuilder.Entity<ResultNote>()
            .HasOne(s => s.ProfSubject)
            .WithMany(st => st.ResultNotes)
            .HasForeignKey(t => t.ProfSubjectId);

    }

   

    public override int SaveChanges()
    {
        foreach (var entry in ChangeTracker.Entries<Staff>())
        {
            if (entry.State == EntityState.Added)
            {
                int nextSequenceValue;

                // Get the next sequence value from the database
                using (var command = Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT nextval('\"matriculesequence\"')";
                    Database.OpenConnection();

                    using (var result = command.ExecuteReader())
                    {
                        if (result.Read())
                        {
                            nextSequenceValue = result.GetInt32(0); // Get the value of the first column
                        }
                        else
                        {
                            nextSequenceValue = 0; // Default value if nothing is returned
                        }
                    }
                }

                // Generate the custom matricule
                entry.Entity.Matricule = StaffService.GenerateMatricule(entry.Entity.FirstName + " " + entry.Entity.StaffName, nextSequenceValue, DateTime.Now, entry.Entity.VenueId);
            }
        }

        return base.SaveChanges();
    }


    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    public virtual DbSet<Staff> Staffs { get; set; }
    public virtual DbSet<Venue> Venues { get; set; }
    public virtual DbSet<Privilege> Privileges { get; set; }
    public virtual DbSet<Member> Members { get; set; }
    public virtual DbSet<TimeOff> TimeOffs { get; set; }
    public virtual DbSet<Sanction> Sanctions { get; set; }
    public virtual DbSet<Policy> Policys { get; set; }
    public virtual DbSet<Log> Logss { get; set; }
    public virtual DbSet<Schedule> Schedules { get; set; }
    public virtual DbSet<ProfSubject> ProfSubjects { get; set; }
    public virtual DbSet<Subject> Subjects { get; set; }
    public virtual DbSet<Exam> Exams { get; set; }
    public virtual DbSet<Resource> Resources { get; set; }
    public virtual DbSet<ResourceInteraction> ResourceInteractions { get; set; }
    public virtual DbSet<ResourceType> ResourceTypes { get; set; }
    public virtual DbSet<Category> Categorys { get; set; }
    public virtual DbSet<SubjectSection> SubjectSections { get; set; }
    public virtual DbSet<Section> Sections { get; set; }
    public virtual DbSet<Level> Levels { get; set; }
    public virtual DbSet<StudentLevel> StudentLevels { get; set; }
    public virtual DbSet<Student> Students { get; set; }
    public virtual DbSet<ResultNoteSection> ResultNoteSections { get; set; }
    public virtual DbSet<ResultNote> ResultNotes { get; set; }
    public virtual DbSet<Nationality> Nationalitys { get; set; }
    public virtual DbSet<ProfLevel> ProfLevels { get; set; }
    public virtual DbSet<Period> Periods { get; set; }
}
