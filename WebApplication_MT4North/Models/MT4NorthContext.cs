using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

//#nullable disable

namespace WebApplication_MT4North.Models
{
    public partial class MT4NorthContext : IdentityDbContext
    {
        public MT4NorthContext()
        {
        }

        public MT4NorthContext(DbContextOptions<MT4NorthContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Activity> Activities { get; set; }
        public virtual DbSet<BaseActivityInfo> BaseActivityInfos { get; set; }
        public virtual DbSet<CustomActivityInfo> CustomActivityInfos { get; set; }
        public virtual DbSet<InnovationModel> InnovationModels { get; set; }
        public virtual DbSet<Note> Notes { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<RegisteredUser> RegisteredUsers { get; set; }
        public virtual DbSet<RegisteredUserProject> RegisteredUserProjects { get; set; }
        public virtual DbSet<Theme> Themes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                //FIXME: TODO:
                optionsBuilder.UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\goma02\\Source\\Repos\\WebApplication_MT4North\\WebApplication_MT4North\\Models\\medtechinnovationmodel_v3.mdf;Integrated Security=True;Trusted_Connection=False;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasAnnotation("Relational:Collation", "Finnish_Swedish_CI_AS");

            modelBuilder.Entity<Activity>(entity =>
            {
                entity.ToTable("activity");

                entity.Property(e => e.ActivityId).HasColumnName("activity_id");

                entity.Property(e => e.BaseInfoId).HasColumnName("base_info_id");

                entity.Property(e => e.DeadlineDate)
                    .HasColumnType("datetime")
                    .HasColumnName("deadline_date");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.FinishDate)
                    .HasColumnType("datetime")
                    .HasColumnName("finish_date");

                entity.Property(e => e.IsExcluded).HasColumnName("is_excluded");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.Property(e => e.Phase)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("phase");

                entity.Property(e => e.ProjectId).HasColumnName("project_id");

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

                entity.HasOne(d => d.BaseInfo)
                    .WithMany(p => p.Activities)
                    .HasForeignKey(d => d.BaseInfoId)
                    .HasConstraintName("FK_activity_base_activity_info");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Activities)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_activity_project");

                entity.HasOne(d => d.Theme)
                    .WithMany(p => p.Activities)
                    .HasForeignKey(d => d.ThemeId)
                    .HasConstraintName("FK_activity_theme");
            });

            modelBuilder.Entity<BaseActivityInfo>(entity =>
            {
                entity.HasKey(e => e.BaseInfoId);

                entity.ToTable("base_activity_info");

                entity.Property(e => e.BaseInfoId).HasColumnName("base_info_id");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.Property(e => e.Phase)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("phase");

                entity.Property(e => e.ThemeId).HasColumnName("theme_id");

                entity.HasOne(d => d.Theme)
                    .WithMany(p => p.BaseActivityInfos)
                    .HasForeignKey(d => d.ThemeId)
                    .HasConstraintName("FK_base_activity_info_theme");
            });

            modelBuilder.Entity<CustomActivityInfo>(entity =>
            {
                entity.HasKey(e => e.CustomInfoId)
                    .HasName("PK_custom_activity");

                entity.ToTable("custom_activity_info");

                entity.Property(e => e.CustomInfoId).HasColumnName("custom_info_id");

                entity.Property(e => e.ActivityId).HasColumnName("activity_id");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.Property(e => e.Phase)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("phase");

                entity.Property(e => e.ThemeId).HasColumnName("theme_id");

                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.CustomActivityInfos)
                    .HasForeignKey(d => d.ActivityId)
                    .HasConstraintName("FK_custom_activity_info_activity");

                entity.HasOne(d => d.Theme)
                    .WithMany(p => p.CustomActivityInfos)
                    .HasForeignKey(d => d.ThemeId)
                    .HasConstraintName("FK_custom_activity_info_theme");
            });

            modelBuilder.Entity<InnovationModel>(entity =>
            {
                entity.ToTable("innovation_model");

                entity.Property(e => e.InnovationModelId).HasColumnName("innovation_model_id");

                entity.Property(e => e.ProjectId).HasColumnName("project_id");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.InnovationModels)
                    .HasForeignKey(d => d.ProjectId)
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
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_note_activity");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Notes)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
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
                    .HasConstraintName("FK_project_member_project");

                entity.HasOne(d => d.RegisteredUser)
                    .WithMany(p => p.RegisteredUserProjects)
                    .HasForeignKey(d => d.RegisteredUserId)
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


            // Identity Start
            modelBuilder
                .HasAnnotation("ProductVersion", "3.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
            {
                b.Property<string>("Id")
                    .HasColumnType("nvarchar(450)");

                b.Property<string>("ConcurrencyStamp")
                    .IsConcurrencyToken()
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("Name")
                    .HasColumnType("nvarchar(256)")
                    .HasMaxLength(256);

                b.Property<string>("NormalizedName")
                    .HasColumnType("nvarchar(256)")
                    .HasMaxLength(256);

                b.HasKey("Id");

                b.HasIndex("NormalizedName")
                    .IsUnique()
                    .HasName("RoleNameIndex")
                    .HasFilter("[NormalizedName] IS NOT NULL");

                b.ToTable("AspNetRoles");
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int")
                    .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                b.Property<string>("ClaimType")
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("ClaimValue")
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("RoleId")
                    .IsRequired()
                    .HasColumnType("nvarchar(450)");

                b.HasKey("Id");

                b.HasIndex("RoleId");

                b.ToTable("AspNetRoleClaims");
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
            {
                b.Property<string>("Id")
                    .HasColumnType("nvarchar(450)");

                b.Property<int>("AccessFailedCount")
                    .HasColumnType("int");

                b.Property<string>("ConcurrencyStamp")
                    .IsConcurrencyToken()
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("Email")
                    .HasColumnType("nvarchar(256)")
                    .HasMaxLength(256);

                b.Property<bool>("EmailConfirmed")
                    .HasColumnType("bit");

                b.Property<bool>("LockoutEnabled")
                    .HasColumnType("bit");

                b.Property<DateTimeOffset?>("LockoutEnd")
                    .HasColumnType("datetimeoffset");

                b.Property<string>("NormalizedEmail")
                    .HasColumnType("nvarchar(256)")
                    .HasMaxLength(256);

                b.Property<string>("NormalizedUserName")
                    .HasColumnType("nvarchar(256)")
                    .HasMaxLength(256);

                b.Property<string>("PasswordHash")
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("PhoneNumber")
                    .HasColumnType("nvarchar(max)");

                b.Property<bool>("PhoneNumberConfirmed")
                    .HasColumnType("bit");

                b.Property<string>("SecurityStamp")
                    .HasColumnType("nvarchar(max)");

                b.Property<bool>("TwoFactorEnabled")
                    .HasColumnType("bit");

                b.Property<string>("UserName")
                    .HasColumnType("nvarchar(256)")
                    .HasMaxLength(256);

                b.HasKey("Id");

                b.HasIndex("NormalizedEmail")
                    .HasName("EmailIndex");

                b.HasIndex("NormalizedUserName")
                    .IsUnique()
                    .HasName("UserNameIndex")
                    .HasFilter("[NormalizedUserName] IS NOT NULL");

                b.ToTable("AspNetUsers");
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int")
                    .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                b.Property<string>("ClaimType")
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("ClaimValue")
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("UserId")
                    .IsRequired()
                    .HasColumnType("nvarchar(450)");

                b.HasKey("Id");

                b.HasIndex("UserId");

                b.ToTable("AspNetUserClaims");
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
            {
                b.Property<string>("LoginProvider")
                    .HasColumnType("nvarchar(128)")
                    .HasMaxLength(128);

                b.Property<string>("ProviderKey")
                    .HasColumnType("nvarchar(128)")
                    .HasMaxLength(128);

                b.Property<string>("ProviderDisplayName")
                    .HasColumnType("nvarchar(max)");

                b.Property<string>("UserId")
                    .IsRequired()
                    .HasColumnType("nvarchar(450)");

                b.HasKey("LoginProvider", "ProviderKey");

                b.HasIndex("UserId");

                b.ToTable("AspNetUserLogins");
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
            {
                b.Property<string>("UserId")
                    .HasColumnType("nvarchar(450)");

                b.Property<string>("RoleId")
                    .HasColumnType("nvarchar(450)");

                b.HasKey("UserId", "RoleId");

                b.HasIndex("RoleId");

                b.ToTable("AspNetUserRoles");
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
            {
                b.Property<string>("UserId")
                    .HasColumnType("nvarchar(450)");

                b.Property<string>("LoginProvider")
                    .HasColumnType("nvarchar(128)")
                    .HasMaxLength(128);

                b.Property<string>("Name")
                    .HasColumnType("nvarchar(128)")
                    .HasMaxLength(128);

                b.Property<string>("Value")
                    .HasColumnType("nvarchar(max)");

                b.HasKey("UserId", "LoginProvider", "Name");

                b.ToTable("AspNetUserTokens");
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
            {
                b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                    .WithMany()
                    .HasForeignKey("RoleId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
            {
                b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
            {
                b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
            {
                b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                    .WithMany()
                    .HasForeignKey("RoleId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

                b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
            {
                b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
            });
            // Identity End

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
