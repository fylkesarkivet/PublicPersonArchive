using System.Collections.Generic;
using System.Linq;
using PersonArchive.Entities.SearchIndex;

namespace PersonArchive.Web.Models.ViewModels.Search
{
	public class PersonItemInIndexViewModel
	{
		public PersonItemInIndexViewModel(
			PersonDocument personDocument)
		{
			PersonGuid = personDocument.PersonGuid;
			Gender = personDocument.Gender;

			if (personDocument.Names.Any())
				Names = personDocument.Names;
			
			if (personDocument.YearOfBirth != null)
				YearOfBirth = personDocument.YearOfBirth.ToString();

			if (personDocument.YearOfDeath != null)
				YearOfDeath = personDocument.YearOfDeath.ToString();

			if (personDocument.Age != null)
				Age = personDocument.Age.ToString();
		}

		public string PersonGuid { get; }
		public string Gender { get; }
		public List<string> Names { get; set; } = new List<string>();
		public string YearOfBirth { get; set; }
		public string YearOfDeath { get; set; }
		public string Age { get; set; }
	}
}