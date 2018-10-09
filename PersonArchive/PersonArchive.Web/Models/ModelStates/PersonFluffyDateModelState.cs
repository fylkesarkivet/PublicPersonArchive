using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PersonArchive.Entities.PersonDbContext;
using PersonArchive.Logic.Validate;
using PersonArchive.Web.Models.Helpers;

namespace PersonArchive.Web.Models.ModelStates
{
	public class PersonFluffyDateModelState
	{
		public ModelStateDictionary ModelState;

		private readonly FluffyDate _selectedFluffyDate;
		private readonly PersonFluffyDateTypeOptions _selectedFluffyDateType;
		private readonly IEnumerable<PersonFluffyDate> _allFluffyDates;
		private readonly int? _personFluffyDateId;

		private readonly BirthVsDeathDate _birthVsDeathDate;
		private readonly FluffyDate _birthFluffyDate = new FluffyDate(null, null, null);
		private readonly FluffyDate _deathFluffyDate = new FluffyDate(null, null, null);

		public PersonFluffyDateModelState(
			ModelStateDictionary modelState,
			FluffyDate selectedFluffyDate,
			PersonFluffyDateTypeOptions selectedFluffyDateType,
			IEnumerable<PersonFluffyDate> allFluffyDates,
			int? personFluffyDateId)
		{
			ModelState = modelState;
			_selectedFluffyDate = selectedFluffyDate;
			_selectedFluffyDateType = selectedFluffyDateType;
			_allFluffyDates = allFluffyDates;
			_personFluffyDateId = personFluffyDateId;

			switch (_selectedFluffyDateType)
			{
				case PersonFluffyDateTypeOptions.Birth:

					_birthFluffyDate = _selectedFluffyDate;

					var death =
						_allFluffyDates
							.FirstOrDefault(x =>
								x.Type.Equals(PersonFluffyDateType.Death));

					if (death != null)
						_deathFluffyDate =
							new FluffyDate(
								death.Year,
								death.Month,
								death.Day);

					break;

				case PersonFluffyDateTypeOptions.Death:

					_deathFluffyDate = _selectedFluffyDate;

					var birth =
						_allFluffyDates
							.FirstOrDefault(x =>
								x.Type.Equals(PersonFluffyDateType.Birth));

					if (birth != null)
						_birthFluffyDate =
							new FluffyDate(
								birth.Year,
								birth.Month,
								birth.Day);

					break;
			}

			_birthVsDeathDate =
				new BirthVsDeathDate(_birthFluffyDate, _deathFluffyDate);
		}

		public void HasValidValues()
		{
			// Year - Not null
			if (_selectedFluffyDate.Year != null && !_selectedFluffyDate.HasValidYear)
				ModelState.AddModelError("Year", "Denne verdien er ugyldig.");

			// Year - Null (When no date)
			// All dates must have at least a year,
			// except for death type,
			// a person can in this way be marked as dead
			// (without knowing the time).
			if (_selectedFluffyDateType != PersonFluffyDateTypeOptions.Death &&
				_selectedFluffyDate.Year == null)
				ModelState.AddModelError(
					"Year", "Dato må ha mist eit årstal.");

			// Month
			if (_selectedFluffyDate.Month != null && !_selectedFluffyDate.HasValidMonth)
				ModelState.AddModelError("Month", "Denne verdien er ugyldig.");

			// Day
			if (_selectedFluffyDate.Day != null && !_selectedFluffyDate.HasValidDay)
				ModelState.AddModelError("Day", "Denne verdien er ugyldig.");

			// Complete date
			if (_selectedFluffyDate.HasValidYear &&
			    _selectedFluffyDate.HasValidMonth &&
			    _selectedFluffyDate.HasValidDay &&
			    !_selectedFluffyDate.IsValidDate)
			{
				const string message = "Dette er ikkje ein gyldig dato.";
				ModelState.AddModelError("Year", message);
				ModelState.AddModelError("Month", message);
				ModelState.AddModelError("Day", message);
			}

			//
			// If the date is too fluffy
			//

			if (!_selectedFluffyDate.DayCriteriaOk)
				ModelState.AddModelError(
					"Day",
					"År og månad lyt vere på plass om ein skal legge til dag.");

			if (!_selectedFluffyDate.MonthCriteriaOk)
				ModelState.AddModelError(
					"Month",
					"År lyt vere på plass om ein skal legge til månad.");
		}

		public void BirthVsDeathYear()
		{
			if (_birthFluffyDate.HasValidYear &&
			    _deathFluffyDate.HasValidYear &&
			    !_birthVsDeathDate.HasValidYears)
			{
				ModelState.AddModelError(
					"Year",
					$"År fødd ({_birthFluffyDate.Year}) vs " +
					$"år død ({_deathFluffyDate.Year}) gir ikkje meining.");
			}
		}

		public void BirthVsDeathMonth()
		{
			if (_birthFluffyDate.HasValidYear &&
			    _birthFluffyDate.HasValidMonth &&
			    _deathFluffyDate.HasValidYear &&
			    _deathFluffyDate.HasValidMonth &&
			    !_birthVsDeathDate.HasValidMonths)
			{
				ModelState.AddModelError(
					"Month",
					$"År ({_birthFluffyDate.Year}) " +
					$"og månad fødd ({_birthFluffyDate.Month}) vs " +
					$"år ({_deathFluffyDate.Year}) " +
					$"og månad død ({_deathFluffyDate.Month}) gir ikkje meining.");
			}
		}

		public void BirthVsDeathDay()
		{
			if (_birthFluffyDate.IsValidDate &&
			    _deathFluffyDate.IsValidDate &&
			    !_birthVsDeathDate.HasValidDays)
			{
				ModelState.AddModelError(
					"Day",
					"Fødselsdag og dødsdag lyt stå i forhold til kvarandre.");
			}
		}

		public void PersonHasNormalAge()
		{
			var age =
				new Age(_birthFluffyDate, _deathFluffyDate);

			if (age.InYears != null &&
			    age.InYears >= 120)
			{
				ModelState.AddModelError(
					"Year",
					$"Er denne personen {age.InYears} år gamal? No tek du litt hardt i...");
			}
		}

		public void DateIsNotInTheFuture(
			int currentYear,
			int currentMonth,
			int currentDay)
		{
			var time =
				new Time(
					_selectedFluffyDate,
					new FluffyDate(
						currentYear,
						currentMonth,
						currentDay));

			if (time.CompareDateIsInTheFuture == null || 
			    time.CompareDateIsInTheFuture != true)
				return;

			const string message = "Framtidige datoar støttast ikkje.";
			ModelState.AddModelError("Year", message);
			ModelState.AddModelError("Month", message);
			ModelState.AddModelError("Day", message);
		}

		public void HasUniqueTypeToBeCreated()
		{
			if (_allFluffyDates.Any(x =>
				x.Type == (PersonFluffyDateType)_selectedFluffyDateType))
			{
				ModelState.AddModelError(
					"Type",
					"Det er alt registrert ein verdi av denne typen.");
			}
		}

		public void HasUniqueTypeToBeEdited()
		{
			if (_personFluffyDateId != null &&
				_allFluffyDates.Any(x =>
					x.Type == (PersonFluffyDateType)_selectedFluffyDateType &&
					x.PersonFluffyDateId != _personFluffyDateId))
			{
				ModelState.AddModelError(
					"Type",
					"Det er alt registrert ein verdi av denne typen.");
			}
		}
	}
}