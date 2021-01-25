using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineMarket.Models;
using OnlineMarket.Utility;
using System;
using System.Linq;

namespace OnlineMarket.DataAccess.Data.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

       public DbInitializer(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            try
            {
                if(_context.Database.GetPendingMigrations().Count() > 0)
                {
                    _context.Database.Migrate();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (_context.Roles.Any(i => i.Name == SD.Role_SuperAdmin)) return;

            _roleManager.CreateAsync(new IdentityRole(SD.Role_SuperAdmin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Company)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Individual)).GetAwaiter().GetResult();

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "admin@mail.com",
                Email = "admin@mail.com",
                EmailConfirmed = true,
                Name = "admin"
            }, "111111Test$").GetAwaiter().GetResult();

            ApplicationUser user = _context.ApplicationUsers.Where(i => i.Email == "admin@mail.com").FirstOrDefault();

            _userManager.AddToRoleAsync(user, SD.Role_SuperAdmin).GetAwaiter().GetResult();
        }
    }
}
