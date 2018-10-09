using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PersonArchive.Web.Models;
using PersonArchive.Web.Models.ViewModels;
using PersonArchive.Web.Services;

namespace PersonArchive.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly IPersonData _personData;

		public HomeController(IPersonData personData)
		{
			_personData = personData;
		}

		public IActionResult Index()
		{
			return View();
		}

		[HttpGet, Route("persons")]
		public IActionResult PersonList()
		{
			var viewModel = new PersonListViewModel();

			var persons = _personData.ReadLastAddedPersonsWithAllData(10);

			viewModel.AddPersonsToViewModel(persons.ToList());

			return View(viewModel);
		}

		//
		// Other
		//

		public IActionResult Error()
		{
			return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
		}
	}
}