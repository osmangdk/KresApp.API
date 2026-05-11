using KresApp.Application.Interfaces;
using KresApp.Application.Services;
using KresApp.Infrastructure.Services;
using KresApp.Infrastructure.Settings;
using KresApp.Persistence.Context;
using KresApp.Persistence.Repositories;
using KresApp.API.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddScoped<IChildRepository, ChildRepository>();
builder.Services.AddScoped<IChildAllergyRepository, ChildAllergyRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
builder.Services.AddScoped<IAnnouncementRepository, AnnouncementRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IDailyReportRepository, DailyReportRepository>();
builder.Services.AddScoped<IConversationRepository, ConversationRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IMealMenuRepository, MealMenuRepository>();
builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();
builder.Services.AddScoped<IClassRepository, ClassRepository>();
builder.Services.AddScoped<ISchoolBellRepository, SchoolBellRepository>();
builder.Services.AddScoped<ILearningOutcomeRepository, LearningOutcomeRepository>();
builder.Services.AddScoped<IMedicationRepository, MedicationRepository>();
builder.Services.AddScoped<IGalleryRepository, GalleryRepository>();
builder.Services.AddScoped<IVaccinationRepository, VaccinationRepository>();
builder.Services.AddScoped<IChildHealthRepository, ChildHealthRepository>();
builder.Services.AddScoped<IUserAccessRequestRepository, UserAccessRequestRepository>();
builder.Services.AddScoped<IEnrollmentRequestRepository, EnrollmentRequestRepository>();
builder.Services.AddScoped<ISystemSettingRepository, SystemSettingRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ILdapService, LdapService>();
var minioSettings = builder.Configuration.GetSection("Minio").Get<MinioSettings>();
if (minioSettings?.UseLocal == true)
{
    builder.Services.AddScoped<IFileStorageService, LocalStorageService>();
}
else
{
    builder.Services.AddScoped<IFileStorageService, MinioStorageService>();
}

builder.Services.Configure<LdapSettings>(builder.Configuration.GetSection("Ldap"));
builder.Services.Configure<MinioSettings>(builder.Configuration.GetSection("Minio"));
builder.Services.Configure<SmsSettings>(builder.Configuration.GetSection("Sms"));

builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ISmsService, SmsService>();

builder.Services.AddScoped<ChildService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ProfileService>();
builder.Services.AddScoped<AttendanceService>();
builder.Services.AddScoped<AnnouncementService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<DailyReportService>();
builder.Services.AddScoped<MessageService>();
builder.Services.AddScoped<MealMenuService>();
builder.Services.AddScoped<ScheduleService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<ClassService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<SchoolBellService>();
builder.Services.AddScoped<LearningOutcomeService>();
builder.Services.AddScoped<MedicationService>();
builder.Services.AddScoped<GalleryService>();
builder.Services.AddScoped<VaccinationService>();
builder.Services.AddScoped<ChildHealthService>();
builder.Services.AddScoped<UserAccessRequestService>();
builder.Services.AddScoped<EnrollmentService>();
builder.Services.AddScoped<SystemSettingService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(opt =>
{
    var jwtKey = builder.Configuration["Jwt:Key"];
    var issuer = builder.Configuration["Jwt:Issuer"];
    var audience = builder.Configuration["Jwt:Audience"];

    if (string.IsNullOrWhiteSpace(jwtKey))
    {
        throw new InvalidOperationException("Configuration value 'Jwt:Key' is not set or is empty. Please set Jwt:Key in configuration.");
    }
    if (Encoding.UTF8.GetByteCount(jwtKey) < 32)
    {
        throw new InvalidOperationException("Configuration value 'Jwt:Key' must be at least 32 bytes for HmacSha256.");
    }
    if (string.IsNullOrWhiteSpace(issuer))
    {
        throw new InvalidOperationException("Configuration value 'Jwt:Issuer' is not set or is empty.");
    }
    if (string.IsNullOrWhiteSpace(audience))
    {
        throw new InvalidOperationException("Configuration value 'Jwt:Audience' is not set or is empty.");
    }

    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();

builder.Services.AddCors(o =>
    o.AddPolicy("all", p => p
        .SetIsOriginAllowed(origin => true) // Yerel geliştirmede tüm originlere izin ver (port farkı için)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "KresApp API", Version = "v1" });

    var bearerSchemeId = JwtBearerDefaults.AuthenticationScheme;
    opt.AddSecurityDefinition(bearerSchemeId, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = bearerSchemeId,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization: Bearer {token}"
    });

    opt.OperationFilter<JwtAuthorizeOperationFilter>();
});

var app = builder.Build();

// ── Startup: EnrollmentRequests tablosunu yoksa oluştur ──────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.ExecuteSqlRaw(@"
        CREATE TABLE IF NOT EXISTS ""EnrollmentRequests"" (
            ""Id""                  uuid        NOT NULL DEFAULT uuid_generate_v4(),

            -- Veli bilgileri
            ""ParentName""          text        NOT NULL,
            ""ParentEmail""         text        NOT NULL,
            ""ParentPhone""         text        NOT NULL,
            ""ParentTcKimlikNo""    text,
            ""ParentPassword""      text        NOT NULL,
            ""ParentJob""           text,
            ""ParentWorkAddress""   text,
            ""ParentHomeAddress""   text,

            -- Baba bilgileri
            ""FatherName""          text,
            ""FatherPhone""         text,
            ""FatherJob""           text,
            ""FatherWorkAddress""   text,
            ""FatherTcKimlikNo""    text,

            -- Çocuk bilgileri
            ""ChildFullName""       text        NOT NULL,
            ""ChildBirthDate""      date,
            ""ChildGender""         text,
            ""ChildBloodType""      text,
            ""ChildAllergies""      text,

            -- Sağlık izleme formu (JSON)
            ""InfectiousDiseases""  text,
            ""ChronicDiseases""     text,
            ""OtherHealthNotes""    text,

            -- Acil durum kişileri
            ""Emergency1Name""      text,
            ""Emergency1Relation""  text,
            ""Emergency1Phone""     text,
            ""Emergency1Address""   text,
            ""Emergency2Name""      text,
            ""Emergency2Relation""  text,
            ""Emergency2Phone""     text,
            ""Emergency2Address""   text,

            -- İzinler
            ""MediaConsent""        boolean,

            -- MinIO dosya URL'leri
            ""FolderPath""          text,
            ""ChildPhotoUrl""       text,
            ""MotherPhotoUrl""      text,
            ""FatherPhotoUrl""      text,
            ""IdCardCopyUrl""       text,
            ""HealthReportUrl""     text,
            ""VaccinationCardUrl""  text,
            ""CommitmentDocUrl""    text,
            ""MediaConsentDocUrl""  text,
            ""InstitutionIdDocUrl"" text,

            -- Durum
            ""Notes""               text,
            ""Status""              integer     NOT NULL DEFAULT 0,
            ""AdminNote""           text,
            ""CreatedAt""           timestamptz NOT NULL DEFAULT now(),
            ""ReviewedAt""          timestamptz,
            ""ReviewedByUserId""    uuid,
            CONSTRAINT ""PK_EnrollmentRequests"" PRIMARY KEY (""Id"")
        );

        -- Mevcut tabloyu güncelle (varsa eksik sütunları ekle)
        ALTER TABLE ""EnrollmentRequests""
            ADD COLUMN IF NOT EXISTS ""ParentJob""           text,
            ADD COLUMN IF NOT EXISTS ""ParentWorkAddress""   text,
            ADD COLUMN IF NOT EXISTS ""ParentHomeAddress""   text,
            ADD COLUMN IF NOT EXISTS ""FatherName""          text,
            ADD COLUMN IF NOT EXISTS ""FatherPhone""         text,
            ADD COLUMN IF NOT EXISTS ""FatherJob""           text,
            ADD COLUMN IF NOT EXISTS ""FatherWorkAddress""   text,
            ADD COLUMN IF NOT EXISTS ""FatherTcKimlikNo""    text,
            ADD COLUMN IF NOT EXISTS ""InfectiousDiseases""  text,
            ADD COLUMN IF NOT EXISTS ""ChronicDiseases""     text,
            ADD COLUMN IF NOT EXISTS ""OtherHealthNotes""    text,
            ADD COLUMN IF NOT EXISTS ""Emergency1Name""      text,
            ADD COLUMN IF NOT EXISTS ""Emergency1Relation""  text,
            ADD COLUMN IF NOT EXISTS ""Emergency1Phone""     text,
            ADD COLUMN IF NOT EXISTS ""Emergency1Address""   text,
            ADD COLUMN IF NOT EXISTS ""Emergency2Name""      text,
            ADD COLUMN IF NOT EXISTS ""Emergency2Relation""  text,
            ADD COLUMN IF NOT EXISTS ""Emergency2Phone""     text,
            ADD COLUMN IF NOT EXISTS ""Emergency2Address""   text,
            ADD COLUMN IF NOT EXISTS ""MediaConsent""        boolean,
            ADD COLUMN IF NOT EXISTS ""FolderPath""          text,
            ADD COLUMN IF NOT EXISTS ""ChildPhotoUrl""       text,
            ADD COLUMN IF NOT EXISTS ""MotherPhotoUrl""      text,
            ADD COLUMN IF NOT EXISTS ""FatherPhotoUrl""      text,
            ADD COLUMN IF NOT EXISTS ""IdCardCopyUrl""       text,
            ADD COLUMN IF NOT EXISTS ""HealthReportUrl""     text,
            ADD COLUMN IF NOT EXISTS ""VaccinationCardUrl""  text,
            ADD COLUMN IF NOT EXISTS ""CommitmentDocUrl""    text,
            ADD COLUMN IF NOT EXISTS ""MediaConsentDocUrl""  text,
            ADD COLUMN IF NOT EXISTS ""InstitutionIdDocUrl"" text,
            ADD COLUMN IF NOT EXISTS ""Score""               integer,
            ADD COLUMN IF NOT EXISTS ""ScoringNotes""        text,
            ADD COLUMN IF NOT EXISTS ""CreatedUserId""       uuid,
            ADD COLUMN IF NOT EXISTS ""ChildTcKimlikNo""     text,
            ADD COLUMN IF NOT EXISTS ""ParentSicilNo""      text,
            ADD COLUMN IF NOT EXISTS ""ParentUnit""         text,
            ADD COLUMN IF NOT EXISTS ""ParentTitle""        text,
            ADD COLUMN IF NOT EXISTS ""ParentServiceYears"" integer,
            ADD COLUMN IF NOT EXISTS ""SpouseIsAlive""      boolean DEFAULT true,
            ADD COLUMN IF NOT EXISTS ""SpouseIsWorking""    boolean,
            ADD COLUMN IF NOT EXISTS ""SpouseWorkplace""    text,
            ADD COLUMN IF NOT EXISTS ""SpouseWorkplaceHasDaycare"" boolean;

        -- SystemSettings tablosu oluşturma
        CREATE TABLE IF NOT EXISTS ""SystemSettings"" (
            ""Id""                      uuid        PRIMARY KEY DEFAULT uuid_generate_v4(),
            ""IsPreEnrollmentActive""   boolean     NOT NULL DEFAULT true,
            ""PreEnrollmentStartDate""  timestamptz,
            ""PreEnrollmentEndDate""    timestamptz,
            ""UpdatedAt""               timestamptz NOT NULL DEFAULT now()
        );

        -- İlk ayar satırını ekle (eğer yoksa)
        INSERT INTO ""SystemSettings"" (""Id"", ""IsPreEnrollmentActive"", ""UpdatedAt"")
        SELECT uuid_generate_v4(), true, now()
        WHERE NOT EXISTS (SELECT 1 FROM ""SystemSettings"");

        -- Children tablosuna yeni alanlar ekle
        ALTER TABLE ""Children""
            ADD COLUMN IF NOT EXISTS ""TcKimlikNo""      text;

        -- Users tablosuna yeni alanlar ekle
        ALTER TABLE ""Users""
            ADD COLUMN IF NOT EXISTS ""TcKimlikNo""      text,
            ADD COLUMN IF NOT EXISTS ""AccountStatus""   integer NOT NULL DEFAULT 2;
    ");
}
// ─────────────────────────────────────────────────────────────────────────────

app.UseStaticFiles();
app.UseCors("all");
app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();