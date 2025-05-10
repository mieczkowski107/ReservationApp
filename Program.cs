using Microsoft.EntityFrameworkCore;
using ReservationApp.Data;
using ReservationApp.Data.Repository;
using ReservationApp.Data.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using ReservationApp.Utility;
using ReservationApp.Services;
using System.Runtime.CompilerServices;
using Stripe;
using Hangfire;
using Hangfire.Dashboard;
using ReservationApp.Utility.Enums;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using Microsoft.Extensions.Options;
using Serilog.Sinks.MSSqlServer;
using Serilog.Ui.Web.Extensions;
using Serilog.Ui.MsSqlServerProvider.Extensions;
using Serilog.Ui.Core.Extensions;
using Serilog.Ui.Web.Models;
using Hangfire.Dashboard.BasicAuthorization;
using ReservationApp.Services.Interfaces;

namespace ReservationApp
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            #region builder Services
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddHostedService<AppointmentStatusService>();
            builder.Services.AddHostedService<AppointmentConfirmationService>();
            builder.Services.AddMemoryCache();

            #region Hangfire configuration
            builder.Services.AddHangfire(config =>
            {
                config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddHangfireServer();

            #endregion

            #region Identity
            builder.Services
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
               // .AddDefaultTokenProviders(); //for 2FA and email confirmation


            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            });

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });

            builder.Services.AddRazorPages();
            #endregion

            #region Scoped Services
            builder.Services.AddScoped<NotificationService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IEmailSender, EmailSender>();
            builder.Services.AddScoped<IServiceProvider, ServiceProvider>();
            builder.Services.AddScoped<IReportService, ReportService>();
            builder.Services.AddScoped<IImageService, CompanyImageService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            #endregion

            #region Stripe
            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
            StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
            #endregion

            #region Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Warning().Enrich
                .FromLogContext().WriteTo
                .MSSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
                new MSSqlServerSinkOptions()
                {
                    TableName = "Logs",
                    AutoCreateSqlTable = true,
                })
                .CreateLogger();
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog();
            builder.Host.UseSerilog();

            builder.Services.AddSerilogUi(options => options
                                .UseSqlServer(opts => opts
                                    .WithConnectionString(builder.Configuration.GetConnectionString("DefaultConnection"))
                                                                                .WithTable("Logs")));
            #endregion


            var app = builder.Build();

            
            #endregion

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseStatusCodePagesWithRedirects("/Home/Error/{404}");
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSerilogRequestLogging();
            app.UseSerilogUi(options=> options.HideSerilogUiBrand()
                                              .WithAuthenticationType(AuthenticationType.Custom));
            app.UseHangfireDashboard("/hangfire");

            app.MapRazorPages();
            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
