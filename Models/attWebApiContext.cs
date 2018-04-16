using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AttendanceWebApi.Models
{
    public partial class attWebApiContext : DbContext
    {
        public virtual DbSet<AcademicTerms> AcademicTerms { get; set; }
        public virtual DbSet<AcademicTermTypes> AcademicTermTypes { get; set; }
        public virtual DbSet<AttendanceLog> AttendanceLog { get; set; }
        public virtual DbSet<AttendanceTypes> AttendanceTypes { get; set; }
        public virtual DbSet<ClassRooms> ClassRooms { get; set; }
        public virtual DbSet<Courses> Courses { get; set; }
        public virtual DbSet<CoursesAssignments> CoursesAssignments { get; set; }
        public virtual DbSet<Enrollments> Enrollments { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<UserRoles> UserRoles { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        public attWebApiContext(DbContextOptions<attWebApiContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AcademicTerms>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Descr)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.TypeId).HasColumnName("TypeID");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.AcademicTerms)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AcademicTerms_AcademicTermTypes");
            });

            modelBuilder.Entity<AcademicTermTypes>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Descr)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<AttendanceLog>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.AcademicTermId).HasColumnName("AcademicTermID");

                entity.Property(e => e.AttendanceTypeId).HasColumnName("AttendanceTypeID");

                entity.Property(e => e.CourseId).HasColumnName("CourseID");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.StudentId).HasColumnName("StudentID");

                entity.HasOne(d => d.AcademicTerm)
                    .WithMany(p => p.AttendanceLog)
                    .HasForeignKey(d => d.AcademicTermId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AttendanceLog_AcademicTerms");

                entity.HasOne(d => d.AttendanceType)
                    .WithMany(p => p.AttendanceLog)
                    .HasForeignKey(d => d.AttendanceTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AttendanceLog_AttendanceTypes");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.AttendanceLog)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AttendanceLog_Courses");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.AttendanceLog)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AttendanceLog_Users");
            });

            modelBuilder.Entity<AttendanceTypes>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Descr)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ClassRooms>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Descr)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Courses>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Descr)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CoursesAssignments>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.AcademicTermId).HasColumnName("AcademicTermID");

                entity.Property(e => e.ClassRoomId).HasColumnName("ClassRoomID");

                entity.Property(e => e.CourseId).HasColumnName("CourseID");

                entity.Property(e => e.Notes)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.ProfessorId).HasColumnName("ProfessorID");

                entity.HasOne(d => d.AcademicTerm)
                    .WithMany(p => p.CoursesAssignments)
                    .HasForeignKey(d => d.AcademicTermId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CoursesAssignments_AcademicTerms");

                entity.HasOne(d => d.ClassRoom)
                    .WithMany(p => p.CoursesAssignments)
                    .HasForeignKey(d => d.ClassRoomId)
                    .HasConstraintName("FK_CoursesAssignments_ClassRooms");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.CoursesAssignments)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CoursesAssignments_Courses");

                entity.HasOne(d => d.Professor)
                    .WithMany(p => p.CoursesAssignments)
                    .HasForeignKey(d => d.ProfessorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CoursesAssignments_Users");
            });

            modelBuilder.Entity<Enrollments>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.AcademicTermId).HasColumnName("AcademicTermID");

                entity.Property(e => e.CourseId).HasColumnName("CourseID");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Notes)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.StudentId).HasColumnName("StudentID");

                entity.HasOne(d => d.AcademicTerm)
                    .WithMany(p => p.Enrollments)
                    .HasForeignKey(d => d.AcademicTermId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Enrollments_AcademicTerms");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Enrollments)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Enrollments_Courses");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.Enrollments)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Enrollments_Users");
            });

            modelBuilder.Entity<Roles>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UserRoles>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRoles_Roles");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRoles_Users");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.FirstName)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });
        }
    }
}
