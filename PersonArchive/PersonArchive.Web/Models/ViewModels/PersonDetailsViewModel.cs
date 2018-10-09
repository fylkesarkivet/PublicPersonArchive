using System;
using System.Collections.Generic;
using System.Linq;
using PersonArchive.Entities.PersonDbContext;
using PersonArchive.Logic.Validate;

namespace PersonArchive.Web.Models.ViewModels
{
	public class PersonDetailsViewModel
	{
		public Person Person { get; }

		public PersonDetailsViewModel(Person person)
		{
			Person = person;
		}

		public string DisplayGender
		{
			get
			{
				switch (Person.Gender)
				{
					case PersonGender.Male:
						return "Han";
					case PersonGender.Female:
						return "Ho";
					case PersonGender.GenderVariant:
						return "Variant";
					default:
						return "Udefinert";
				}
			}
		}

		public List<DisplayFluffyDate> DisplayFluffyDates
		{
			get
			{
				if(!Person.FluffyDates.Any())
					return new List<DisplayFluffyDate>();

				var lastItemInList = Person.FluffyDates.Last();

				return Person.FluffyDates.Select(personFluffyDate => 
					new DisplayFluffyDate(
						personFluffyDate.PersonFluffyDateId,
						personFluffyDate.Year,
						personFluffyDate.Month,
						personFluffyDate.Day,
						personFluffyDate.Type)
					{
						LastItemInList = 
							personFluffyDate.Equals(lastItemInList)
					}).ToList();
			}
		}

		public List<DisplayPersonDescription> DisplayPersonDescriptions
		{
			get
			{
				if (!Person.Descriptions.Any())
					return new List<DisplayPersonDescription>();

				var descriptions =
					Person.Descriptions
						.OrderBy(x => x.Type)
						.Select(personDescription =>
							new DisplayPersonDescription(
								personDescription)).ToList();

				descriptions.Last().LastItemInList = true;

				return descriptions;
			}
		}

		public List<DisplayPersonName> DisplayPersonNames
		{
			get
			{
				var items = Person.Names.OrderByDescending(x => x.NameWeight).ToList();

				if (!items.Any())
					return new List<DisplayPersonName>();

				var lastItemInList = items.Last();

				return items.Select(personName =>
					new DisplayPersonName(personName)
					{
						LastItemInList = personName.Equals(lastItemInList)
					}).ToList();
			}
		}

		//
		// Helpers
		//

		// At this time it is only possible to add birth and death dates.
		public bool ItIsPossibleToAddMoreFluffyDates => 
			DisplayFluffyDates.Count < 2;

		// At this time it is only possible to add norwegian and english description.
		public bool ItIsPossibleToAddMoreDescriptions => 
			DisplayPersonDescriptions.Count < 2;

		// Age
		public string Age
		{
			get
			{
				var birthFluffyDate =
					Person.FluffyDates.FirstOrDefault(x =>
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
						Person.FluffyDates.FirstOrDefault(x =>
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

					return age.InYears.ToString();
				}

				return string.Empty;
			}
		}

		public bool DisplayAge =>
			!string.IsNullOrWhiteSpace(Age);

		public string FluffyDateTitle =>
			DisplayAge ? $"Levetid: {Age} år" : "Levetid";
	}
}