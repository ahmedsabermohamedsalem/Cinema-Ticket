using Cinema_Ticket.DataAccess;
using Cinema_Ticket.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Cinema_Ticket.Utilities.DBSeeder
{
    public class DBInitialization : IDBInitialization
    {
        private readonly ApplicationDBcContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DBInitialization> _logger;

        public DBInitialization(
            ApplicationDBcContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            ILogger<DBInitialization> logger)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            try
            {
                if (_context.Database.GetPendingMigrations().Any())
                {
                    _context.Database.Migrate();
                }

                if (!_roleManager.Roles.Any())
                {
                    await _roleManager.CreateAsync(
                        new IdentityRole(CD.SUPER_ADMIN_ROLE));

                    await _roleManager.CreateAsync(
                        new IdentityRole(CD.ADMIN_ROLE));

                    await _roleManager.CreateAsync(
                        new IdentityRole(CD.EMPLOYEE_ROLE));

                    await _roleManager.CreateAsync(
                        new IdentityRole(CD.CUSTOMER_ROLE));


                    var result = await _userManager.CreateAsync(
                        new ApplicationUser()
                        {
                            Name = "SuperAdmin",
                            UserName = "SuperAdmin",
                            Email = "superadmin@eraasoft.com",
                            address = "cairo",
                            EmailConfirmed = true
                        },
                        "SuperAdmin@123");


                    var user = await _userManager
                        .FindByNameAsync("SuperAdmin");

                    if (user != null)
                    {
                        await _userManager.AddToRoleAsync(
                            user,
                            CD.SUPER_ADMIN_ROLE);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while initializing database");
            }
        }
    }
}