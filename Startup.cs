using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Assignment3VasylMilchevskyi.Data;
using Microsoft.EntityFrameworkCore;
using Assignment3VasylMilchevskyi.Services;

namespace Assignment3VasylMilchevskyi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
                services.AddTransient<ICommonDatastore, CommonStorageAccessDao>();
                services.AddTransient<StorageAccessMovie>();
                services.AddTransient<StorageAccessComment>();
                services.AddTransient<StorageAccessRating>();
                services.AddTransient<ImageUploader>();

            services.AddMvc()
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AuthorizeFolder("/Account/Manage");
                    options.Conventions.AuthorizePage("/Account/Logout");
                });

            services.AddAuthentication().AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
                googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
            });

            // Register no-op EmailSender used by account confirmation and password reset during development
            // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=532713
            services.AddSingleton<IEmailSender, EmailSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseIdentity();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "addMovie",
                    template: "{controller=Movies}/{action=AddMovie}/{id?}");
                routes.MapRoute(
                    name: "showMovies",
                    template: "{controller=Movies}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "movieDetails",
                    template: "{controller=Movies}/{action=Details}/{id?}");
                routes.MapRoute(
                 name: "addComment",
                    template: "{controller=Movies}/{action=AddComment}/{id?}");
            });
        }
    }
}
