﻿using InfoSurge.Configuration;
using InfoSurge.Core.Implementations;
using InfoSurge.Core.Interfaces;
using InfoSurge.Data;
using InfoSurge.Data.Common;
using InfoSurge.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using static InfoSurge.Data.Constants.DataConstants.CommentConstants;
using static InfoSurge.Data.Constants.DataConstants.UserConstants;

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
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICategoryUserService, CategoryUserService>();
            services.AddScoped<IEmailService, EmailService>();

            services.AddMvc(options =>
                options
                .Filters
                .Add(new AutoValidateAntiforgeryTokenAttribute()));

            services.AddResponseCompression();

            return services;
        }
        public static IServiceCollection AddEmailService(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
            services.AddTransient<IEmailService, EmailService>();

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
        public static async Task ApplyDatabaseMigrations(this IHost app)
        {
            using IServiceScope scope = app.Services.CreateScope();
            InfoSurgeDbContext db = scope.ServiceProvider.GetRequiredService<InfoSurgeDbContext>();
            RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            UserManager<User> userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            await db.Database.MigrateAsync();

            string[] roles = ["Administrator", "Editor", "Moderator", "User"];

            foreach (string role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminEmail = "admin@infosurge.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "User",
                    Status = UserStatus.Approved
                };

                await userManager.CreateAsync(adminUser, "admin123!");
                await userManager.AddToRolesAsync(adminUser, roles);
            }

            if (!db.Categories.Any())
            {
                List<Category> categories = new List<Category>
            {
                new Category { Name = "Спорт", Description = "Последните новини за спортни събития и отбори." },
                new Category { Name = "Култура", Description = "Всичко за културата." }
            };

                db.Categories.AddRange(categories);
                await db.SaveChangesAsync();
            }

            if (!db.Articles.Any())
            {
                Category firstCategory = db.Categories.First();

                Article article = new Article
                {
                    Title = "Карлос Насар коментира скандалите и бъдеще!",
                    Introduction = "Карлос Насар: Скандалите заплашват бъдещето на щангите, но ще продължа да се боря",
                    Content = "Олимпийският шампион и световен рекордьор по щанги Карлос Насар получи наградата за спортист на месец август. Насар, който спечели златен медал за България на игрите в Париж, коментира актуалните скандали в щангите, които продължават да предизвикват внимание.\"" +
                    "Насар заяви, че е бил принуден, заедно с колегите си, да превежда 30% от приходите си към федерацията. Президентът на Българската федерация по щанги, Антон Коджабашев, призова Насар да премине полиграфски тест, за да потвърди твърденията си.\"" +
                    "На награждаването Насар сподели: \"Ако въпросите са зададени правилно, Коджабашев няма да успее да премине теста. Но смятам, че има по-важни въпроси. Тренирам по два пъти на ден и спазвам режим. Проблемите трябва да бъдат обсъдени и решени. " +
                    "Тези 30% отиват за моята подготовка и, когато ги дам, се лишавам от част от необходимото. Препоръчвам на Коджабашев да бъде честен към себе си и към другите.\"" +
                    "Работим усилено, защото обичаме спорта. Клубовете с по 20-30 деца не могат да се справят без средства. Не трябва да се поддаваме на грешни решения, които могат да застрашат бъдещето на българските щанги." +
                    "Призовавам за хора с визия и ангажимент в нашата федерация. Нямам нищо против Стефан Ботев, но той трябва да уточни намеренията си. \"Ще видим какво можем да направим\" вече не е достатъчно.\"" +
                    "Имам още какво да покажа и искам да стана отново олимпийски шампион. Фокусът ми е върху Световното първенство в Бахрейн. Ако продължи бездействието на федерацията, може да загубим лиценза си и да не участваме. Моите критики към Коджабашев са професионални. " +
                    "Искам федерацията ни да бъде водена от хора, които обичат и разбират спорта.",
                    MainImageUrl = "/ArticleImageFolders/Article-1-Images/MainImage/pobeda.jpeg",
                    AuthorId = adminUser.Id,
                };

                db.Articles.Add(article);
                await db.SaveChangesAsync();

                db.CategoryArticles.Add(new CategoryArticle
                {
                    ArticleId = article.Id,
                    CategoryId = firstCategory.Id
                });

                db.CategoryArticles.Add(new CategoryArticle
                {
                    ArticleId = article.Id,
                    CategoryId = db.Categories.OrderBy(c => c.Name).First().Id
                });

                db.ArticleImages.Add(new ArticleImage
                {
                    ArticleId = article.Id,
                    ImgUrl = "/ArticleImageFolders/Article-1-Images/AdditionalImages/carlos.jpg"
                });

                db.Comments.Add(new Comment
                {
                    ArticleId = article.Id,
                    Title = "Коментар 1",
                    Content = "Тест, тест, тест, тест",
                    AuthorId = adminUser.Id,
                    Status = CommentStatus.Approved
                });

                db.Reactions.Add(new Reaction
                {
                    ArticleId = article.Id,
                    UserId = adminUser.Id,
                    IsLike = true
                });

                db.SavedArticles.Add(new SavedArticle
                {
                    ArticleId = article.Id,
                    UserId = adminUser.Id
                });

                await db.SaveChangesAsync();
            }
        }
    }
}
