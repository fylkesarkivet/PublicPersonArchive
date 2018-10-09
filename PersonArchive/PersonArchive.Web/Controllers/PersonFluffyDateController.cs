using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonArchive.Entities.PersonDbContext;
using PersonArchive.Logic.Validate;
using PersonArchive.SearchStore.Services;
using PersonArchive.Web.Models.CreateModels;
using PersonArchive.Web.Models.EditModels;
using PersonArchive.Web.Models.Helpers;
using PersonArchive.Web.Models.ModelStates;
using PersonArchive.Web.Services;

namespace PersonArchive.Web.Controllers
{
	[Route("person-dates")]
	public class PersonFluffyDateController : Controller
	{
		private readonly IPersonData _personData;
		private readonly IPersonIndex _personSearchIndex;
		private readonly DataTripleStore.Services.IPersonData _rdfData;

		public PersonFluffyDateController(
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

			var createModel = new PersonFluffyDateCreateModel();
			return View(createModel);
		}

		[HttpPost,
		 ValidateAntiForgeryToken,
		 Route("create/{personGuid}")]
		public IActionResult Create(
			Guid personGuid,
			PersonFluffyDateCreateModel createModel)
		{
			// Must have GUID
			if (personGuid == Guid.Empty)
				return NotFound();

			// Must have a person
			var person =
				_personData.ReadPersonWithFluffyDates(personGuid);

			// Do we have what we need so far?
			if (person == null)
				return NotFound();

			// Page navigation
			ViewData["PersonGuid"] = person.PersonGuid;

			//
			// Validate
			//

			var form = 
				new PersonFluffyDateModelState(
					ModelState,
					new FluffyDate(
						createModel.Year,
						createModel.Month,
						createModel.Day),
					createModel.Type,
					person.FluffyDates,
					null);

			form.HasValidValues();
			if (!ModelState.IsValid) return View(createModel);

			form.HasUniqueTypeToBeCreated();
			if (!ModelState.IsValid) return View(createModel);

			form.BirthVsDeathYear();
			if (!ModelState.IsValid) return View(createModel);

			form.BirthVsDeathMonth();
			if (!ModelState.IsValid) return View(createModel);

			form.BirthVsDeathDay();
			if (!ModelState.IsValid) return View(createModel);

			form.PersonHasNormalAge();
			if (!ModelState.IsValid) return View(createModel);

			form.DateIsNotInTheFuture(
				DateTime.Now.Year,
				DateTime.Now.Month,
				DateTime.Now.Day);
			if (!ModelState.IsValid) return View(createModel);

			//
			// Create
			//

			var newPersonFluffyDate = new PersonFluffyDate();
				newPersonFluffyDate.PersonId = person.PersonId;
				newPersonFluffyDate.Year = createModel.Year;
				newPersonFluffyDate.Month = createModel.Month;
				newPersonFluffyDate.Day = createModel.Day;
				newPersonFluffyDate.Type =
					(PersonFluffyDateType) createModel.Type;

			try
			{
				_personData.AddPersonFluffyDate(newPersonFluffyDate);
			}
			catch (DbUpdateException /* ex */)
			{
				// Log the error (uncomment ex variable name and write a log.)
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

			// Update selected year type
			switch (createModel.Type)
			{
				case PersonFluffyDateTypeOptions.Birth:
					_personSearchIndex.MergeYearOfBirth(
						person.PersonGuid.ToString(),
						createModel.Year);
					break;
				case PersonFluffyDateTypeOptions.Death:
					_personSearchIndex.MergeYearOfDeath(
						person.PersonGuid.ToString(),
						createModel.Year);
					break;
			}

			// Update age

			// Get updated dates
			person = _personData.ReadPersonWithFluffyDates(personGuid);

			var birthFluffyDate = 
				person.FluffyDates.FirstOrDefault(x => 
					x.Type == PersonFluffyDateType.Birth);

			if (birthFluffyDate?.Year != null)
			{
				var fluffyDateStart =
					new FluffyDate(
						birthFluffyDate.Year,
						birthFluffyDate.Month,
						birthFluffyDate.Day);

				var fluffyDateEnd =
					new FluffyDate(
						DateTime.Now.Year,
						DateTime.Now.Month,
						DateTime.Now.Day);

				var deathFluffyDate =
					person.FluffyDates.FirstOrDefault(x =>
						x.Type == PersonFluffyDateType.Death);

				if (deathFluffyDate?.Year != null)
				{
					fluffyDateEnd =
						new FluffyDate(
							deathFluffyDate.Year,
							deathFluffyDate.Month,
							deathFluffyDate.Day);
				}

				var age =
					new Age(
						fluffyDateStart,
						fluffyDateEnd);

				if (age.InYears != null)
				{
					_personSearchIndex.MergeAge(
						person.PersonGuid.ToString(),
						age.InYears);
				}
			}

			//
			// Redirect
			//

			return RedirectToAction(
				"Details",
				"Person",
				new { personGuid = personGuid });
		}

		[HttpGet, Route("edit/{personFluffyDateId}")]
		public IActionResult Edit(int personFluffyDateId)
		{
			// Get data
			var fluffyDate =
				_personData
					.ReadPersonFluffyDateWithNavigation(personFluffyDateId);

			if (fluffyDate == null)
				return NotFound();

			// Load data to model
			var editModel = new PersonFluffyDateEditModel();
				editModel.Year = fluffyDate.Year;
				editModel.Month = fluffyDate.Month;
				editModel.Day = fluffyDate.Day;
				editModel.Type = (PersonFluffyDateTypeOptions) fluffyDate.Type;

			// Page navigation
			ViewData["PersonGuid"] = fluffyDate.Person.PersonGuid;

			return View(editModel);
		}

		[HttpPost,
		 ValidateAntiForgeryToken,
		 Route("edit/{personFluffyDateId}")]
		public IActionResult Edit(
			int personFluffyDateId, 
			PersonFluffyDateEditModel editModel)
		{
			var fluffyDate =
				_personData
					.GetPersonFluffyDateWithNavigation(
						personFluffyDateId);

			if (fluffyDate == null)
				return NotFound();

			// Read to validate
			var person =
				_personData
					.ReadPersonWithFluffyDates(
						fluffyDate.Person.PersonGuid);

			if (person == null)
				return NotFound();

			// Page navigation
			ViewData["PersonGuid"] = 
				fluffyDate.Person.PersonGuid;

			//
			// Validate
			//

			var form =
				new PersonFluffyDateModelState(
					ModelState,
					new FluffyDate(
						editModel.Year,
						editModel.Month,
						editModel.Day),
					editModel.Type,
					person.FluffyDates,
					fluffyDate.PersonFluffyDateId);

			form.HasValidValues();
			if (!ModelState.IsValid) return View(editModel);

			form.HasUniqueTypeToBeEdited();
			if (!ModelState.IsValid) return View(editModel);

			form.BirthVsDeathYear();
			if (!ModelState.IsValid) return View(editModel);

			form.BirthVsDeathMonth();
			if (!ModelState.IsValid) return View(editModel);

			form.BirthVsDeathDay();
			if (!ModelState.IsValid) return View(editModel);

			form.PersonHasNormalAge();
			if (!ModelState.IsValid) return View(editModel);

			form.DateIsNotInTheFuture(
				DateTime.Now.Year,
				DateTime.Now.Month,
				DateTime.Now.Day);
			if (!ModelState.IsValid) return View(editModel);

			//
			// Edit
			//

			fluffyDate.Year = editModel.Year;
			fluffyDate.Month = editModel.Month;
			fluffyDate.Day = editModel.Day;
			fluffyDate.Type = (PersonFluffyDateType) editModel.Type;

			try
			{
				_personData.UpdatePersonFluffyDate(fluffyDate);
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
				_personData.ReadAllPersonData(fluffyDate.Person.PersonGuid);
			if (readPerson != null)
				_rdfData.AddOrUpdatePerson(readPerson);

			//
			// Update search index
			//

			// Update selected year type
			switch (editModel.Type)
			{
				case PersonFluffyDateTypeOptions.Birth:
					_personSearchIndex.MergeYearOfBirth(
						person.PersonGuid.ToString(),
						editModel.Year);
					break;
				case PersonFluffyDateTypeOptions.Death:
					_personSearchIndex.MergeYearOfDeath(
						person.PersonGuid.ToString(),
						editModel.Year);
					break;
			}

			// Update age

			// Get updated dates
			person = _personData.ReadPersonWithFluffyDates(person.PersonGuid);

			var birthFluffyDate =
				person.FluffyDates.FirstOrDefault(x =>
					x.Type == PersonFluffyDateType.Birth);

			if (birthFluffyDate?.Year != null)
			{
				var fluffyDateStart =
					new FluffyDate(
						birthFluffyDate.Year,
						birthFluffyDate.Month,
						birthFluffyDate.Day);

				var fluffyDateEnd =
					new FluffyDate(
						DateTime.Now.Year,
						DateTime.Now.Month,
						DateTime.Now.Day);

				var deathFluffyDate =
					person.FluffyDates.FirstOrDefault(x =>
						x.Type == PersonFluffyDateType.Death);

				if (deathFluffyDate?.Year != null)
				{
					fluffyDateEnd =
						new FluffyDate(
							deathFluffyDate.Year,
							deathFluffyDate.Month,
							deathFluffyDate.Day);
				}

				var age =
					new Age(
						fluffyDateStart,
						fluffyDateEnd);

				if (age.InYears != null)
				{
					_personSearchIndex.MergeAge(
						person.PersonGuid.ToString(),
						age.InYears);
				}
			}

			//
			// Redirect
			//

			return RedirectToAction(
				"Details",
				"Person",
				new { personGuid = fluffyDate.Person.PersonGuid });
		}

		[HttpGet,
		 Route("delete/{personFluffyDateId}")]
		public IActionResult Delete(
			int personFluffyDateId)
		{
			var fluffyDate =
				_personData
					.ReadPersonFluffyDateWithNavigation(
						personFluffyDateId);

			if (fluffyDate == null)
				return NotFound();

			// Page navigation
			ViewData["PersonGuid"] = fluffyDate.Person.PersonGuid;

			return View();
		}

		[HttpPost,
		 ValidateAntiForgeryToken,
		 Route("delete/{personFluffyDateId}")]
		public IActionResult DeleteByPost(
			int personFluffyDateId)
		{
			var fluffyDate =
				_personData
					.GetPersonFluffyDateWithNavigation(
						personFluffyDateId);

			if (fluffyDate == null)
				return NotFound();

			_personData.DeletePersonFluffyDate(fluffyDate);

			//
			// Update RDF
			//

			var readPerson =
				_personData.ReadAllPersonData(fluffyDate.Person.PersonGuid);
			if (readPerson != null)
				_rdfData.AddOrUpdatePerson(readPerson);

			//
			// Update search index
			//

			// Update selected year type
			switch (fluffyDate.Type)
			{
				case PersonFluffyDateType.Birth:
					_personSearchIndex.MergeYearOfBirth(
						fluffyDate.Person.PersonGuid.ToString(),
						null);
					break;
				case PersonFluffyDateType.Death:
					_personSearchIndex.MergeYearOfDeath(
						fluffyDate.Person.PersonGuid.ToString(),
						null);
					break;
			}

			// Update age

			// Get updated dates
			var person = _personData.ReadPersonWithFluffyDates(fluffyDate.Person.PersonGuid);

			var birthFluffyDate =
				person.FluffyDates.FirstOrDefault(x =>
					x.Type == PersonFluffyDateType.Birth);

			if (birthFluffyDate?.Year != null)
			{
				var fluffyDateStart =
					new FluffyDate(
						birthFluffyDate.Year,
						birthFluffyDate.Month,
						birthFluffyDate.Day);

				var fluffyDateEnd =
					new FluffyDate(
						DateTime.Now.Year,
						DateTime.Now.Month,
						DateTime.Now.Day);

				var deathFluffyDate =
					person.FluffyDates.FirstOrDefault(x =>
						x.Type == PersonFluffyDateType.Death);

				if (deathFluffyDate?.Year != null)
				{
					fluffyDateEnd =
						new FluffyDate(
							deathFluffyDate.Year,
							deathFluffyDate.Month,
							deathFluffyDate.Day);
				}

				var age =
					new Age(
						fluffyDateStart,
						fluffyDateEnd);

				if (age.InYears != null)
				{
					_personSearchIndex.MergeAge(
						person.PersonGuid.ToString(),
						age.InYears);
				}
			}
			else
			{
				_personSearchIndex.MergeAge(
					person.PersonGuid.ToString(),
					null);
			}

			//
			// Redirect
			//

			return RedirectToAction(
				"Details",
				"Person",
				new { personGuid = fluffyDate.Person.PersonGuid });
		}
	}
}