using GeneralBlogApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace GeneralBlogApp.ViewModels
{
	public class CategoryVM
	{
		public Category Category { get; set; }
		public string[]? MetaKeywords { get; set; }
		
	}
}
