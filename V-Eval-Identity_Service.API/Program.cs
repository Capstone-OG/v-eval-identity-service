using System.Text;
using Application;
using Infrastructure;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Seeds;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using V_Eval_Identity_Service.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// 1. Đăng ký Application & Infrastructure layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// 2. Cấu hình Controllers & JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// 3. Cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 4. Cấu hình Authentication (JWT Bearer) & Authorization
var jwtSecret = builder.Configuration["Jwt:SecretKey"] ?? "V-Eval_Secure_JWT_Key_2026_Identity_Service_Capstone_Secret!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "V-Eval-IdentityService";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "V-Eval-Clients";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// 5. Cấu hình gRPC (nếu sử dụng cho giao tiếp microservices)
builder.Services.AddGrpc();

// 6. Cấu hình Swagger với JWT Bearer Authorize button
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "V-Eval Identity & User Profile Service API",
        Version = "v1",
        Description = "Microservice cung cấp chức năng Xác thực, Phân quyền & Quản lý Hồ sơ người dùng (IAM & Profile)."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Nhập JWT Bearer Token theo format: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// 7. Global Exception Handler Middleware
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

// 8. Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "V-Eval Identity Service API v1");
        c.RoutePrefix = string.Empty; // Mở Swagger ngay tại root URL
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// 9. Database Auto-Migration & Seed Data
try
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    // Tự động áp dụng các migration cho PostgreSQL (tạo schema, bảng)
    await dbContext.Database.MigrateAsync();
    
    // Seed các Roles hệ thống
    await DbInitializer.SeedAsync(dbContext);
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Lỗi khi khởi tạo và seed dữ liệu cơ sở dữ liệu.");
}

app.Run();
