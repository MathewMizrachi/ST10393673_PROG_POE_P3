using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging; // Add this to include logging
using ST10393673_PROG6212_POE.Models;
using ST10393673_PROG6212_POE.Services;

namespace ST10393673_PROG6212_POE
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Register IClaimService and ClaimService
            builder.Services.AddScoped<IClaimService, ClaimService>();

            // Configure Entity Framework Core and ApplicationDbContext for SQL Server
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Configure ASP.NET Core Identity with custom password options
            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>() // Store Identity in ApplicationDbContext
                .AddDefaultTokenProviders(); // Add token providers for password reset, etc.

            // Register TableService for Azure Table Storage using the updated connection string
            builder.Services.AddSingleton<TableService>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var logger = provider.GetRequiredService<ILogger<TableService>>(); // Get the logger from DI container
                var connectionString = configuration.GetConnectionString("AzureStorageConnection"); // Fetch connection string from appsettings.json under ConnectionStrings
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new ArgumentNullException("AzureStorageConnection", "The Azure Storage connection string is not configured.");
                }
                return new TableService(connectionString, logger); // Pass logger to the TableService constructor
            });

            // Register BlobStorageService
            builder.Services.AddSingleton<BlobStorageService>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var logger = provider.GetRequiredService<ILogger<BlobStorageService>>(); // Get the logger from DI container
                var connectionString = configuration.GetValue<string>("AzureBlobStorage:ConnectionString"); // Fetch the blob connection string from appsettings.json
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new ArgumentNullException("AzureBlobStorageConnection", "The Blob Storage connection string is not configured.");
                }
                return new BlobStorageService(connectionString, logger); // Pass both connection string and logger to BlobStorageService
            });

            // Register IClaimService and ClaimService (ensure this line exists)
            builder.Services.AddSingleton<IClaimService, ClaimService>();

            // Build the application
            var app = builder.Build();

            // Configure middleware pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); // Show detailed errors in development
            }
            else
            {
                app.UseExceptionHandler("/Home/Error"); // Global error handling page
                app.UseHsts(); // Use HTTP Strict Transport Security in production
            }

            app.UseHttpsRedirection(); // Redirect HTTP to HTTPS
            app.UseStaticFiles(); // Serve static files like CSS, JavaScript, images, etc.

            app.UseRouting(); // Enable routing middleware
            app.UseAuthentication(); // Add authentication middleware for Identity
            app.UseAuthorization(); // Add authorization middleware for securing resources

            // Define default route for controllers
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Run the application
            app.Run();
        }
    }
}
