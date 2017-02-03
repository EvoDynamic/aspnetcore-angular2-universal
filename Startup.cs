using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Angular2Spa.Server.Data;
using Angular2Spa.Server.Data.Users;
using Angular2Spa.Server.Infrastructure;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Angular2Spa
{

    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
            services.AddMemoryCache();

	        services.AddEntityFramework();
	        services.AddIdentity<ApplicationUser, IdentityRole>(config =>
		        {
			        config.User.RequireUniqueEmail = true;
			        config.Password.RequireNonAlphanumeric = false;
			        config.Cookies.ApplicationCookie.AutomaticChallenge = false;
		        })
				.AddEntityFrameworkStores<ApplicationDbContext>()
		        .AddDefaultTokenProviders();

			services.AddDbContext<ApplicationDbContext>(options =>
			        options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

			// Add ApplicationDbContext's DbSeeder
			services.AddSingleton<DbSeeder>();
		}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, DbSeeder dbSeeder)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                // Webpack middleware setup
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions {
                    HotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

			// Add a custom Jwt Provider to generate Tokens
			app.UseJwtProvider();
			// Add the Jwt Bearer Header Authentication to validate Tokens
			app.UseJwtBearerAuthentication(new JwtBearerOptions()
			{
				AutomaticAuthenticate = true,
				AutomaticChallenge = true,
				RequireHttpsMetadata = false,
				TokenValidationParameters = new TokenValidationParameters()
				{
					IssuerSigningKey = JwtProvider.SecurityKey,
					ValidateIssuerSigningKey = true,
					ValidIssuer = JwtProvider.Issuer,
					ValidateIssuer = false,
					ValidateAudience = false
				}
			});

			//  ** MVC / WebAPI Routing & default SPA fallback Routing
			app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });

			// Seed the Database (if needed)
			try
			{
				dbSeeder.SeedAsync().Wait();
			}
			catch (AggregateException e)
			{
				throw new Exception(e.ToString());
			}
		}
        
    }
}
