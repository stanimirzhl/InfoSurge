using InfoSurge.Core.Implementations;
using InfoSurge.Core.Interfaces;
using InfoSurge.Data;
using InfoSurge.Data.Common;
using InfoSurge.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace InfoSurge.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IArticleService, ArticleService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IArticleImageService, ArticleImageService>();
            services.AddScoped<ICategoryArticleService, CategoryArticleService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ISavedArticleService, SavedArticleService>();
            services.AddScoped<IReactService, ReactService>();

            services.AddMvc(options =>
                options
                .Filters
                .Add(new AutoValidateAntiforgeryTokenAttribute()));

            services.AddResponseCompression();

            return services;
        }

        public static IServiceCollection AddDbServices(this IServiceCollection services, IConfiguration config)
        {
            string connectionString = config.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            services.AddDbContext<InfoSurgeDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            return services;
        }
        public static IServiceCollection AddIdentityServices(this IServiceCollection services)
        {
            services.AddDefaultIdentity<User>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 5;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<InfoSurgeDbContext>();

            return services;
        }
        public static IServiceCollection AddAccountOptions(this IServiceCollection services)
        {
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.ReturnUrlParameter = "ReturnUrl";
            });

            return services;
        }
        public static void ApplyDatabaseMigrations(this IHost app)
        {
            using IServiceScope scope = app.Services.CreateScope();
            InfoSurgeDbContext db = scope.ServiceProvider.GetRequiredService<InfoSurgeDbContext>();
            db.Database.Migrate();
        }
    }
}
