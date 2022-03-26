using System;
using E_Assignment.Areas.Identity.Data;
using E_Assignment.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(E_Assignment.Areas.Identity.IdentityHostingStartup))]
namespace E_Assignment.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<E_AssignmentDbContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("E_AssignmentDbContextConnection")));

                services.AddDefaultIdentity<ApplicationUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireDigit = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequiredLength = 8;
                })
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<E_AssignmentDbContext>();                    
            });
        }
    }
}