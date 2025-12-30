using GateKeeper.Core.Application;
using GateKeeper.Core.Context;
using GateKeeper.Core.Services;
using System.Threading.Tasks;


namespace GateKeeper.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddSingleton<AddressesDbContext>();
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<DomainApplication>();

            var app = builder.Build();
            var dbService = app.Services.GetRequiredService<DatabaseService>();
            await dbService.InitDatabaseAsync();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
