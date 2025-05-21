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

            services.AddMvc(options =>
                options
                .Filters
                .Add(new AutoValidateAntiforgeryTokenAttribute()));

            services.AddResponseCompression();

            return services;
        }

        public static IServiceCollection AddDbServices(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
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
        public static void ApplyDatabaseMigrations(this IHost app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<InfoSurgeDbContext>();
            db.Database.Migrate();
        }
    }
}
