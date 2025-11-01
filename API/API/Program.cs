using Application.Interfaces;
using Application.Services;
using Infrastructure.Repositories;
using Infrastructure.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ===========================
// 1. Cấu hình JWT từ appsettings.json
// ===========================
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Map interface IJwtSettings -> implementation JwtSettings
builder.Services.AddSingleton<IJwtSettings>(sp =>
    sp.GetRequiredService<IOptions<JwtSettings>>().Value);

// ===========================
// 2. JWT Authentication
// ===========================
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        // Lấy instance JwtSettings đã được inject
        var jwtSettings = builder.Services.BuildServiceProvider().GetRequiredService<IJwtSettings>();

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
        };
    });

// ===========================
// 3. Đăng ký Application services
// ===========================
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// ===========================
// 4. Các repository & services khác
// ===========================
builder.Services.AddSingleton<IUserRepository, FileUserRepository>();
builder.Services.AddSingleton<ICustomerRepository, FileCustomerRepository>();
builder.Services.AddScoped<IAccountRepository, FileAccountRepository>();
builder.Services.AddScoped<IReportRepository, OptimizedSqlReportRepository>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CustomerService>();

// ===========================
// 5. Add Controllers & Swagger
// ===========================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ===========================
// 6. Middleware pipeline
// ===========================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// **Thứ tự quan trọng**
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
