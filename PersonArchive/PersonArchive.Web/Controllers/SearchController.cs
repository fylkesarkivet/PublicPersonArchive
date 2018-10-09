using Microsoft.AspNetCore.Mvc;
using PersonArchive.SearchStore.Services;
using PersonArchive.Web.Models.SearchModels;
using PersonArchive.Web.Models.ViewModels.Search;
using PersonArchive.Web.Services;

namespace PersonArchive.Web.Controllers
{
	[Route("search")]
	public class SearchController : Controller
	{
		private readonly IPersonData _personData;
		private readonly IPersonIndex _personSearchService;

		public SearchController(
			IPersonData personData,
			IPersonIndex personSearchService)
		{
			_personData = personData;
			_personSearchService = personSearchService;
		}

		[HttpGet]
		public IActionResult Index(SearchInputModel searchInputModel)
		{
			var searchText = searchInputModel.s;
			var filterText = searchInputModel.f;

			// Search
			var searchResult = 
				_personSearchService.SearchPersonNames(searchText, filterText);

			// ViewModel
			var viewModel = new IndexViewModel();
			viewModel.AddSearchInputFromUser(searchInputModel);
			viewModel.AddPersonSearchResult(searchResult);

			// View
			return View(viewModel);
		}
	}
}