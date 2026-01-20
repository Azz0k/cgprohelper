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
            string defaultConnectionString = "Data Source=AddressDatabase.sqlite";
            var settignsBuilder = new ConfigurationBuilder().AddJsonFile($"appsettings.json");
            IConfiguration config = settignsBuilder.Build();
            string? allowedOrigins = config.GetSection("AllowedOrigins").Get<string>();
            string? connectionString = config.GetSection("ConnectionString").Get<string>()??defaultConnectionString;
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddScoped<DatabaseService>();
            builder.Services.AddDbContext<AddressesDbContext>(options => options.UseSqlite(connectionString));
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
            app.UseDefaultFiles();
            app.UseStaticFiles();
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
