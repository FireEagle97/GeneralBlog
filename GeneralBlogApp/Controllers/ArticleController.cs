using Microsoft.AspNetCore.Mvc;

namespace GeneralBlogApp.Controllers
{
    public class ArticleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
