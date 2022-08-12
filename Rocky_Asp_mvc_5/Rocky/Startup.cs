using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocky_DataAccess;
using Rocky_Utility;
using Rocky_DataAccess.Repository.IReposotory;
using Rocky_DataAccess.Repository;
using System;
using Rocky_Model;
using Rocky_Model.Viewmodels;
using Rocky_Utility.BrainTree;
using Rocky_DataAccess.Initializer;

namespace Rocky2
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
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                Configuration.GetConnectionString("DefaultConnection")));

            //identity
            services.AddIdentity<IdentityUser, IdentityRole>()
                    .AddDefaultTokenProviders()
                    .AddDefaultUI()
                    .AddEntityFrameworkStores<ApplicationDbContext>();
            //mail
            services.AddTransient<IEmailSender, EmailSender>();


            //session
            services.AddHttpContextAccessor();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.Configure<BrainTreeSettings>(Configuration.GetSection("BrainTree"));   //BrainTree Keys
            services.AddSingleton<IBrainTreeGate, BrainTreeGate>();                         //BrainTree 



            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IAplicationTypeRepository, AplicationTypeRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IInquiryHeaderRepository, InquiryHeaderRepository>();
            services.AddScoped<IInquiryDetailRepository, InquiryDetailRepository>();

            services.AddScoped<IOrderHeaderRepository, OrderHeaderRepository>();
            services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();

            services.AddScoped<IAplicationUserRepository, AplicationUserRepository>();

            services.AddScoped<UserRepo>();

            services.AddScoped<ProductUserViewModel>();


            services.AddScoped<IDbInitializer, DbInitializer>();

            services.AddAuthentication().AddFacebook(Options =>
            {
                Options.AppId = "2388410891297300";
                Options.AppSecret = "88576b7cee674b192f1cd8bd888238eb";
            });

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDbInitializer dbInitializer) //conveyer
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseDeveloperExceptionPage();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            dbInitializer.Initialize(); //init data
            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
