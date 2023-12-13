using GeneralBlogApp.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GeneralBlogApp.ViewModels
{
	public class ArticleVM
	{
		[Key]
		public int Id { get; set; }
		[Required(ErrorMessage = "required", AllowEmptyStrings = false)]
		public string Title { get; set; } = string.Empty;
		[Required(ErrorMessage = "required", AllowEmptyStrings = false)]
		public int CategoryId { get; set; }
		public Article? Article { get; set; }
		public string? CategoryName { get; set; }
		//public string? AuthorDisplayName { get; set; }
		public IFormFile? MainImageFile { get; set; }
		public string[]? MetaKeywords { get; set; }
		//public bool? IsPublished { get; set; }
		//public bool? IsArchived { get; set; }
		[ValidateNever]
		public IEnumerable<SelectListItem> CategoryList { get; set; } = Enumerable.Empty<SelectListItem>();


	}
}