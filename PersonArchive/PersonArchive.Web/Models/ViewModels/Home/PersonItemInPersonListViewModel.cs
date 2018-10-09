using System.Collections.Generic;
using System.Linq;
using PersonArchive.Entities.PersonDbContext;

namespace PersonArchive.Web.Models.ViewModels.Home
{
	public class PersonItemInPersonListViewModel
	{
		public PersonItemInPersonListViewModel(
			Person person)
		{
			PersonGuid = person.PersonGuid.ToString();
			Gender = person.Gender.ToString();

			if (person.Names.Any())
			{
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

					Names.Add(fullName);
				}
			}

			if (person.FluffyDates.Any())
			{
				var birth = 
					person.FluffyDates.FirstOrDefault(x => 
						x.Type == PersonFluffyDateType.Birth);

				if (birth?.Year != null)
					YearOfBirth = birth.Year.ToString();

				var death = 
					person.FluffyDates.FirstOrDefault(x => 
						x.Type == PersonFluffyDateType.Death);

				if (death?.Year != null)
					YearOfDeath = death.Year.ToString();
			}
		}

		public string PersonGuid { get; }
		public string Gender { get; }
		public List<string> Names { get; set; } = new List<string>();
		public string YearOfBirth { get; set; }
		public string YearOfDeath { get; set; }
		public string Age { get; set; }
	}
}