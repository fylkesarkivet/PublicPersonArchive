using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonArchive.Entities.PersonDbContext;
using PersonArchive.Web.Models.CreateModels;
using PersonArchive.Web.Models.EditModels;
using PersonArchive.Web.Models.Helpers;
using PersonArchive.Web.Models.ModelStates;
using PersonArchive.Web.Services;

namespace PersonArchive.Web.Controllers
{
	[Route("person-descriptions")]
	public class PersonDescriptionController : Controller
	{
		private readonly IPersonData _personData;
		private readonly DataTripleStore.Services.IPersonData _rdfData;

		public PersonDescriptionController(
			IPersonData personData,
			DataTripleStore.Services.IPersonData rdfData)
		{
			_personData = personData;
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
				_personData.ReadPersonWithDescriptions(personGuid);

			// Do we have what we need so far?
			if (person == null)
				return NotFound();

			// Page navigation
			ViewData["PersonGuid"] = person.PersonGuid;

			var createModel = new PersonDescriptionCreateModel();
			return View(createModel);
		}

		[HttpPost, Route("create/{personGuid}")]
		[ValidateAntiForgeryToken]
		public IActionResult Create(
			Guid personGuid,
			PersonDescriptionCreateModel createModel)
		{
			// Must have an GUID
			if (personGuid == Guid.Empty)
				return NotFound();

			// Must have a person
			var person =
				_personData.ReadPersonWithDescriptions(personGuid);

			// Do we have what we need so far?
			if (person == null)
				return NotFound();

			// Page navigation
			ViewData["PersonGuid"] = person.PersonGuid;

			//
			// Validate
			//

			var form =
				new PersonDescriptionModelState(
					ModelState,
					new Logic.Validate.PersonDescription(
						createModel.Text),
					createModel.Type,
					person.Descriptions,
					null);

			form.HasValidValues();
			if (!ModelState.IsValid) return View(createModel);

			form.HasUniqueTypeToBeCreated();
			if (!ModelState.IsValid) return View(createModel);

			//
			// Create
			//

			var newDescription = new PersonDescription();
			newDescription.PersonId = person.PersonId;
			newDescription.Type = (PersonDescriptionType)createModel.Type;
			newDescription.Description = createModel.Text;

			try
			{
				_personData.AddPersonDescription(newDescription);
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

			// Descriptions are at this time not searchable.

			//
			// Redirect
			//

			return RedirectToAction(
				"Details",
				"Person",
				new { personGuid = person.PersonGuid });
		}

		[HttpGet, Route("edit/{personDescriptionId}")]
		public IActionResult Edit(int personDescriptionId)
		{
			//
			// Get data
			//

			var description =
				_personData
					.GetPersonDescriptionWithNavigation(
						personDescriptionId);

			if (description == null)
				return NotFound();

			//
			// Create edit model
			//

			var editModel = new PersonDescriptionEditModel();
			editModel.Type = (PersonDescriptionTypeOptions)description.Type;
			editModel.Text = description.Description;

			// View helpers
			ViewData["PersonGuid"] = description.Person.PersonGuid;

			return View(editModel);
		}

		[HttpPost, Route("edit/{personDescriptionId}")]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(
			int personDescriptionId,
			PersonDescriptionEditModel editModel)
		{
			//
			// Get data
			//

			var description =
				_personData
					.GetPersonDescriptionWithNavigation(
						personDescriptionId);

			if (description == null)
				return NotFound();

			// Read to validate
			var person =
				_personData
					.ReadPersonWithDescriptions(
						description.Person.PersonGuid);

			if (person == null)
				return NotFound();

			//
			// View helpers
			// Model must be ready if validation fails
			//

			ViewData["PersonGuid"] = person.PersonGuid;

			//
			// Validate
			//

			var form =
				new PersonDescriptionModelState(
					ModelState,
					new Logic.Validate.PersonDescription(
						editModel.Text),
				editModel.Type,
				person.Descriptions,
				description.PersonDescriptionId);

			form.HasValidValues();
			if (!ModelState.IsValid) return View(editModel);

			form.HasUniqueTypeToBeEdited();
			if (!ModelState.IsValid) return View(editModel);

			//
			// Edit
			//

			description.Type = (PersonDescriptionType)editModel.Type;
			description.Description = editModel.Text;

			try
			{
				_personData.UpdatePersonDescription(description);
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
				_personData.ReadAllPersonData(description.Person.PersonGuid);
			if (readPerson != null)
				_rdfData.AddOrUpdatePerson(readPerson);

			//
			// Update search index
			//

			// Descriptions are at this time not searchable.

			//
			// Redirect
			//

			return RedirectToAction(
				"Details",
				"Person",
				new { personGuid = description.Person.PersonGuid });
		}

		[HttpGet,
		 Route("delete/{personDescriptionId}")]
		public IActionResult Delete(
			int personDescriptionId)
		{
			var description =
				_personData
					.ReadPersonDescriptionWithNavigation(
						personDescriptionId);

			if (description == null)
				return NotFound();

			// Page navigation
			ViewData["PersonGuid"] = description.Person.PersonGuid;

			return View();
		}

		[HttpPost,
		 ValidateAntiForgeryToken,
		 Route("delete/{personDescriptionId}")]
		public IActionResult DeleteByPost(
			int personDescriptionId)
		{
			var description =
				_personData
					.GetPersonDescriptionWithNavigation(
						personDescriptionId);

			if (description == null)
				return NotFound();

			_personData.DeletePersonDescription(description);

			//
			// Update RDF
			//

			var readPerson =
				_personData.ReadAllPersonData(description.Person.PersonGuid);
			if (readPerson != null)
				_rdfData.AddOrUpdatePerson(readPerson);

			//
			// Update search index
			//

			// Descriptions are at this time not searchable.

			//
			// Redirect
			//

			return RedirectToAction(
				"Details",
				"Person",
				new { personGuid = description.Person.PersonGuid });
		}
	}
}