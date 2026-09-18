using EquipmentRental.Api.Data;
using EquipmentRental.Api.Interfaces;
using EquipmentRental.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace EquipmentRental.Api
{
    public class Program
    {
        // Instantiated by the test host only, never by hand.
        protected Program() { }

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddRouting(options => options.LowercaseUrls = true);
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // A SQLite file next to the API, created with the fleet on first
            // start, so a clone runs as is.
            builder.Services.AddDbContext<RentalContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("Rental") ?? "Data Source=rental.db"));

            builder.Services.AddScoped<IEquipmentService, EquipmentService>();
            builder.Services.AddScoped<IRentalService, RentalService>();

            // Only the front end may call from a browser: the configured
            // origins, plus any local one while developing.
            var origins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
            var isDevelopment = builder.Environment.IsDevelopment();
            builder.Services.AddCors(options =>
                options.AddDefaultPolicy(policy => policy
                    .SetIsOriginAllowed(origin => origins.Contains(origin) || (isDevelopment && IsLocal(origin)))
                    .AllowAnyHeader()
                    .AllowAnyMethod()));

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<RentalContext>().Database.EnsureCreated();
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors();
            app.MapControllers();

            app.Run();
        }

        private static bool IsLocal(string origin)
        {
            return Uri.TryCreate(origin, UriKind.Absolute, out var uri) && uri.IsLoopback;
        }
    }
}
