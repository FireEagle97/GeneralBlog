using GeneralBlogApp.Models;
using GeneralBlogApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GeneralBlogApp.Controllers
{
    public class ArticleController : Controller
    {
		private readonly ApplicationContext _db;
		private readonly IWebHostEnvironment _hostEnvironment;
		private readonly IHttpContextAccessor _httpContext;
		private IEnumerable<Article>? _articleList;
		public ArticleController(ApplicationContext db, IWebHostEnvironment hostEnvironment, IHttpContextAccessor httpContext) {
            _db = db;
            _hostEnvironment = hostEnvironment;
            _httpContext = httpContext;
            _articleList = _db.Articles.ToList();
        }
        public IActionResult Index()
        {
			List<ArticleVM> articleVMList = new List<ArticleVM>();
			foreach(var objArticle in _articleList)
			{
				var categoryName = getArticleCategoryName(objArticle.Id);
				var article = new ArticleVM
				{
					Id = objArticle.Id,
					Article = objArticle,
					Title = objArticle.Title,
					CategoryId = objArticle.CatId,
					CategoryName = categoryName,

				};

				articleVMList.Add(article);
			}

            return View(articleVMList);
        }
		public string getArticleCategoryName(int categoryId)
		{
			var categoryName = "";
			var objCategory = _db.Categories.Where(cat => cat.Id == categoryId);
			if (objCategory.Any())
			{
				categoryName = objCategory.FirstOrDefault().Name;
			}
			return categoryName;
		}

	}
}
