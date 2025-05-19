using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TaskManagementSystem.Api.Services;
using TaskManagementSystem.Core.Entities;
using TaskManagementSystem.Core.Interfaces;
using TaskManagementSystem.Core.Services;
using TaskManagementSystem.Infrastructure.Data;
using TaskManagementSystem.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Task Management API", Version = "v1" });

    // Configure Swagger to use JWT Authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// Add JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
                builder.Configuration["Jwt:Secret"])),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("TaskManagementDb"));

// Register repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();

// Register services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<IPasswordHashService, PasswordHashService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        var passwordHashService = services.GetRequiredService<IPasswordHashService>();
        
        // Ensure database is created
        dbContext.Database.EnsureCreated();
        
        // Check if we need to manually seed data
        if (!dbContext.Users.Any())
        {
            app.Logger.LogInformation("Seeding database...");
            
            // Add seed data
            var admin = new User
            {
                Id = 1,
                Username = "admin",
                Password = passwordHashService.HashPassword("admin123"),
                Email = "admin@example.com",
                Role = "Admin"
            };
            
            var user = new User
            {
                Id = 2,
                Username = "user",
                Password = passwordHashService.HashPassword("user123"),
                Email = "user@example.com",
                Role = "User"
            };
            
            dbContext.Users.Add(admin);
            dbContext.Users.Add(user);
            dbContext.SaveChanges();
            
            // Add sample tasks
            dbContext.Tasks.AddRange(
                new TaskItem
                {
                    Id = 1,
                    Title = "Complete project proposal",
                    Description = "Create a detailed project proposal document",
                    DueDate = DateTime.Now.AddDays(7),
                    Status = TaskItemStatus.Pending,
                    AssignedUserId = 1
                },
                new TaskItem
                {
                    Id = 2,
                    Title = "Review code changes",
                    Description = "Review pull request #42",
                    DueDate = DateTime.Now.AddDays(2),
                    Status = TaskItemStatus.InProgress,
                    AssignedUserId = 2
                },
                new TaskItem
                {
                    Id = 3,
                    Title = "Fix login bug",
                    Description = "Fix the authentication issue reported by QA",
                    DueDate = DateTime.Now.AddDays(1),
                    Status = TaskItemStatus.Pending,
                    AssignedUserId = 2
                }
            );
            
            dbContext.SaveChanges();
            app.Logger.LogInformation("Database seeded successfully.");
        }
        else
        {
            app.Logger.LogInformation("Database already contains data, skipping seed.");
        }
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();