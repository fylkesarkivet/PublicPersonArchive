using System;
using System.Collections.Generic;
using System.Linq;
using PersonArchive.Entities.PersonDbContext;
using PersonArchive.Logic.Validate;
using PersonArchive.Web.Models.ViewModels.Home;

namespace PersonArchive.Web.Models.ViewModels
{
	public class PersonListViewModel
	{
		public List<PersonItemInPersonListViewModel> PersonItems { get; }
			= new List<PersonItemInPersonListViewModel>();

		// Helpers
		public bool DisplayPersonItems => PersonItems.Any();

		//
		// Add persons to view model
		//

		public void AddPersonsToViewModel(List<Person> persons)
		{
			foreach (var person in persons)
			{
				var personItem = new PersonItemInPersonListViewModel(person);

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
						personItem.Age = age.InYears.ToString();
				}

				PersonItems.Add(personItem);
			}
		}
	}
}