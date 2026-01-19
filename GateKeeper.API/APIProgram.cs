using GateKeeper.Core.Application;
using GateKeeper.Core.Context;
using GateKeeper.Core.Services;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;


namespace GateKeeper.API
{
    public class APIProgram
    {
        public static async Task Main(string[] args)
        {
            var settignsBuilder = new ConfigurationBuilder().AddJsonFile($"appsettings.json");
            IConfiguration config = settignsBuilder.Build();
            var allowedOrigins = config.GetSection("AllowedOrigins").Get<string>();
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddScoped<DatabaseService>();
            builder.Services.AddDbContext<AddressesDbContext>();
            builder.Services.AddScoped<AllowedDomainsApplication>();
            builder.Services.AddScoped<ForeingEmailsApplication>();
            builder.Services.AddScoped<LocalMonitoredEmailsApplication>();
            if (allowedOrigins != null)
            {
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("FrontEnd", policy =>
                    {
                        policy.WithOrigins(allowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    });
                });
            }
            var app = builder.Build();
            using var scope = app.Services.CreateScope();
            var dbService = scope.ServiceProvider.GetRequiredService<DatabaseService>();
            await dbService.InitDatabaseAsync();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }
            if (allowedOrigins != null)
            {
                app.UseCors("FrontEnd");
            }
                app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
