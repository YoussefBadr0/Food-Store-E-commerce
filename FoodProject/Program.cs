using FoodProject.Data;
using FoodProject.Hubs;
using FoodProject.Models;
using FoodProject.Repositories.Implementations;
using FoodProject.Repositories.Interfaces;
using FoodProject.Services.Implementations;
using FoodProject.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace FoodProject
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
                options.Lockout.AllowedForNewUsers = true;


                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false; 
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
            })
             .AddEntityFrameworkStores<ApplicationDbContext>()
             .AddDefaultTokenProviders();
            
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Login/Login";
                options.AccessDeniedPath = "/Login/AccessDenied";

                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.SlidingExpiration = true;

                
            });

            builder.Services.AddScoped<IEmailService, EmailService>();

            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();

            builder.Services.AddSignalR();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddSingleton<IUserIdProvider, UserIdProvider>();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var roleManager =
                    scope.ServiceProvider
                        .GetRequiredService<RoleManager<AppRole>>();

                string[] roles = { "Admin", "User" };

                foreach (var role in roles)
                {
                    bool roleExists =
                        await roleManager.RoleExistsAsync(role);

                    if (!roleExists)
                    {
                        await roleManager.CreateAsync(
                            new AppRole
                            {
                                Name = role
                            });
                    }
                }
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

                string adminEmail = configuration["AdminSettings:Email"];
                string adminPassword = configuration["AdminSettings:Password"];

                var adminUser = await userManager.FindByEmailAsync(adminEmail);

                if (adminUser == null)
                {
                    var user = new AppUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        FirstName = "System",
                        LastName = "Admin",
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(user, adminPassword);

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "Admin");
                    }
                }
            }
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapHub<NotificationHub>("/notificationHub");
            app.Run();
        }
    }
}
