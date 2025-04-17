
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

            // Add services to the container.
            builder.Services.AddService().AddAuthenticationConfig();

            //builder.Services.AddControllers()
            //    .AddJsonOptions(options =>
            //    {
            //        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            //        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            //        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            //    });
            builder.Services.AddControllers();

            builder.Services.AddDbContext<MilTeaPosDbContext>(options =>
                options.UseMySql(builder.Configuration.GetConnectionString("BlindBoxDbConnection"),
                new MySqlServerVersion(new Version(8, 0, 37))));
            builder.Services.Configure<AuthenticationConfiguration>(builder.Configuration.GetSection("AuthenticationConfiguration"));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerConfig();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                                .Where(a => !a.IsDynamic)
                                .ToArray();

            builder.Services.AddAutoMapper(assemblies);
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
