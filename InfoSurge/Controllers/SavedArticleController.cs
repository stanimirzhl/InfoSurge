using InfoSurge.Core.Interfaces;
using InfoSurge.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InfoSurge.Controllers
{
    public class SavedArticleController : Controller
    {
        private readonly ISavedArticleService savedArticleService;
        private readonly IAccountService accountService;

        public SavedArticleController(ISavedArticleService savedArticleService, IAccountService accountService)
        {
            this.savedArticleService = savedArticleService;
            this.accountService = accountService;
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> SaveArticle(int articleId)
        {
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (await savedArticleService.HasUserSavedThisArticle(currentUserId, articleId))
            {
                await savedArticleService.Remove(currentUserId, articleId);
                return RedirectToAction("Details","Article" ,new { id = articleId });
            }

            await savedArticleService.AddAsync(currentUserId, articleId);
            return RedirectToAction("Details","Article", new { id = articleId });
        }
    }
}
