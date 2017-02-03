using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Angular2Spa.Server.Data.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Angular2Spa.Server.Data
{
    public class DbSeeder
    {
		#region Private Members
		private readonly ApplicationDbContext DbContext;
		private readonly RoleManager<IdentityRole> RoleManager;
		private readonly UserManager<ApplicationUser> UserManager;
		#endregion Private Members

		#region Constructor
		public DbSeeder(ApplicationDbContext dbContext, RoleManager<IdentityRole>
		roleManager, UserManager<ApplicationUser> userManager)
		{
			DbContext = dbContext;
			RoleManager = roleManager;
			UserManager = userManager;
		}
		#endregion Constructor

		#region Public Methods
		public async Task SeedAsync()
		{
			// Create default Users
			if (await DbContext.Users.CountAsync() == 0) await CreateUsersAsync();
			// Create default Items (if there are none) and Comments
			// if (await DbContext.Items.CountAsync() == 0) CreateItems();
		}
		#endregion Public Methods

		#region Methods
		private async Task CreateUsersAsync()
	    {
		    // local variables
		    DateTime createdDate = new DateTime(2017, 02, 01, 12, 30, 00);
		    DateTime lastModifiedDate = DateTime.Now;
		    string role_Administrators = "Administrators";
		    string role_Registered = "Registered";
		    //Create Roles (if they doesn't exist yet)
		    if (!await RoleManager.RoleExistsAsync(role_Administrators))
			    await
				    RoleManager.CreateAsync(new IdentityRole(role_Administrators));
		    if (!await RoleManager.RoleExistsAsync(role_Registered))
			    await
				    RoleManager.CreateAsync(new IdentityRole(role_Registered));
		    // Create the "Admin" ApplicationUser account (if it doesn't exist already)
		    var user_Admin = new ApplicationUser()
		    {
			    UserName = "Admin",
			    Email = "brian@evodynamic.com",
			    CreatedDate = createdDate,
			    LastModifiedDate = lastModifiedDate
		    };
		    // Insert "Admin" into the Database and also assign the "Administrator" role to him.
		    if (await UserManager.FindByIdAsync(user_Admin.Id) == null)
		    {
			    await UserManager.CreateAsync(user_Admin, "Pass4Admin");
			    await UserManager.AddToRoleAsync(user_Admin, role_Administrators);
			    // Remove Lockout and E-Mail confirmation.
			    user_Admin.EmailConfirmed = true;
			    user_Admin.LockoutEnabled = false;
		    }
#if DEBUG
		    // Create some sample registered user accounts (if they don't exist already)
		    var user_Ryan = new ApplicationUser()
		    {
			    UserName = "Ryan",
			    Email = "ryan@evodynamic.com",
			    CreatedDate = createdDate,
			    LastModifiedDate = lastModifiedDate,
			    EmailConfirmed = true,
			    LockoutEnabled = false
		    };
		    var user_Solice = new ApplicationUser()
		    {
			    UserName = "Solice",
			    Email = "solice@evodynamic.com",
			    CreatedDate = createdDate,
			    LastModifiedDate = lastModifiedDate,
			    EmailConfirmed = true,
			    LockoutEnabled = false
		    };
		    var user_Vodan = new ApplicationUser()
		    {
			    UserName = "Vodan",
			    Email = "vodan@evodynamic.com",
			    CreatedDate = createdDate,
			    LastModifiedDate = lastModifiedDate,
			    EmailConfirmed = true,
			    LockoutEnabled = false
		    };
		    // Insert sample registered users into the Database and also assign the "Registered" role to him.
		    if (await UserManager.FindByIdAsync(user_Ryan.Id) == null)
		    {
			    await UserManager.CreateAsync(user_Ryan, "Pass4Ryan");
			    await UserManager.AddToRoleAsync(user_Ryan, role_Registered);
			    // Remove Lockout and E-Mail confirmation.
			    user_Ryan.EmailConfirmed = true;
			    user_Ryan.LockoutEnabled = false;
		    }
		    if (await UserManager.FindByIdAsync(user_Solice.Id) == null)
		    {
			    await UserManager.CreateAsync(user_Solice, "Pass4Solice");
			    await UserManager.AddToRoleAsync(user_Solice, role_Registered);
			    // Remove Lockout and E-Mail confirmation.
			    user_Solice.EmailConfirmed = true;
			    user_Solice.LockoutEnabled = false;
		    }
		    if (await UserManager.FindByIdAsync(user_Vodan.Id) == null)
		    {
			    await UserManager.CreateAsync(user_Vodan, "Pass4Vodan");
			    await UserManager.AddToRoleAsync(user_Vodan, role_Registered);
			    // Remove Lockout and E-Mail confirmation.
			    user_Vodan.EmailConfirmed = true;
			    user_Vodan.LockoutEnabled = false;
		    }
#endif
		    await DbContext.SaveChangesAsync();
	    }
		#endregion
	}
}
