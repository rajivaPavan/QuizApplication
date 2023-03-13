using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuizApplication.DbContext;
using QuizApplication.DbOperations;
using QuizApplication.Handlers;
using QuizApplication.Models;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.FeatureFilters;
using QuizApplication.Authorization;
using QuizApplication.Entities;

namespace QuizApplication
{
    public class Startup
    {
        private readonly string _environmentName;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _environmentName = env.EnvironmentName;
        }
        
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            
            // MySQL DB connection service
            services.AddDbContextPool<AppDbContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));

            // EFCore identity and set password validations  
            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 0;
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<AppDbContext>();
            
            // add repositories
            services.AddScoped<IQuestionRepository, QuestionRepository>();
            services.AddScoped<IQuizRepository, QuizRepository>();

            // add handlers
            services.AddScoped<IAuthHandler, AuthHandler>();
            services.AddScoped<IQuizHandler, QuizHandler>();

            services.AddAzureAppConfiguration();
            services.AddFeatureManagement()
                .AddFeatureFilter<TimeWindowFilter>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.QuizAccessAndTime,
                    policy =>
                    {
                        policy.Requirements.Add(
                            new QuizAccessAndTimeRequirement());
                    });
            });
            
            services.Configure<QuizSettings>(
                Configuration.GetSection($"QuizSettings:{_environmentName}"));
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

            app.UseAzureAppConfiguration();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            
            app.UseRouting();

            app.UseCors();
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Quiz}/{action=Home}/{id?}");
            });
        }
    }
}