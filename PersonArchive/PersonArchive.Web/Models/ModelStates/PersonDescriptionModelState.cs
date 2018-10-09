using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PersonArchive.Entities.PersonDbContext;
using PersonArchive.Web.Models.Helpers;
using PersonDescription = PersonArchive.Logic.Validate.PersonDescription;

namespace PersonArchive.Web.Models.ModelStates
{
	public class PersonDescriptionModelState
	{
		public ModelStateDictionary ModelState;
		private readonly PersonDescription _selectedPersonDescription;
		private readonly PersonDescriptionTypeOptions _selectedDescriptionType;
		private readonly IEnumerable<Entities.PersonDbContext.PersonDescription> _allPersonDescriptions;
		private readonly int? _personDescriptionId;

		public PersonDescriptionModelState(
			ModelStateDictionary modelState,
			PersonDescription selectedPersonDescription,
			PersonDescriptionTypeOptions selectedDescriptionType,
			IEnumerable<Entities.PersonDbContext.PersonDescription> allPersonDescriptions,
			int? personDescriptionId)
		{
			ModelState = modelState;
			_selectedPersonDescription = selectedPersonDescription;
			_selectedDescriptionType = selectedDescriptionType;
			_allPersonDescriptions = allPersonDescriptions;
			_personDescriptionId = personDescriptionId;
		}

		public void HasValidValues()
		{
			// We must have a text
			if (string.IsNullOrWhiteSpace(_selectedPersonDescription.Text))
				ModelState.AddModelError("Text", "Ein beskrivelse må ha tekst.");

			// TODO Text must have an language
			// TODO get english error message if input is 0. Possible solution to add 0 value as Undefined to enum. Havent tried it yet.
//			if (_selectedDescriptionType == 0)
//				ModelState.AddModelError("Type", "Teksten må ha eit språk.");
		}

		public void HasUniqueTypeToBeCreated()
		{
			if (_allPersonDescriptions.Any(x =>
				x.Type == (PersonDescriptionType)_selectedDescriptionType))
			{
				ModelState.AddModelError(
					"Type",
					"Det er alt registrert ein verdi av denne typen.");
			}
		}

		public void HasUniqueTypeToBeEdited()
		{
			if (_personDescriptionId != null &&
				_allPersonDescriptions.Any(x =>
					x.Type == (PersonDescriptionType) _selectedDescriptionType &&
					x.PersonDescriptionId != _personDescriptionId))
			{
				ModelState.AddModelError(
					"Type",
					"Det er alt registrert ein verdi av denne typen.");
			}
		}
	}
}