using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace WebApplication_MT4North.Models
{
    public partial class CUSERSKAJO59SOURCEREPOSWEBAPPLICATION_MT4NORTHWEBAPPLICATION_MT4NORTHMODELSMEDTECHINNOVATIONMODEL_V2MDFContext : DbContext
    {
        public CUSERSKAJO59SOURCEREPOSWEBAPPLICATION_MT4NORTHWEBAPPLICATION_MT4NORTHMODELSMEDTECHINNOVATIONMODEL_V2MDFContext()
        {
        }

        public CUSERSKAJO59SOURCEREPOSWEBAPPLICATION_MT4NORTHWEBAPPLICATION_MT4NORTHMODELSMEDTECHINNOVATIONMODEL_V2MDFContext(DbContextOptions<CUSERSKAJO59SOURCEREPOSWEBAPPLICATION_MT4NORTHWEBAPPLICATION_MT4NORTHMODELSMEDTECHINNOVATIONMODEL_V2MDFContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Activity> Activities { get; set; }
        public virtual DbSet<InnovationModel> InnovationModels { get; set; }
        public virtual DbSet<Note> Notes { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<RegisteredUser> RegisteredUsers { get; set; }
        public virtual DbSet<RegisteredUserProject> RegisteredUserProjects { get; set; }
        public virtual DbSet<Theme> Themes { get; set; }

        /*
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\kajo59\\Source\\Repos\\WebApplication_MT4North\\WebApplication_MT4North\\Models\\medtechinnovationmodel_v2.mdf;Integrated Security=True;Trusted_Connection=False;");
            }
        }
        */

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Finnish_Swedish_CI_AS");

            modelBuilder.Entity<Activity>(entity =>
            {
                entity.ToTable("activity");

                entity.Property(e => e.ActivityId).HasColumnName("activity_id");

                entity.Property(e => e.DeadlineDate)
                    .HasColumnType("datetime")
                    .HasColumnName("deadline_date");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.FinishDate)
                    .HasColumnType("datetime")
                    .HasColumnName("finish_date");

                entity.Property(e => e.IsBaseActivity)
                    .IsRequired()
                    .HasColumnName("is_base_activity")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsExcluded).HasColumnName("is_excluded");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.Property(e => e.Phase)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("phase");

                entity.Property(e => e.Resources).HasColumnName("resources");

                entity.Property(e => e.StartDate)
                    .HasColumnType("datetime")
                    .HasColumnName("start_date");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("status")
                    .HasDefaultValueSql("('Ej påbörjad')");

                entity.Property(e => e.ThemeId).HasColumnName("theme_id");

                entity.HasOne(d => d.Theme)
                    .WithMany(p => p.Activities)
                    .HasForeignKey(d => d.ThemeId)
                    .HasConstraintName("FK_activity_theme");
            });

            modelBuilder.Entity<InnovationModel>(entity =>
            {
                entity.ToTable("innovation_model");

                entity.Property(e => e.InnovationModelId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("innovation_model_id");

                entity.HasOne(d => d.InnovationModelNavigation)
                    .WithOne(p => p.InnovationModel)
                    .HasForeignKey<InnovationModel>(d => d.InnovationModelId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_innovation_model_project");
            });

            modelBuilder.Entity<Note>(entity =>
            {
                entity.ToTable("note");

                entity.Property(e => e.NoteId).HasColumnName("note_id");

                entity.Property(e => e.ActivityId).HasColumnName("activity_id");

                entity.Property(e => e.Text)
                    .IsRequired()
                    .HasColumnName("text");

                entity.Property(e => e.TimeStamp)
                    .HasColumnType("datetime")
                    .HasColumnName("time_stamp");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.Notes)
                    .HasForeignKey(d => d.ActivityId)
                    .HasConstraintName("FK_note_activity");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Notes)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_note_registered_user");
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.ToTable("project");

                entity.Property(e => e.ProjectId).HasColumnName("project_id");

                entity.Property(e => e.Description)
                    .HasMaxLength(50)
                    .HasColumnName("description");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<RegisteredUser>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK_registered_user_1");

                entity.ToTable("registered_user");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.EmailAdress)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("email_adress");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("first_name");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("last_name");

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .HasColumnName("password");

                entity.Property(e => e.Sex)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("sex");

                entity.Property(e => e.UserName)
                    .HasMaxLength(50)
                    .HasColumnName("user_name");
            });

            modelBuilder.Entity<RegisteredUserProject>(entity =>
            {
                entity.ToTable("registered_user_project");

                entity.Property(e => e.RegisteredUserProjectId).HasColumnName("registered_user_project_id");

                entity.Property(e => e.ProjectId).HasColumnName("project_id");

                entity.Property(e => e.RegisteredUserId).HasColumnName("registered_user_id");

                entity.Property(e => e.Rights)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("rights")
                    .HasDefaultValueSql("('Read and write')");

                entity.Property(e => e.Role)
                    .HasMaxLength(50)
                    .HasColumnName("role");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.RegisteredUserProjects)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_project_member_project");

                entity.HasOne(d => d.RegisteredUser)
                    .WithMany(p => p.RegisteredUserProjects)
                    .HasForeignKey(d => d.RegisteredUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_project_member_registered_user");
            });

            modelBuilder.Entity<Theme>(entity =>
            {
                entity.ToTable("theme");

                entity.Property(e => e.ThemeId).HasColumnName("theme_id");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.InnovationModelId).HasColumnName("innovation_model__id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.HasOne(d => d.InnovationModel)
                    .WithMany(p => p.Themes)
                    .HasForeignKey(d => d.InnovationModelId)
                    .HasConstraintName("FK_theme_innovation_model");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
