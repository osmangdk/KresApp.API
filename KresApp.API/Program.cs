using KresApp.Application.Interfaces;
using KresApp.Application.Services;
using KresApp.Infrastructure.Services;
using KresApp.Persistence.Context;
using KresApp.Persistence.Repositories;
using KresApp.API.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
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
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtService, JwtService>();

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
    o.AddPolicy("all", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

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

app.UseCors("all");
app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();