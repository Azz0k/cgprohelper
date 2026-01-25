using GateKeeper.API.Models;
using GateKeeper.Core.Application;
using GateKeeper.Core.Context;
using GateKeeper.Core.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("GateKeeper.Tests")]
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
            string secretCode = config.GetSection("JWTSecretCode").Get<string>() ?? Guid.NewGuid().ToString();
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddSpaStaticFiles(conf =>
            {
                conf.RootPath = "wwwroot";
            });
            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddScoped<DatabaseService>();
            builder.Services.AddDbContext<AddressesDbContext>(options => options.UseSqlite(connectionString));
            builder.Services.AddScoped<AllowedDomainsApplication>();
            builder.Services.AddScoped<ForeingEmailsApplication>();
            builder.Services.AddScoped<LocalMonitoredEmailsApplication>();
            builder.Services.AddScoped<UserApplication>();
            builder.Services.AddScoped<UserAuthenticationApplication>();
            builder.Services.Configure<ApiSettings>(opt=>opt.JwtSecretCode = secretCode);
            builder.Services.AddScoped<ApiSettings>();
            builder.Services.AddAuthorization();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretCode)),
                        ValidateIssuerSigningKey = true,
                    };
                });
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
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "wwwroot";
            });
            app.Run();
        }
    }
}
