
using Microsoft.EntityFrameworkCore;
using MilkTeaPosManagement.Api.Extensions;
using MilkTeaPosManagement.Api.Models.Configurations;
using MilkTeaPosManagement.Domain.Models;
using System.Text.Json.Serialization;

namespace MilkTeaPosManagement.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddService().AddAuthenticationConfig();

            builder.Services.AddControllers();

            builder.Services.AddDbContext<MilTeaPosDbContext>(options =>
                options.UseMySql(builder.Configuration.GetConnectionString("BlindBoxDbConnection"),
                new MySqlServerVersion(new Version(8, 0, 37))));
            builder.Services.Configure<AuthenticationConfiguration>(builder.Configuration.GetSection("AuthenticationConfiguration"));
            builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerConfig();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:5173") // Your frontend URL
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials(); // Important for cookies/auth
                });
            });

            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                                .Where(a => !a.IsDynamic)
                                .ToArray();

            builder.Services.AddAutoMapper(assemblies);
            var app = builder.Build();
            app.UseExceptionHandler(appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(new
                    {
                        error = "An unexpected error occurred"
                    });
                });
            });
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MilkTeaPosProject.API v1");
                });
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowFrontend");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
