using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PersonArchive.Entities;
using PersonArchive.Web.Models.ViewModels.Rdf;
using PersonArchive.Web.Services;

namespace PersonArchive.Web.Controllers
{
	[Route("rdf")]
	public class RdfController : Controller
	{
		private readonly IPersonData _personData;
		private readonly DataTripleStore.Services.IPersonData _rdfData;
		private readonly IOptions<DataTripleStoreServiceSettingsModel> _rdfDataServiceSettings;

		public RdfController(
			IPersonData personData,
			DataTripleStore.Services.IPersonData rdfData,
			IOptions<DataTripleStoreServiceSettingsModel> rdfDataServiceSettings)
		{
			_personData = personData;
			_rdfData = rdfData;
			_rdfDataServiceSettings = rdfDataServiceSettings;
		}

		[HttpGet, Route("person/{personGuid}")]
		public IActionResult PersonTriples(Guid personGuid)
		{
			// Must have an GUID
			if (personGuid == Guid.Empty)
				return NotFound();

			// Must have a person
			var personRdfTurtleFileExists =
				System.IO.File.Exists(
					$"{_rdfDataServiceSettings.Value.RdfTurtleFilesForPersonPath}/{personGuid}.ttl");

			// Do we have what we need so far?
			if (!personRdfTurtleFileExists)
				return NotFound();

			//TODO Page navigation <-- link back to person details page
			//ViewData["PersonGuid"] = person.PersonGuid;

			var viewModel = new PersonTriplesViewModel
			{
				PersonGuid = personGuid
			};

			viewModel.TriplesAsText =
				System.IO.File.ReadAllText(
					$"{_rdfDataServiceSettings.Value.RdfTurtleFilesForPersonPath}/{personGuid}.ttl");

			return View(viewModel);
		}
	}
}