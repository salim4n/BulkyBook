using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {

        
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _db;

        public DbInitializer(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUnitOfWork unitOfWork,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _db = db;
        }

        public void Initialize()
        {
            //migration if no migration
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }

            }
            catch (Exception e)
            {

            }

            //create roles if they are not created
            if (!_roleManager.RoleExistsAsync(SD.Role_User_Admin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Employee)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Indi)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Comp)).GetAwaiter().GetResult();

                //if roles are not created, then we will create admin user

                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "laimeche160@gmail.com",
                    Email = "laimeche160@gmail.com",
                    Name = "Salim Laimeche",
                    PhoneNumber = "0749628470",
                    StreetAddress = "je vit ici et la bas",
                    State = "un peu partout",
                    PostalCode = "000000",
                    City = "Los Santos"
                }, "Admin123*").GetAwaiter().GetResult();

                ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "laimeche160@gmail.com");

                _userManager.AddToRoleAsync(user, SD.Role_User_Admin).GetAwaiter().GetResult();
            }

            return;

        }
    }
}
