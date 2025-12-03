using BusinessLogicLayer.IServices;
using BusinessLogicLayer.Mappers;
using BusinessLogicLayer.Services;
using BussinessLogicLayer.IServices;
using BussinessLogicLayer.Services;
using DataAccessLayer.Context;
using DataAccessLayer.Entities;
using DataAccessLayer.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

namespace RecyclingSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Configure DbContext with SQL Server
            string? conString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<RecyclingDbContext>(options =>
                options.UseSqlServer(conString, sql => sql.EnableRetryOnFailure()));

           
            // Configure Identity with ApplicationUser and Roles
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<RecyclingDbContext>()
            .AddDefaultTokenProviders();

            // Register Material Service
            builder.Services.AddScoped<MaterialService>();
                        builder.Services.AddScoped<IOrderService, OrderService>();

            // Register Unit of Work
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register Business Services
            builder.Services.AddScoped<IFactoryService, FactoryService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            // Register AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

            // Configure Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });

            builder.Services.AddAuthorization(options =>
            {
                // Role-based policies
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("CollectorAccess", policy => policy.RequireRole("Collector", "Admin"));
                options.AddPolicy("UserAccess", policy => policy.RequireRole("User", "Collector", "Admin"));
            });

            builder.Services.AddControllers();

            // Add Swagger/OpenAPI Documentation
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                // Enable Swagger UI
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Recycling System API v1");
                    options.RoutePrefix = "swagger";
                });

                // Enable Scalar UI (uses Swagger document)
                app.MapScalarApiReference(options =>
                {
                    options.WithTitle("Recycling System API")
                           .WithTheme(ScalarTheme.Moon)
                           .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
                });
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();

            // Redirect root URL to Swagger documentation
            app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

            app.MapControllers();

            app.Run();
        }
    }
}
