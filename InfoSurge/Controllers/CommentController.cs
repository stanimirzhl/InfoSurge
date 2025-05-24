using InfoSurge.Core;
using InfoSurge.Core.DTOs.Article;
using InfoSurge.Core.DTOs.Comment;
using InfoSurge.Core.Interfaces;
using InfoSurge.Models.Comment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace InfoSurge.Controllers
{
    public class CommentController : Controller
    {
        private ICommentService commentService;
        private IArticleService articleService;
        private IEmailService emailService;

        public CommentController(ICommentService commentService, IArticleService articleService, IEmailService emailService)
        {
            this.commentService = commentService;
            this.articleService = articleService;
            this.emailService = emailService;
        }

        [HttpGet]
        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> All(int pageIndex = 1, int pageSize = 10)
        {
            PagingModel<CommentDto> pagedCommentDto = await commentService.GetAllPendingPagedComments(pageIndex, pageSize);

            PagingModel<CommentVM> pagedCommentVM = pagedCommentDto.Map(x => new CommentVM()
            {
                AuthorName = x.AuthorName,
                Content = x.Content,
                Title = x.Title,
                ArticleTitle = x.ArticleTitle,
                CreatedOn = x.CreatedOn.ToString("f", System.Globalization.CultureInfo.GetCultureInfo("bg-BG")),
                Id = x.Id,
                ArticleId = x.ArticleId,
            });

            return View(pagedCommentVM);
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> AddComment(int articleId, CommentFormModel formModel)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Details", "Article", new { id = articleId });
            }

            try
            {
                ArticleDto articleDto = await articleService.GetByIdAsync(articleId);

                CommentDto commentDto = new CommentDto
                {
                    Title = formModel.Title,
                    Content = formModel.Content,
                    AuthorId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                };
                await commentService.AddAsync(articleId, commentDto);
            }
            catch (NoEntityException ex)
            {
                return NotFound();
            }
            TempData["AddedComment"] = "Коментарът ви е изпратен за проверка от нашите модератори";
            return RedirectToAction("Details", "Article", new { id = articleId });
        }

        [HttpPost]
        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> Approve(int commentId, int articleId)
        {
            try
            {
                await commentService.Approve(commentId);

                List<string> userEmails = await commentService.GetAllUsersEmailWhoHaveCommentedUnderArticle(articleId);

                foreach (string email in userEmails)
                {
                    string message = "<p>Нов коментар под новина, под която сте коментирали.</p>";
                    await emailService.SendEmailAsync(email, "Ново известие", message);
                }
            }
            catch (NoEntityException ex)
            {

                return NotFound();
            }

            return RedirectToAction("All");
        }

        [HttpPost]
        [Authorize(Roles = "Moderator")]
        public async Task<IActionResult> Reject(int commentId)
        {
            try
            {
                await commentService.Remove(commentId);
            }
            catch (NoEntityException ex)
            {
                return NotFound();
            }

            return RedirectToAction("All");
        }
    }
}
