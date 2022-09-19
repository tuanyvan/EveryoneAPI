using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EveryoneAPI.Models
{
    public partial class EveryoneDBContext : DbContext
    {
        public EveryoneDBContext()
        {
        }

        public EveryoneDBContext(DbContextOptions<EveryoneDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Department> Departments { get; set; } = null!;
        public virtual DbSet<Employee> Employees { get; set; } = null!;
        public virtual DbSet<Employer> Employers { get; set; } = null!;
        public virtual DbSet<Ethnicity> Ethnicities { get; set; } = null!;
        public virtual DbSet<GenderIdentity> GenderIdentities { get; set; } = null!;
        public virtual DbSet<Pod> Pods { get; set; } = null!;
        public virtual DbSet<SexualOrientation> SexualOrientations { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("ConnectionStrings--EveryoneDb"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>(entity =>
            {
                entity.Property(e => e.DepartmentId).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsFixedLength();

                entity.HasOne(d => d.Employer)
                    .WithMany(p => p.Departments)
                    .HasForeignKey(d => d.EmployerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Departments_Employers");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.Property(e => e.EmployeeId).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsFixedLength();

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK_Employees_Departments");

                entity.HasOne(d => d.Employer)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.EmployerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Employees_Employers");

                entity.HasOne(d => d.EthnicityNavigation)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.Ethnicity)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Employees_Ethnicities");

                entity.HasOne(d => d.GenderIdentityNavigation)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.GenderIdentity)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Employees_GenderIdentities");

                entity.HasOne(d => d.Pod)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.PodId)
                    .HasConstraintName("FK_Employees_Pods");

                entity.HasOne(d => d.SexualOrientationNavigation)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.SexualOrientation)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Employees_SexualOrientations");
            });

            modelBuilder.Entity<Employer>(entity =>
            {
                entity.HasIndex(e => e.EmployerId, "IX_Employers");

                entity.Property(e => e.EmployerId).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Ethnicity>(entity =>
            {
                entity.Property(e => e.EthnicityId).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsFixedLength();
            });

            modelBuilder.Entity<GenderIdentity>(entity =>
            {
                entity.HasKey(e => e.GenderId);

                entity.Property(e => e.GenderId).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Pod>(entity =>
            {
                entity.Property(e => e.PodId).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsFixedLength();

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Pods)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Pods_Departments");
            });

            modelBuilder.Entity<SexualOrientation>(entity =>
            {
                entity.HasKey(e => e.OrientationId);

                entity.Property(e => e.OrientationId).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsFixedLength();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
