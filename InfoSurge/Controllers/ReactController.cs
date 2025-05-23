using InfoSurge.Core.Interfaces;
using InfoSurge.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InfoSurge.Controllers
{
    public class ReactController : Controller
    {
        private readonly IReactService reactService;
        private readonly IAccountService accountService;

        public ReactController(IReactService reactService, IAccountService accountService)
        {
            this.reactService = reactService;
            this.accountService = accountService;
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> LikeUnLike(int articleId, bool isLike)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            bool hasUserReacted = await reactService.HasUserReactedToArticle(userId, articleId);

            if (hasUserReacted == true)
            {
               await reactService.ChangeReaction(userId, articleId, isLike);
            }
            else
            {
                await reactService.AddAsync(userId, articleId, isLike);
            }

            return RedirectToAction("Details", "Article", new { id = articleId });
        }
    }
}
