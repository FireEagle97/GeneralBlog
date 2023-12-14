using GeneralBlogApp.Models;
using GeneralBlogApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace GeneralBlogApp.Controllers
{
    public class ArticleController : Controller
    {
		private readonly ApplicationContext _db;
		private readonly IWebHostEnvironment _hostEnvironment;
		private readonly IHttpContextAccessor _httpContext;
		private IEnumerable<Article>? _articleList;
		private IEnumerable<Category>? _categoryList;
		public ArticleController(ApplicationContext db, IWebHostEnvironment hostEnvironment, IHttpContextAccessor httpContext) {
            _db = db;
            _hostEnvironment = hostEnvironment;
            _httpContext = httpContext;
            _articleList = _db.Articles.ToList();
			_categoryList = _db.Categories.ToList();
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
		public IActionResult Create()
		{
			var categoryList = _categoryList.Select(
				category => new SelectListItem
				{
					Text = category.Name,
					Value = category.Id.ToString()

				});
			ArticleVM articleObj = new ArticleVM
			{
				CategoryList = categoryList

			};
			return View(articleObj);
		}
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ArticleVM obj)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                var uploads = Path.Combine(wwwRootPath, @"assets\article\banners");
                if (obj.MainImageFile != null)
                {
                    //check invalid chars
                    string fileName = "art_banner" + DateTime.Now.ToString("MMddyyyy");

                    
                    if (!Directory.Exists(uploads))
                    {
                        Directory.CreateDirectory(uploads);
                    }
                    var extension = Path.GetExtension(obj.MainImageFile.FileName);

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        obj.MainImageFile.CopyTo(fileStreams);
                    }
                    obj.Article.MainImageFileName = fileName + extension;
                    obj.Article.MainImageUrl = @"\assets\article\banners\" + fileName + extension;
                    obj.Article.MainImageUploadDate = DateTime.Now;

                }

                var objArticle = new Article
                {
                    Title = obj.Title,
                    CatId = obj.CategoryId,
                    Summary = obj.Article.Summary,
                    MainImageUrl = obj.Article.MainImageUrl,
                    MainImageFileName = obj.Article.MainImageFileName,
                    MainImageUploadDate = obj.Article.MainImageUploadDate,
                    MainImageAltTag = obj.Article.MainImageAltTag,
                    IsArchived = false,
                    IsPublished = false,
                    MetaKeywords = String.Join(",", obj.MetaKeywords),
                    MetaDescription = obj.Article.MetaDescription,
                    MetaTitle = obj.Article.MetaTitle,
                    //Slug = obj.Article.Slug.Replace(" ", "-")
                };
                _db.Articles.Add(objArticle);
                _db.SaveChanges();
                //update main image filename
                var articleFromDB = _db.Articles.OrderBy(article => article.Id).Last();
                if (!string.IsNullOrEmpty(articleFromDB.MainImageUrl))
                {
                    string fileExtension = Path.GetExtension(articleFromDB.MainImageUrl);
                    string newFileName = Path.GetFileNameWithoutExtension(articleFromDB.MainImageUrl) + "_" + articleFromDB.Id;
                    string oldFilePath = Path.Combine(uploads, articleFromDB.MainImageFileName);
                    using (var fileStreams = new FileStream(Path.Combine(uploads, newFileName + fileExtension), FileMode.Create))
                    {
                        obj.MainImageFile.CopyTo(fileStreams);
                    }
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                    articleFromDB.MainImageFileName = newFileName + fileExtension;
                    articleFromDB.MainImageUrl = @"\assets\article\banners\" + newFileName + fileExtension;
                    _db.Articles.Update(articleFromDB);
                    _db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return View(obj);

        }
    }
}
