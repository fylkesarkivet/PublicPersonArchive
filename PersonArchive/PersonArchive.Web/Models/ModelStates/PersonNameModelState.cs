using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PersonArchive.Logic.Validate;

namespace PersonArchive.Web.Models.ModelStates
{
	public class PersonNameModelState
	{
		public ModelStateDictionary ModelState;

		private readonly PersonName _selectedPersonName;
		private readonly IEnumerable<Entities.PersonDbContext.PersonName> _allPersonNames;
		private readonly int? _personNameId;

		public PersonNameModelState(
			ModelStateDictionary modelState,
			PersonName selectedPersonName,
			IEnumerable<Entities.PersonDbContext.PersonName> allPersonNames,
			int? personNameId)
		{
			ModelState = modelState;
			_selectedPersonName = selectedPersonName;
			_allPersonNames = allPersonNames;
			_personNameId = personNameId;
		}

		public void HasValidValues()
		{
			// Name must have text
			if (string.IsNullOrWhiteSpace(_selectedPersonName.First) &&
				string.IsNullOrWhiteSpace(_selectedPersonName.Middle) &&
				string.IsNullOrWhiteSpace(_selectedPersonName.Last))
			{
				const string message = "Namn utan tekst støttast ikkje.";
				ModelState.AddModelError("First", message);
				ModelState.AddModelError("Middle", message);
				ModelState.AddModelError("Last", message);
			}

			// Name can't just be the middle name
			if (string.IsNullOrWhiteSpace(_selectedPersonName.First) &&
				string.IsNullOrWhiteSpace(_selectedPersonName.Last) &&
				!string.IsNullOrWhiteSpace(_selectedPersonName.Middle))
			{
				ModelState.AddModelError(
					"Middle",
					"Må ha fornamn eller etternamn om ein skal registrere mellomnamn.");
			}
		}

		public void HasUniquePersonNameToBeCreated()
		{
			if (!_allPersonNames.Any(x =>
				x.Prefix == _selectedPersonName.Prefix &&
				x.First == _selectedPersonName.First &&
				x.Middle == _selectedPersonName.Middle &&
				x.Last == _selectedPersonName.Last &&
				x.Suffix == _selectedPersonName.Suffix)) return;

			const string message = "Namnet er alt registrert.";
			ModelState.AddModelError("Prefix", message);
			ModelState.AddModelError("First", message);
			ModelState.AddModelError("Middle", message);
			ModelState.AddModelError("Last", message);
			ModelState.AddModelError("Suffix", message);
		}

		public void HasUniquePersonNameToBeEdited()
		{
			if (_personNameId == null || 
				!_allPersonNames.Any(x =>
					x.Prefix == _selectedPersonName.Prefix &&
					x.First == _selectedPersonName.First &&
					x.Middle == _selectedPersonName.Middle &&
					x.Last == _selectedPersonName.Last &&
					x.Suffix == _selectedPersonName.Suffix &&
					x.PersonNameId != _personNameId)) return;

			const string message = "Namnet er alt registrert.";
			ModelState.AddModelError("Prefix", message);
			ModelState.AddModelError("First", message);
			ModelState.AddModelError("Middle", message);
			ModelState.AddModelError("Last", message);
			ModelState.AddModelError("Suffix", message);
		}

		public void NameWeightHasValidNumber()
		{
			if (_selectedPersonName.NameWeight == null ||
				_selectedPersonName.NameWeight <= _allPersonNames.Count() &&
				_selectedPersonName.NameWeight > 0)
			{
				return;
			}

			ModelState.AddModelError(
				"NameWeight", 
				"Vekting lyt vere eit nummer som ikkje overgår talet på namn i lista.");
		}
	}
}