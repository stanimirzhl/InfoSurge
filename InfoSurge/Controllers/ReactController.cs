using InfoSurge.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

			bool? currentReaction = await reactService.GetUserReactionToArticle(userId, articleId);

			if (currentReaction.HasValue)
			{
				if (currentReaction.Value == isLike)
				{
					await reactService.Remove(userId, articleId);
				}
				else
				{
					await reactService.ChangeReaction(userId, articleId, isLike);
				}
			}
			else
			{
				await reactService.AddAsync(userId, articleId, isLike);
			}

			return RedirectToAction("Details", "Article", new { id = articleId });
		}
	}
}
