using Microsoft.AspNetCore.Mvc;

namespace InfoSurge.Controllers
{
    public class CommentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
