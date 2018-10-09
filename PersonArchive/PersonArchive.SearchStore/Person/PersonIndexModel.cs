using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace PersonArchive.SearchStore.Person
{
	[SerializePropertyNamesAsCamelCase]
	public class PersonIndexModel
	{
		[Key]
		// Key is always [IsRetrievable(true)]
		public string PersonGuid { get; set; }

		[IsRetrievable(true)]
		[IsFacetable]
		[IsFilterable]
		public string Gender { get; set; }

		[IsRetrievable(true)]
		[IsSearchable]
		public string[] Names { get; set; }

		[IsRetrievable(true)]
		//[IsFacetable]
		[IsFilterable]
		public int? YearOfBirth { get; set; }

		[IsRetrievable(true)]
		//[IsFacetable]
		[IsFilterable]
		public int? YearOfDeath { get; set; }

		[IsRetrievable(true)]
		//[IsFacetable]
		[IsFilterable]
		public int? Age { get; set; }
	}
}