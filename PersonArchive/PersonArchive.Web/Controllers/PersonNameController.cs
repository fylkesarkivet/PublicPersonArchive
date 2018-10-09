using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonArchive.Entities.PersonDbContext;
using PersonArchive.SearchStore.Services;
using PersonArchive.Web.Models.CreateModels;
using PersonArchive.Web.Models.EditModels;
using PersonArchive.Web.Models.ModelStates;
using PersonArchive.Web.Services;

namespace PersonArchive.Web.Controllers
{
	[Route("person-names")]
	public class PersonNameController : Controller
	{
		private readonly IPersonData _personData;
		private readonly IPersonIndex _personSearchIndex;
		private readonly DataTripleStore.Services.IPersonData _rdfData;

		public PersonNameController(
			IPersonData personData,
			IPersonIndex personSearchIndex,
			DataTripleStore.Services.IPersonData rdfData)
		{
			_personData = personData;
			_personSearchIndex = personSearchIndex;
			_rdfData = rdfData;
		}

		[HttpGet, Route("create/{personGuid}")]
		public IActionResult Create(Guid personGuid)
		{
			// Must have an GUID
			if (personGuid == Guid.Empty)
				return NotFound();

			// Must have a person
			var person =
				_personData.ReadPerson(personGuid);

			// Do we have what we need so far?
			if (person == null)
				return NotFound();

			// Page navigation
			ViewData["PersonGuid"] = person.PersonGuid;

			var createModel = new PersonNameCreateModel();
			return View(createModel);
		}

		[HttpPost,
		 ValidateAntiForgeryToken,
		 Route("create/{personGuid}")]
		public IActionResult Create(
			Guid personGuid,
			PersonNameCreateModel createModel)
		{
			// Must have an GUID
			if (personGuid == Guid.Empty)
				return NotFound();

			// Must have a person
			var person =
				_personData.ReadPersonWithNames(personGuid);

			// Do we have what we need so far?
			if (person == null)
				return NotFound();

			// Page navigation
			ViewData["PersonGuid"] = person.PersonGuid;

			//
			// Validate
			//

			var form =
				new PersonNameModelState(
					ModelState,
					new Logic.Validate.PersonName(
						createModel.Prefix,
						createModel.First,
						createModel.Middle,
						createModel.Last,
						createModel.Suffix,
						null), // no name weight value in creation form
					person.Names,
					null);

			form.HasValidValues();
			if (!ModelState.IsValid) return View(createModel);

			form.HasUniquePersonNameToBeCreated();
			if (!ModelState.IsValid) return View(createModel);

			//
			// Create
			//

			var newName = new PersonName();
				newName.PersonId = person.PersonId;
				newName.Prefix = createModel.Prefix;
				newName.First = createModel.First;
				newName.Middle = createModel.Middle;
				newName.Last = createModel.Last;
				newName.Suffix = createModel.Suffix;

			// TimeCreated and TimeLastUpdated is set at class creation.

			// New name is added at the bottom of the list.
			newName.NameWeight = person.Names.Count + 1;

			try
			{
				_personData.AddPersonName(newName);
			}
			catch (DbUpdateException /* ex */)
			{
				//Log the error (uncomment ex variable name and write a log.)
				ModelState.AddModelError(
					"",
					"Unable to save changes. " +
					"Try again, and if the problem persists, " +
					"see your system administrator.");

				return View(createModel);
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

			var names = new List<string>();

			person = _personData.ReadPersonWithNames(personGuid);

			foreach (var name in person.Names.OrderByDescending(x => x.NameWeight))
			{
				var fullName =
					string.Join(" ", new[]
					{
						name.Prefix,
						name.First,
						name.Middle,
						name.Last,
						name.Suffix
					}.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray());

				names.Add(fullName);
			}

			// TODO try catch to handle errors
			_personSearchIndex.MergeNames(
				person.PersonGuid.ToString(),
				names.ToArray());

			//
			// Redirect
			//

			return RedirectToAction(
				"Details",
				"Person",
				new { personGuid = personGuid });
		}

		[HttpGet, Route("edit/{personNameId}")]
		public IActionResult Edit(int personNameId)
		{
			//
			// Get data
			//

			var name =
				_personData
					.ReadPersonNameWithNavigation(personNameId);

			if (name == null)
				return NotFound();

			var person =
				_personData
					.ReadPersonWithNames(
						name.Person.PersonGuid);

			if (person == null)
				return NotFound();

			//
			// Create edit model
			//

			var editModel = new PersonNameEditModel();

			// Editable in form
			editModel.Prefix = name.Prefix;
			editModel.First = name.First;
			editModel.Middle = name.Middle;
			editModel.Last = name.Last;
			editModel.Suffix = name.Suffix;
			editModel.NameWeight = name.NameWeight;

			// View helpers
			editModel.NameCount = person.Names.Count;
			ViewData["PersonGuid"] = name.Person.PersonGuid;

			return View(editModel);
		}

		[HttpPost,
		 ValidateAntiForgeryToken,
		 Route("edit/{personNameId}")]
		public IActionResult Edit(int personNameId, PersonNameEditModel editModel)
		{
			//
			// Get data
			//

			var name =
				_personData.GetPersonNameWithNavigation(personNameId);

			if (name == null)
				return NotFound();

			// Read to validate
			var person =
				_personData
					.ReadPersonWithNames(
						name.Person.PersonGuid);

			if (person == null)
				return NotFound();

			//
			// View helpers
			// Model must be ready if validation fails
			//

			editModel.NameCount = person.Names.Count;
			ViewData["PersonGuid"] = person.PersonGuid;

			//
			// Validate
			//

			var form =
				new PersonNameModelState(
					ModelState,
					new Logic.Validate.PersonName(
						editModel.Prefix,
						editModel.First,
						editModel.Middle,
						editModel.Last,
						editModel.Suffix,
						editModel.NameWeight),
					person.Names,
					name.PersonNameId);

			form.HasValidValues();
			if (!ModelState.IsValid) return View(editModel);

			form.HasUniquePersonNameToBeEdited();
			if (!ModelState.IsValid) return View(editModel);

			form.NameWeightHasValidNumber();
			if (!ModelState.IsValid) return View(editModel);

			//TODO check that the validation is the same as at the creation

			//
			// Edit
			//

			name.Prefix = editModel.Prefix;
			name.First = editModel.First;
			name.Middle = editModel.Middle;
			name.Last = editModel.Last;
			name.Suffix = editModel.Suffix;
			name.NameWeight = editModel.NameWeight;

			try
			{
				// Update name in form
				_personData.UpdatePersonName(name);

				// Addjust name weight at the other names
				_personData.AdjustAllNameWeightsButOne(
					person.PersonId,
					name.PersonNameId);
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

			var readPerson =
				_personData.ReadAllPersonData(person.PersonGuid);
			if (readPerson != null)
				_rdfData.AddOrUpdatePerson(readPerson);

			//
			// Update search index
			//

			var names = new List<string>();

			person =
				_personData
					.ReadPersonWithNames(
						name.Person.PersonGuid);

			foreach (var n in person.Names.OrderByDescending(x => x.NameWeight))
			{
				var fullName =
					string.Join(" ", new[]
					{
						n.Prefix,
						n.First,
						n.Middle,
						n.Last,
						n.Suffix
					}.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray());

				names.Add(fullName);
			}

			// TODO try catch to handle errors
			_personSearchIndex.MergeNames(
				person.PersonGuid.ToString(),
				names.ToArray());

			//
			// Redirect
			//

			return RedirectToAction(
				"Details",
				"Person",
				new { personGuid = name.Person.PersonGuid });
		}

		[HttpGet,
		 Route("delete/{personNameId}")]
		public IActionResult Delete(
			int personNameId)
		{
			var name =
				_personData
					.ReadPersonNameWithNavigation(
						personNameId);

			if (name == null)
				return NotFound();

			// Page navigation
			ViewData["PersonGuid"] = name.Person.PersonGuid;

			return View();
		}

		[HttpPost,
		 ValidateAntiForgeryToken,
		 Route("delete/{personNameId}")]
		public IActionResult DeleteByPost(
			int personNameId)
		{
			var name =
				_personData
					.GetPersonNameWithNavigation(
						personNameId);

			if (name == null)
				return NotFound();

			_personData.DeletePersonName(name);
			_personData.AdjustAllNameWeights(name.PersonId);

			//
			// Update RDF
			//

			var readPerson =
				_personData.ReadAllPersonData(name.Person.PersonGuid);
			if (readPerson != null)
				_rdfData.AddOrUpdatePerson(readPerson);

			//
			// Update search index
			//

			var names = new List<string>();

			var person =
				_personData
					.ReadPersonWithNames(
						name.Person.PersonGuid);

			foreach (var n in person.Names.OrderByDescending(x => x.NameWeight))
			{
				var fullName =
					string.Join(" ", new[]
					{
						n.Prefix,
						n.First,
						n.Middle,
						n.Last,
						n.Suffix
					}.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray());

				names.Add(fullName);
			}

			// TODO try catch to handle errors
			_personSearchIndex.MergeNames(
				person.PersonGuid.ToString(),
				names.ToArray());

			//
			// Redirect
			//

			return RedirectToAction(
				"Details",
				"Person",
				new { personGuid = name.Person.PersonGuid });
		}
	}
}