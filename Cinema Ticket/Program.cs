using Cinema_Ticket.DataAccess;
using Cinema_Ticket.Models;
using Cinema_Ticket.Repositories;
using Cinema_Ticket.services;
using Ecommerce531.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Cinema_Ticket.Utilities.DBSeeder;

namespace Cinema_Ticket
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // User Settings
                options.User.RequireUniqueEmail = true;

                // Password Settings
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.SignIn.RequireConfirmedEmail = true;
            })
              .AddEntityFrameworkStores<ApplicationDBcContext>()
              .AddDefaultTokenProviders();


            builder.Services.AddDbContext<ApplicationDBcContext>(
                options =>
                {
                    options.UseSqlServer(
                        builder.Configuration.GetConnectionString(
                            "DefaultConnection"));
                });
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login"; // Where to send unauthorized users
                options.AccessDeniedPath = "/Identity/Account/AccessDenied"; // Where to send forbidden users
            });

            builder.Services.AddScoped<IRepository<Category>, Repository<Category>>();
            builder.Services.AddScoped<IRepository<Cinema>, Repository<Cinema>>();
            builder.Services.AddScoped<IRepository<Actor>, Repository<Actor>>();
            builder.Services.AddScoped<IRepository<Movie>, Repository<Movie>>();
            builder.Services.AddScoped<IRepository<MovieActor>, Repository<MovieActor>>();
            builder.Services.AddScoped<IRepository<MovieImage>, Repository<MovieImage>>();
            builder.Services.AddScoped<IRepository<Cart>, Repository<Cart>>();

            builder.Services.AddScoped<IRepository<Show>, Repository<Show>>();

            builder.Services.AddScoped<IRepository<Hall>, Repository<Hall>>();
            builder.Services.AddScoped<IDBInitialization, DBInitialization>();



            builder.Services.AddScoped<IRepository<ApplicationUserOtp>, Repository<ApplicationUserOtp>>();

            builder.Services.AddTransient<IEmailSender, EmailSender>();


            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var dbInitializer = scope.ServiceProvider.GetRequiredService<IDBInitialization>();
                await dbInitializer.InitializeAsync();
            }



            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapStaticAssets();

            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Identity}/{controller=Account}/{action=login}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}