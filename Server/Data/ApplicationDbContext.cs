using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Angular2Spa.Server.Data.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Angular2Spa.Server.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
		#region Constructor
	    public ApplicationDbContext(DbContextOptions options) : base(options)
	    {
	    }
		#endregion Constructor

	    #region Methods
	    protected override void OnModelCreating(ModelBuilder modelBuilder)
	    {
		    base.OnModelCreating(modelBuilder);
		    modelBuilder.Entity<ApplicationUser>().ToTable("Users");
	    }
		#endregion Methods

	    #region Properties
	    #endregion Properties
	}
}
