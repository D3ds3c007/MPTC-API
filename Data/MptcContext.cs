﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MPTC_API.Models.Attendance;
using MPTC_API.Models.BigData;
using MPTC_API.Models.Education;

namespace MPTC_API.Data;

public partial class MptcContext : DbContext
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
                optionsBuilder.UseNpgsql("Host=localhost;Database=mptc_db;Username=postgres;Password=root;");
                optionsBuilder.UseLazyLoadingProxies();
    }

      protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);

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
        modelBuilder.Entity<Exam>()
            .HasOne(s => s.Staff)
            .WithMany(st => st.Exams)
            .HasForeignKey(t => t.StaffId);
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
        modelBuilder.Entity<TempResultSection>()
            .HasOne(s => s.SubjectSection)
            .WithMany(st => st.TempResultSections)
            .HasForeignKey(t => t.SubjectSectionId);
        modelBuilder.Entity<TempResult>()
            .HasOne(s => s.Staff)
            .WithMany(st => st.TempResults)
            .HasForeignKey(t => t.StaffId);
        modelBuilder.Entity<TempResultSection>()
            .HasOne(s => s.TempResult)
            .WithMany(st => st.TempResultSections)
            .HasForeignKey(t => t.TempResultId);
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
        modelBuilder.Entity<TempResult>()
            .HasOne(s => s.Student)
            .WithMany(st => st.TempResults)
            .HasForeignKey(t => t.StudentId);
        modelBuilder.Entity<ResultNoteSection>()
            .HasOne(s => s.SubjectSection)
            .WithMany(st => st.ResultNoteSections)
            .HasForeignKey(t => t.SubjectSectionId);
        modelBuilder.Entity<ResultNote>()
            .HasOne(s => s.Staff)
            .WithMany(st => st.ResultNotes)
            .HasForeignKey(t => t.StaffId);
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
    public virtual DbSet<TempResultSection> TempResultSections { get; set; }
    public virtual DbSet<TempResult> TempResults { get; set; }
    public virtual DbSet<Section> Sections { get; set; }
    public virtual DbSet<Level> Levels { get; set; }
    public virtual DbSet<StudentLevel> StudentLevels { get; set; }
    public virtual DbSet<Student> Students { get; set; }
    public virtual DbSet<ResultNoteSection> ResultNoteSections { get; set; }
    public virtual DbSet<ResultNote> ResultNotes { get; set; }
    public virtual DbSet<Nationality> Nationalitys { get; set; }
}
