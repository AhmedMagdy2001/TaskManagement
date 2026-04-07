using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TaskManagement.Application.Services;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Infrastructure.Auth;
using TaskManagement.Infrastructure.Background;
using TaskManagement.Infrastructure.Persistence;
using TaskManagement.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 1. Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Redis Caching
builder.Services.AddStackExchangeRedisCache(options => {
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

// 3. Dependency Injection (DI)
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TaskService>();


// 4. background worker
builder.Services.AddSingleton<PostTaskCreationWorker>();
// This line allows TaskService to find the worker via the interface
builder.Services.AddSingleton<ITaskBackgroundProcessor>(sp => sp.GetRequiredService<PostTaskCreationWorker>());
builder.Services.AddHostedService(sp => sp.GetRequiredService<PostTaskCreationWorker>());

// 5. JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

// 6. Swagger with JWT Support
builder.Services.AddSwaggerGen(c => {
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();