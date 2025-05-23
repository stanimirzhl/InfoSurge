using InfoSurge.Core;
using InfoSurge.Core.DTOs.Article;
using InfoSurge.Core.Interfaces;
using InfoSurge.Data.Models;
using InfoSurge.Models.Account;
using InfoSurge.Models.Article;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Configuration;
using System.Security.Claims;

namespace InfoSurge.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService accountService;
        private readonly ISavedArticleService savedArticleService;

        public AccountController(IAccountService accountService, ISavedArticleService savedArticleService)
        {
            this.accountService = accountService;
            this.savedArticleService = savedArticleService;
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterFormModel formModel)
        {
            if (!ModelState.IsValid)
            {
                return View(formModel);
            }

            if (await accountService.EmailExists(formModel.Email))
            {
                ModelState.AddModelError(string.Empty, "Потребител с тази електронна поща вече съществува!");
                return View(formModel);
            }

            if (await accountService.UserNameExists(formModel.UserName))
            {
                ModelState.AddModelError(string.Empty, "Потребител с това име вече съществува!");
                return View(formModel);
            }

            var user = new User()
            {
                UserName = formModel.UserName,
                FirstName = formModel.FirstName,
                LastName = formModel.LastName,
                Email = formModel.Email,
            };

            IdentityResult result = await accountService.AddAsync(user, formModel.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(formModel);
            }

            TempData["Register"] = "Регистрацията ви ще бъде разгледана от нашите администратори и ще получите имейл на дадената електронна поща ако сте били одобрени!";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login(string ReturnUrl = null)
        {
            ReturnUrl ??= Url.Content("~/");

            LoginFormModel login = new LoginFormModel()
            {
                ReturnUrl = ReturnUrl
            };

            return View(login);
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginFormModel formModel, string ReturnUrl = null)
        {
            ReturnUrl ??= Url.Content("~/");

            if (!ModelState.IsValid)
            {
                return View(formModel);
            }

            if (!await accountService.UserNameExists(formModel.UserName))
            {
                ModelState.AddModelError(string.Empty, $"Потребител с име {formModel.UserName} не съществува!");
                return View(formModel);
            }

            if (!await accountService.IsUserApproved(formModel.UserName))
            {
                ModelState.AddModelError(string.Empty, "Потребителят не е одобрен!");
                return View(formModel);
            }

            if (await accountService.IsPasswordValidForUserName(formModel.UserName, formModel.Password))
            {
                ModelState.AddModelError(string.Empty, "Грешна парола!");
                return View(formModel);
            }

            Microsoft.AspNetCore.Identity.SignInResult result = await accountService.Login(formModel.UserName, formModel.Password);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Неуспешен вход!");
                return View(formModel);
            }

            if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
            {
                return LocalRedirect(ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await accountService.Logout();
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> Profile(string section)
        {
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                ViewData["IsEditor"] = User.IsInRole("Editor");

                User user = await accountService.GetCurrentUserById(currentUserId);

                List<ArticleDto> articleDtos = await savedArticleService.GetSavedArticlesByUserId(currentUserId);

                SettingsViewModel formModel = new SettingsViewModel
                {
                    Section = section,
                    ProfileFormModel = new UpdateProfileFormModel()
                    {
                        Email = user.Email,
                        UserName = user.UserName,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                    },
                    PasswordFormModel = new ChangePasswordFormModel(),
                    UserFullName = user.FirstName + " " + user.LastName,
                    SavedArticles = articleDtos.Select(x => new ArticleVM()
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Introduction = x.Introduction,
                        PublishDate = x.PublishDate.ToString("f", System.Globalization.CultureInfo.GetCultureInfo("bg-BG")),
                        MainImageUrl = x.MainImageUrl
                    }).ToList()
                };

                return View(formModel);
            }
            catch (NoEntityException ex)
            {
                return Unauthorized();
            }
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateProfile()
        {
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                User user = await accountService.GetCurrentUserById(currentUserId);

                SettingsViewModel formModel = new SettingsViewModel
                {
                    ProfileFormModel = new UpdateProfileFormModel()
                    {
                        Email = user.Email,
                        UserName = user.UserName,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                    },
                    UserFullName = user.FirstName + " " + user.LastName
                };

                return PartialView("_SettingsPartial", formModel);
            }
            catch (NoEntityException ex)
            {
                return Unauthorized();
            }
        }
        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateProfile(SettingsViewModel formModel)
        {
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                User user = await accountService.GetCurrentUserById(currentUserId);

                IdentityResult result = await accountService.UpdateProfile(user.Id, formModel.ProfileFormModel.UserName, formModel.ProfileFormModel.FirstName, formModel.ProfileFormModel.LastName, formModel.ProfileFormModel.Email);

                if (result.Succeeded)
                {
                    await accountService.SignInAgain(user);

                    TempData["Success"] = "Профилът е успешно обновен!";
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return RedirectToAction("Profile", new { section = "profile" });
                }

                return RedirectToAction("Profile", new { section = "profile" });
            }
            catch (NoEntityException ex)
            {
                return Unauthorized();
            }
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        public IActionResult ChangePassword()
        {
            SettingsViewModel formModel = new SettingsViewModel();
            return PartialView("_ChangePasswordPartial", formModel);
        }
        [Authorize(Roles = "User")]
        public async Task<IActionResult> ChangePassowrd(SettingsViewModel formModel)
        {
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                User user = await accountService.GetCurrentUserById(currentUserId);

                bool isPasswordValid = await accountService.IsPasswordValidForUserName(user.UserName, formModel.PasswordFormModel.OldPassword);

                if (isPasswordValid)
                {
                    TempData["Error"] = "Неуспешно променяне на данните, паролата е невалидна!";
                    return RedirectToAction("Profile", new { section = "changePassword" });
                }

                IdentityResult result = await accountService.UpdatePassword(user, formModel.PasswordFormModel.OldPassword, formModel.PasswordFormModel.NewPassword);

                if (result.Succeeded)
                {
                    await accountService.SignInAgain(user);

                    TempData["Success"] = "Паролата е успешно обновена!";

                    return RedirectToAction("Profile", new { section = "changePassword" });

                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return RedirectToAction("Profile", new { section = "changePassword" });

                }
            }
            catch (NoEntityException ex)
            {
                return Unauthorized();
            }
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public async Task<IActionResult> SavedArticles()
        {
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                List<ArticleDto> articleDtos = await savedArticleService.GetSavedArticlesByUserId(currentUserId);

                SettingsViewModel formModel = new SettingsViewModel
                {
                    SavedArticles = articleDtos.Select(x => new ArticleVM()
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Introduction = x.Introduction,
                        PublishDate = x.PublishDate.ToString("f", System.Globalization.CultureInfo.GetCultureInfo("bg-BG")),
                        MainImageUrl = x.MainImageUrl
                    }).ToList()
                };

                return PartialView("_SavedArticlesPartial", formModel);
            }
            catch(NoEntityException ex)
            {
                return Unauthorized();
            }
        }
    }
}
