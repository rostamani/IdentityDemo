using AutoMapper;
using IdentitySample.Mappers;
using IdentitySample.Models.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersianTranslation.Identity;
using IdentitySample.Repositories;
using IdentitySample.Repositories.Services;

namespace IdentitySample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("IdentitySampleConnectionString"));
            });

            services.AddIdentity<IdentityUser, IdentityRole>(options=> {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;

                options.User.RequireUniqueEmail = true;

                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

                
            }).
                AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders().AddErrorDescriber<PersianIdentityErrorDexcriber>();
            services.AddAuthentication().AddGoogle(options=> {
                options.ClientId = "704291046373-t7i4ebnis7gumeu1jh3dn61i0olojn7c.apps.googleusercontent.com";
                options.ClientSecret = "n3hTPwzIiy75ocSFBNRog9jw";
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("EmployeeIndex", policy =>
                {
                    policy.RequireClaim(ClaimType.EmployeeIndex, true.ToString());
                });

                options.AddPolicy("ClaimOrRole", policy =>
                {
                    policy.RequireAssertion(context => context.User.HasClaim(ClaimType.EmployeeIndex, true.ToString())
                    || context.User.IsInRole("Admin")||context.User.IsInRole("User"));
                });
            });

            #region DI
            services.AddAutoMapper(typeof(IdentityMapping));
            services.AddScoped<IMessageSender, MessageSender>();
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
