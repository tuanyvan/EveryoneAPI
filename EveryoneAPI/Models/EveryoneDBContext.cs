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
        public virtual DbSet<Pronoun> Pronouns { get; set; } = null!;
        public virtual DbSet<SexualOrientation> SexualOrientations { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=tcp:everyone-sql-db.database.windows.net,1433;Initial Catalog=EveryoneDB;User Id=everyonedb@everyone-sql-db;Password=kMj5cRqLEXKDnHb");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasIndex(e => e.EmployerId, "IX_Departments_EmployerId");

                entity.Property(e => e.DepartmentId).ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.HasOne(d => d.Employer)
                    .WithMany(p => p.Departments)
                    .HasForeignKey(d => d.EmployerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Departments_Employers");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasIndex(e => e.DepartmentId, "IX_Employees_DepartmentId");

                entity.HasIndex(e => e.EmployerId, "IX_Employees_EmployerId");

                entity.HasIndex(e => e.Ethnicity, "IX_Employees_Ethnicity");

                entity.HasIndex(e => e.GenderIdentity, "IX_Employees_GenderIdentity");

                entity.HasIndex(e => e.PodId, "IX_Employees_PodId");

                entity.HasIndex(e => e.Pronoun, "IX_Employees_Pronoun");

                entity.HasIndex(e => e.SexualOrientation, "IX_Employees_SexualOrientation");

                entity.Property(e => e.Name).HasMaxLength(50);

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

                entity.HasOne(d => d.PronounNavigation)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.Pronoun)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Employees_Pronouns");

                entity.HasOne(d => d.SexualOrientationNavigation)
                    .WithMany(p => p.Employees)
                    .HasForeignKey(d => d.SexualOrientation)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Employees_SexualOrientations");
            });

            modelBuilder.Entity<Employer>(entity =>
            {
                entity.HasIndex(e => e.EmployerId, "IX_Employers");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Uuid)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("UUID");
            });

            modelBuilder.Entity<Ethnicity>(entity =>
            {
                entity.Property(e => e.EthnicityId).ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<GenderIdentity>(entity =>
            {
                entity.HasKey(e => e.GenderId);

                entity.Property(e => e.GenderId).ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Pod>(entity =>
            {
                entity.HasIndex(e => e.DepartmentId, "IX_Pods_DepartmentId");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Pods)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Pods_Departments");
            });

            modelBuilder.Entity<Pronoun>(entity =>
            {
                entity.Property(e => e.PronounId).ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(25);
            });

            modelBuilder.Entity<SexualOrientation>(entity =>
            {
                entity.HasKey(e => e.OrientationId);

                entity.Property(e => e.OrientationId).ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
