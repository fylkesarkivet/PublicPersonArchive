using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonArchive.Entities.PersonDbContext;
using PersonArchive.Entities.SearchIndex;
using PersonArchive.SearchStore.Services;
using PersonArchive.Web.Models.Helpers;
using PersonArchive.Web.Models.CreateModels;
using PersonArchive.Web.Models.EditModels;
using PersonArchive.Web.Models.ViewModels;
using PersonArchive.Web.Services;

namespace PersonArchive.Web.Controllers
{
	[Route("person")]
	public class PersonController : Controller
	{
		private readonly IPersonData _personData;
		private readonly IPersonIndex _personSearchIndex;
		private readonly DataTripleStore.Services.IPersonData _rdfData;

		public PersonController(
			IPersonData personData,
			IPersonIndex personSearchIndex,
			DataTripleStore.Services.IPersonData rdfData)
		{
			_personData = personData;
			_personSearchIndex = personSearchIndex;
			_rdfData = rdfData;
		}

		[HttpGet, Route("create")]
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost,
		 ValidateAntiForgeryToken,
		 Route("create")]
		public IActionResult Create(
			PersonCreateModel createModel)
		{
			//
			// Validate
			//

			// Abort if something is wrong
			if (!ModelState.IsValid)
				return View(createModel);

			//
			// Create
			//

			var newPerson = new Person();
			newPerson.Gender = (PersonGender)createModel.Gender;

			try
			{
				_personData.AddPerson(newPerson);
			}
			catch (DbUpdateException /* ex */)
			{
				//Log the error (uncomment ex variable name and write a log.)
				ModelState.AddModelError(
					"",
					"Unable to add person. " +
					"Try again, and if the problem persists, " +
					"see your system administrator.");

				return View(createModel);
			}

			//
			// Update RDF
			//

			_rdfData.AddOrUpdatePerson(newPerson);

			//
			// Update search index
			//

			var persons = new List<PersonDocumentCreateModel>
			{
				// When creating a person we only have
				// PersonGuid and Gender.

				new PersonDocumentCreateModel
				{
					PersonGuid = newPerson.PersonGuid.ToString(),
					Gender = newPerson.Gender.ToString()
				}
			};

			// TODO try catch to handle errors
			_personSearchIndex.UploadDocuments(persons);

			//
			// Redirect
			//

			return RedirectToAction(
				"Details",
				"Person",
				new { personGuid = newPerson.PersonGuid });
		}

		[HttpGet, Route("{personGuid}")]
		public IActionResult Details(Guid personGuid)
		{
			// Validate
			if (personGuid == Guid.Empty)
				return BadRequest(ModelState);

			// Data
			var person =
				_personData.ReadAllPersonData(personGuid);

			// Model
			var viewModel = new PersonDetailsViewModel(person);

			// View
			return View(viewModel);
		}

		[HttpGet, Route("edit/{personGuid}")]
		public IActionResult Edit(Guid personGuid)
		{
			if (personGuid == Guid.Empty)
				return BadRequest(ModelState);

			var person =
				_personData.ReadPerson(personGuid);

			if (person == null)
				return NotFound();

			var editModel = new PersonEditModel();
				editModel.GenderType = (PersonGenderTypeOptions)person.Gender;

			// Page navigation
			ViewData["PersonGuid"] = person.PersonGuid;

			return View(editModel);
		}

		[HttpPost, Route("edit/{personGuid}")]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(Guid personGuid, PersonEditModel editModel)
		{
			if (personGuid == Guid.Empty)
				return BadRequest(ModelState);

			var person =
				_personData.GetPerson(personGuid);

			if (person == null)
				return NotFound();

			// Page navigation
			ViewData["PersonGuid"] = person.PersonGuid;

			//
			// Validate
			//

			if (!ModelState.IsValid)
				return View(editModel);

			//
			// Edit
			//

			person.Gender = (PersonGender)editModel.GenderType;

			try
			{
				_personData.UpdatePerson(person);
			}
			catch (DbUpdateException /* ex */)
			{
				// Log the error (uncomment ex variable name and write a log.)
				ModelState.AddModelError(
					"",
					"Unable to save changes. " +
					"Try again, and if the problem persists, " +
					"see your system administrator.");

				return View(editModel);
			}

			//
			// Update RDF
			//

			var readPerson = _personData.ReadAllPersonData(personGuid);
			if (readPerson != null)
				_rdfData.AddOrUpdatePerson(readPerson);

			//
			// Update search index
			//

			// TODO try catch to handle errors
			_personSearchIndex.MergeGender(
				person.PersonGuid.ToString(),
				person.Gender.ToString());

			//
			// Redirect
			//

			return RedirectToAction(
				"Details",
				"Person",
				new { personGuid = person.PersonGuid });
		}

		[HttpGet,
		 Route("delete/{personGuid}")]
		public IActionResult Delete(
			Guid personGuid)
		{
			var person =
				_personData
					.GetPerson(personGuid);

			if (person == null)
				return NotFound();

			// Page navigation
			ViewData["PersonGuid"] = personGuid;

			return View();
		}

		[HttpPost,
		 ValidateAntiForgeryToken,
		 Route("delete/{personGuid}")]
		public IActionResult DeleteByPost(
			Guid personGuid)
		{
			var person =
				_personData
					.GetPerson(personGuid);

			if (person == null)
				return NotFound();

			_personData.DeletePerson(person);

			//
			// Delete RDF
			//

			_rdfData.DeletePerson(person.PersonGuid);

			//
			// Update search index
			//

			// TODO try catch to handle errors
			_personSearchIndex.DeletePerson(person.PersonGuid);

			//
			// Redirect
			//

			return RedirectToAction(
				"PersonList",
				"Home");
		}
	}
}