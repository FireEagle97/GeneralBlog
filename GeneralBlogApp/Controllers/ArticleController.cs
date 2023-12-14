using GeneralBlogApp.Models;
using GeneralBlogApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http.Extensions;

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
        public IActionResult Edit(int? id)
        {
            var categoryList = _categoryList.Select(
                    u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    });
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var objArticleFromDB = _db.Articles.Find(id);
            if (objArticleFromDB == null)
            {
                return NotFound();
            }
            var objArticleVM = new ArticleVM
            {
                Id = objArticleFromDB.Id,
                Article = objArticleFromDB,
                Title = objArticleFromDB.Title,
                CategoryId = objArticleFromDB.CatId,
                MetaKeywords = objArticleFromDB.MetaKeywords?.Split(','),
                CategoryList = categoryList,

            };
            return View(objArticleVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ArticleVM obj)
        {
            if (ModelState.IsValid)
            {
                //try
                //{
                var InputToBeEdited = _db.Articles.Find(obj.Id);
                if (InputToBeEdited != null)
                {
                    //Upload new Banner File
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    if (obj.MainImageFile != null)
                    {
                        //check invalid chars
                        string fileName = "art_banner" + DateTime.Now.ToString("MMddyyyy") + "_" + obj.Id;
                        var uploads = Path.Combine(wwwRootPath, @"assets\article\banners");
                        var extension = Path.GetExtension(obj.MainImageFile.FileName);

                        using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                        {
                            obj.MainImageFile.CopyTo(fileStreams);
                        }
                        obj.Article.MainImageFileName = fileName + extension;
                        obj.Article.MainImageUrl = @"\assets\article\banners\" + fileName + extension;
                        obj.Article.MainImageUploadDate = DateTime.Now;
                        InputToBeEdited.MainImageFileName = obj.Article.MainImageFileName;
                        InputToBeEdited.MainImageUrl = obj.Article.MainImageUrl;
                        InputToBeEdited.MainImageUploadDate = obj.Article.MainImageUploadDate;
                    }
                    InputToBeEdited.Title = obj.Title;
                    InputToBeEdited.CatId = obj.CategoryId;
                    InputToBeEdited.MainImageAltTag = obj.Article.MainImageAltTag;
                    InputToBeEdited.Summary = obj.Article.Summary;
                    InputToBeEdited.IsPublished = obj.Article.IsPublished;
                    InputToBeEdited.IsArchived = obj.Article.IsArchived;
                    InputToBeEdited.MetaKeywords = String.Join(",", obj.MetaKeywords);
                    InputToBeEdited.MetaDescription = obj.Article.MetaDescription;
                    InputToBeEdited.MetaTitle = obj.Article.MetaTitle;
                    //InputToBeEdited.Slug = articleSlug.Replace(" ", "-");
                    //if (InputToBeEdited.Published)
                    //{
                    //    DateTime publishDate = DateTime.Now;
                    //    InputToBeEdited.PublishedAt = new DateTime(publishDate.Year, publishDate.Month, publishDate.Day);
                    //    InputToBeEdited.PublishedBy = userObj.FirstName + " " + userObj.LastName;
                    //    InputToBeEdited.Archived = false;
                    //}
                    //else
                    //{
                    //    InputToBeEdited.PublishedAt = null;
                    //    InputToBeEdited.PublishedBy = string.Empty;
                    //    InputToBeEdited.Published = false;
                    //}
                    _db.Articles.Update(InputToBeEdited);
                    _db.SaveChanges();
                    return RedirectToAction("Edit", new { id = obj.Id });


                }
                //}
                //catch (Exception ex)
                //{
                //    string strBody = String.Format("Date/Time: {0}<br/>Url: {1}<br/>Error: {2}<br/>Stack Trace: {3}", DateTime.Now.ToString(), HttpContext.Request.GetDisplayUrl(), ex.Message, ex.StackTrace);
                //    MailData mailData = new MailData
                //    {
                //        Receiver = _configuration["AdminEmails"],
                //        Subject = "Exception occured in LernaAdmin Application",
                //        Body = strBody
                //    };
                //    bool result = await _mail.SendAsync(mailData, new System.Threading.CancellationToken());

                //    if (result)
                //    {
                //        ViewBag.status = "success";
                //    }
                //    else
                //    {
                //        ViewBag.status = "error";
                //    }
                //}

            }
            return View(obj);
        }
    }
}
