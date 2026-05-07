using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using KresApp.Domain.Entities;

namespace KresApp.Persistence.Context;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Child> Children => Set<Child>();
    public DbSet<Class> Classes => Set<Class>();
    public DbSet<Attendance> Attendances => Set<Attendance>();
    public DbSet<DailyReport> DailyReports => Set<DailyReport>();
    public DbSet<Announcement> Announcements => Set<Announcement>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<MealMenu> MealMenus => Set<MealMenu>();
    public DbSet<Schedule> Schedules => Set<Schedule>();
    public DbSet<ChildAllergy> ChildAllergies => Set<ChildAllergy>();
    public DbSet<SchoolBellRequest> SchoolBellRequests => Set<SchoolBellRequest>();
    public DbSet<LearningOutcome> LearningOutcomes => Set<LearningOutcome>();
    public DbSet<Medication> Medications => Set<Medication>();
    public DbSet<MedicationLog> MedicationLogs => Set<MedicationLog>();
    public DbSet<GalleryItem> GalleryItems => Set<GalleryItem>();
    public DbSet<Vaccination> Vaccinations => Set<Vaccination>();
    public DbSet<ChildHealthRecord> ChildHealthRecords => Set<ChildHealthRecord>();
    public DbSet<UserAccessRequest> UserAccessRequests => Set<UserAccessRequest>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .Property(x => x.Id)
            .ValueGeneratedNever(); // .NET üretir

        modelBuilder.Entity<Child>(eb =>
        {
            eb.ToTable("Children");
            eb.HasKey(x => x.Id);
            eb.Property(x => x.Id)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("uuid_generate_v4()");
            eb.Property(x => x.Name)
                .HasColumnName("Name")
                .IsRequired();
            eb.Property(x => x.BirthDate)
                .HasColumnName("BirthDate");
            eb.Property(x => x.ParentId)
                .HasColumnName("ParentId")
                .IsRequired();
            eb.Property(x => x.ClassId)
                .HasColumnName("ClassId")
                .IsRequired();
            eb.Property(x => x.CreatedAt)
                .HasColumnName("CreatedAt")
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("now()")
                .Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
            eb.Property(x => x.BloodType)
                .HasColumnName("BloodType")
                .HasDefaultValue("Tamamlanmadı");
            eb.Property(x => x.MedicalNotes)
                .HasColumnName("MedicalNotes");
            eb.Property(x => x.Gender)
                .HasColumnName("Gender");
            eb.Property(x => x.Weight)
                .HasColumnName("Weight");
            eb.Property(x => x.Height)
                .HasColumnName("Height");
            eb.Property(x => x.ParentName)
                .HasColumnName("ParentName")
                .HasDefaultValue("");
            eb.Property(x => x.ParentPhone)
                .HasColumnName("ParentPhone")
                .HasDefaultValue("");
            eb.Property(x => x.SecondaryPhone)
                .HasColumnName("SecondaryPhone");
            eb.Property(x => x.EnrollmentDate)
                .HasColumnName("EnrollmentDate")
                .HasDefaultValueSql("CURRENT_DATE");
        });

        // Yeni Entity'lerin Konfigürasyonu
        modelBuilder.Entity<Attendance>(eb => {
            eb.ToTable("Attendances");
            eb.HasKey(x => x.Id);
            eb.Property(x => x.Id).HasDefaultValueSql("uuid_generate_v4()");
            eb.Property(x => x.CreatedAt).HasDefaultValueSql("now()").ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<DailyReport>(eb => {
            eb.ToTable("DailyReports");
            eb.HasKey(x => x.Id);
            eb.Property(x => x.Id).HasDefaultValueSql("uuid_generate_v4()");
            eb.Property(x => x.CreatedAt).HasDefaultValueSql("now()").ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Announcement>(eb => {
            eb.ToTable("Announcements");
            eb.HasKey(x => x.Id);
            eb.Property(x => x.Id).HasDefaultValueSql("uuid_generate_v4()");
            eb.Property(x => x.CreatedAt).HasDefaultValueSql("now()").ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Payment>(eb => {
            eb.ToTable("Payments");
            eb.HasKey(x => x.Id);
            eb.Property(x => x.Id).HasDefaultValueSql("uuid_generate_v4()");
            eb.Property(x => x.CreatedAt).HasDefaultValueSql("now()").ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Conversation>(eb => {
            eb.ToTable("Conversations");
            eb.HasKey(x => x.Id);
            eb.Property(x => x.Id).HasDefaultValueSql("uuid_generate_v4()");
            eb.Property(x => x.CreatedAt).HasDefaultValueSql("now()").ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Message>(eb => {
            eb.ToTable("Messages");
            eb.HasKey(x => x.Id);
            eb.Property(x => x.Id).HasDefaultValueSql("uuid_generate_v4()");
            eb.Property(x => x.CreatedAt).HasDefaultValueSql("now()").ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<MealMenu>(eb => {
            eb.ToTable("MealMenus");
            eb.HasKey(x => x.Id);
            eb.Property(x => x.Id).HasDefaultValueSql("uuid_generate_v4()");
            eb.Property(x => x.CreatedAt).HasDefaultValueSql("now()").ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Schedule>(eb => {
            eb.ToTable("Schedules");
            eb.HasKey(x => x.Id);
            eb.Property(x => x.Id).HasDefaultValueSql("uuid_generate_v4()");
            eb.Property(x => x.CreatedAt).HasDefaultValueSql("now()").ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Class>(eb => {
            eb.ToTable("Classes");
            eb.HasKey(x => x.Id);
            eb.Property(x => x.Id).HasDefaultValueSql("uuid_generate_v4()");
            eb.Property(x => x.Name).IsRequired();
            eb.Property(x => x.TeacherId).IsRequired();
        });

        modelBuilder.Entity<ChildAllergy>(eb => {
            eb.ToTable("ChildAllergies");
            eb.HasKey(x => x.Id);
            eb.Property(x => x.Id)
                .HasColumnName("Id")
                .HasDefaultValueSql("uuid_generate_v4()");
            eb.Property(x => x.ChildId)
                .HasColumnName("ChildId")
                .IsRequired();
            eb.Property(x => x.AllergyName)
                .HasColumnName("AllergyName")
                .IsRequired();
            eb.Property(x => x.Severity)
                .HasColumnName("Severity");
            eb.Property(x => x.Notes)
                .HasColumnName("Notes");
            eb.Property(x => x.CreatedAt)
                .HasColumnName("CreatedAt")
                .HasDefaultValueSql("now()")
                .ValueGeneratedOnAdd();
            eb.HasOne(x => x.Child)
                .WithMany(c => c.Allergies)
                .HasForeignKey(x => x.ChildId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<SchoolBellRequest>(eb => {
            eb.ToTable("SchoolBellRequests");
            eb.HasKey(x => x.Id);
            eb.Property(x => x.Id).HasDefaultValueSql("uuid_generate_v4()");
            eb.Property(x => x.CreatedAt).HasDefaultValueSql("now()").ValueGeneratedOnAdd();
            eb.HasOne(x => x.Child).WithMany().HasForeignKey(x => x.ChildId);
            eb.HasOne(x => x.Parent).WithMany().HasForeignKey(x => x.ParentId);
        });

        modelBuilder.Entity<LearningOutcome>(eb => {
            eb.ToTable("LearningOutcomes");
            eb.HasKey(x => x.Id);
            eb.Property(x => x.Id).HasDefaultValueSql("uuid_generate_v4()");
            eb.Property(x => x.Theme).IsRequired();
            eb.Property(x => x.Outcomes)
              .HasColumnType("jsonb")
              .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>());
            eb.Property(x => x.CreatedAt).HasDefaultValueSql("now()").ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Medication>(eb => {
            eb.ToTable("Medications");
            eb.HasKey(x => x.Id);
            eb.Property(x => x.Id).HasDefaultValueSql("uuid_generate_v4()");
            eb.Property(x => x.Times)
              .HasColumnType("jsonb")
              .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>());
            eb.Property(x => x.CreatedAt).HasDefaultValueSql("now()").ValueGeneratedOnAdd();
            eb.HasOne(x => x.Child).WithMany().HasForeignKey(x => x.ChildId);
        });

        modelBuilder.Entity<MedicationLog>(eb => {
            eb.ToTable("MedicationLogs");
            eb.HasKey(x => x.Id);
            eb.Property(x => x.Id).HasDefaultValueSql("uuid_generate_v4()");
            eb.HasOne(x => x.Medication).WithMany().HasForeignKey(x => x.MedicationId);
            eb.HasOne(x => x.GivenBy).WithMany().HasForeignKey(x => x.GivenByUserId);
        });

        modelBuilder.Entity<GalleryItem>(eb => {
            eb.ToTable("GalleryItems");
            eb.HasKey(x => x.Id);
            eb.Property(x => x.Id).HasDefaultValueSql("uuid_generate_v4()");
            eb.Property(x => x.CreatedAt).HasDefaultValueSql("now()").ValueGeneratedOnAdd();
            eb.HasOne(x => x.CreatedBy).WithMany().HasForeignKey(x => x.CreatedByUserId);
            eb.HasOne(x => x.Class).WithMany().HasForeignKey(x => x.ClassId);
            eb.HasOne(x => x.Child).WithMany().HasForeignKey(x => x.ChildId);
        });

        modelBuilder.Entity<Vaccination>(eb => {
            eb.ToTable("Vaccinations");
            eb.HasKey(x => x.Id);
            eb.Property(x => x.Id).HasDefaultValueSql("uuid_generate_v4()");
            eb.Property(x => x.CreatedAt).HasDefaultValueSql("now()").ValueGeneratedOnAdd();
            eb.HasOne(x => x.Child).WithMany(c => c.Vaccinations).HasForeignKey(x => x.ChildId);
        });

        modelBuilder.Entity<ChildHealthRecord>(eb => {
            eb.ToTable("ChildHealthRecords");
            eb.HasKey(x => x.Id);
            eb.Property(x => x.Id).HasDefaultValueSql("uuid_generate_v4()");
            eb.Property(x => x.CreatedAt).HasDefaultValueSql("now()").ValueGeneratedOnAdd();
            eb.HasOne(x => x.Child).WithMany(c => c.HealthRecords).HasForeignKey(x => x.ChildId);
        });

        modelBuilder.Entity<UserAccessRequest>(eb => {
            eb.ToTable("UserAccessRequests");
            eb.HasKey(x => x.Id);
            eb.Property(x => x.Id).HasDefaultValueSql("uuid_generate_v4()");
            eb.Property(x => x.Email).IsRequired();
            eb.Property(x => x.Name).IsRequired();
            eb.Property(x => x.CreatedAt).HasDefaultValueSql("now()").ValueGeneratedOnAdd();
        });
    }
}