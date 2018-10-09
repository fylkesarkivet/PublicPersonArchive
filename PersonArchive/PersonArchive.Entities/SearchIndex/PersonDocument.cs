using System.Collections.Generic;

namespace PersonArchive.Entities.SearchIndex
{
	public class PersonDocument
	{
		public string PersonGuid { get; set; }
		public string Gender { get; set; }
		public List<string> Names { get; set; }
			= new List<string>();
		public int? YearOfBirth { get; set; }
		public int? YearOfDeath { get; set; }
		public int? Age { get; set; }
	}
}