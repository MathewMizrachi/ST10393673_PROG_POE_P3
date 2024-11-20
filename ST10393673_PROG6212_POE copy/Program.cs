using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using ST10393673_PROG6212_POE.Models;
using ST10393673_PROG6212_POE.Services;
using Microsoft.Azure.Cosmos.Table;

namespace ST10393673_PROG6212_POE
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Configure the database context for Identity
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Configure Identity services
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add ClaimService for managing claims
            builder.Services.AddScoped<IClaimService, ClaimService>();

            // Add BlobStorageService with connection string from appsettings.json
            builder.Services.AddSingleton<BlobStorageService>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var connectionString = configuration["AzureBlobStorage:ConnectionString"];
                return new BlobStorageService(connectionString);
            });

            // Add TableService to manage Azure Table Storage
            builder.Services.AddSingleton<TableService>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var connectionString = configuration["AzureTableStorage:ConnectionString"];
                return new TableService(connectionString);
            });

            // Add UserService for user-related table storage operations
            builder.Services.AddScoped<UserService>(provider =>
            {
                var tableService = provider.GetRequiredService<TableService>();
                return new UserService(tableService);
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Enable authentication and authorization
            app.UseAuthentication();
            app.UseAuthorization();

            // Map default controller route
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Run the application
            app.Run();
        }
    }
}
