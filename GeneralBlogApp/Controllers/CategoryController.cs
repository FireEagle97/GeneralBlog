using GeneralBlogApp.Models;
using GeneralBlogApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GeneralBlogApp.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationContext _db;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IHttpContextAccessor _httpContext;
		private IEnumerable<Category>? _categoryList;

        public CategoryController(ApplicationContext db, IWebHostEnvironment hostEnvironment, IHttpContextAccessor httpContext)
        {
            _db = db;
            _hostEnvironment = hostEnvironment;
            _httpContext = httpContext;
			_categoryList = _db.Categories.ToList();
        }

		public IActionResult Index()
		{
			List<CategoryVM> objCategoryViewModels = new List<CategoryVM>();
			//List<CategoryVM> archivedCatVM = new List<CategoryVM>();
			if (_categoryList != null){
				foreach (var objCategory in _categoryList)
				{
					var category = new CategoryVM
					{
						Category = objCategory,
						MetaKeywords = new string[] {}
					};
					objCategoryViewModels.Add(category);


				}
			}
			
			//ViewBag.ArchivedCatList = archivedCatVM;
			return View(objCategoryViewModels);
		}
		public IActionResult Create()
		{
			var categoryVM = new CategoryVM();
			return View(categoryVM);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create(CategoryVM obj)
		{

			if (ModelState.IsValid)
			{

				var InputUser = new Category
				{

					Name = obj.Category.Name,
					MetaDescription = obj.Category.MetaDescription,
					MetaKeywords = String.Join(",", obj.MetaKeywords),

				};
				_db.Categories.Add(InputUser);
				_db.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(obj);

		}
		public IActionResult Edit(int id)
		{
			var catFromDB = _db.Categories.Where(cat => cat.Id == id).FirstOrDefault();
			var catVM = new CategoryVM
			{
				Category = catFromDB

			};
			if (catFromDB != null)
			{
				return View(catVM);
			}
			else
			{
				return View();
			}
		}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CategoryVM obj)
        {
            if (ModelState.IsValid)
            {
                var InputToBeEdited = _db.Categories.Find(obj.Category.Id);
                if (InputToBeEdited != null)
                {
                    
                    InputToBeEdited.Name = obj.Category.Name;
                    InputToBeEdited.MetaKeywords = String.Join(",", obj.MetaKeywords);
                    InputToBeEdited.MetaDescription = obj.Category.MetaDescription;
                    _db.Categories.Update(InputToBeEdited);
                    _db.SaveChanges();
                    return RedirectToAction("Edit", new { id = obj.Category.Id });

                }

            }
            return View(obj);
        }
    }
}
