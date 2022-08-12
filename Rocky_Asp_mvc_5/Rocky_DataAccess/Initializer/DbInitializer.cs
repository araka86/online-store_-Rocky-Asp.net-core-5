using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Rocky_Model;
using Rocky_Utility;
namespace Rocky_DataAccess.Initializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public void Initialize()
        {
            try
            {
                //проверка незавершенных миграций
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate(); //завершение миграций
                }
                else
                {
                    return;
                }
            }
            catch
            {
            }
            if (!_roleManager.RoleExistsAsync(WebConstanta.AdminRole).GetAwaiter().GetResult())
            {
                 _roleManager.CreateAsync(new IdentityRole(WebConstanta.AdminRole)).GetAwaiter().GetResult();
                 _roleManager.CreateAsync(new IdentityRole(WebConstanta.CustomerRole)).GetAwaiter().GetResult(); 
            }
            _userManager.CreateAsync(new AplicationUser
            {
                UserName = "Admin",
                Email = "araka86@gmail.com",
                EmailConfirmed = true,
                FullName = "Admin",
                PhoneNumber = "0931111111"
            }, "123!@#QWEqwe").GetAwaiter().GetResult();
            AplicationUser user = _db.AplicationUser.First(u => u.Email == "araka86@gmail.com");
            _userManager.AddToRoleAsync(user, WebConstanta.AdminRole).GetAwaiter().GetResult();

        }
    }
}
