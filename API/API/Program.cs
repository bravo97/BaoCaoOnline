using Application.Interfaces;
using Application.Services;
using Infrastructure.Repositories;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebAPI.Middlewares;

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
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings!.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
            ClockSkew = TimeSpan.FromHours(5)
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
builder.Services.AddScoped<INotificationReponsitory,FileNotificationReponsitory>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<NotificationService>();

// ===========================
// 5. Add Controllers & Swagger
// ===========================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Report API",
        Version = "v1",
        Description = "API for multi-tenant report system with JWT authentication"
    });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter JWT token"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


// Khai báo policy CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // domain frontend
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// ===========================
// 6. Middleware pipeline
// ===========================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    //c =>
    //{
    //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "RESTful API Demo v1");
    //    c.RoutePrefix = string.Empty; // Để Swagger UI hiển thị ở root (http://localhost:<port>/)
    //}
}

app.UseHttpsRedirection();
app.UseRouting();
// **Dùng CORS**
app.UseCors("AllowAngularApp");
// **Thứ tự quan trọng**
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

app.Run();
