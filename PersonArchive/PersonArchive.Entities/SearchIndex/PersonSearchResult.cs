using System.Collections.Generic;

namespace PersonArchive.Entities.SearchIndex
{
	public class PersonSearchResult
	{
		//
		// Search results
		//

		public List<PersonDocument> PersonDocuments { get; set; } = 
			new List<PersonDocument>();
		public int NumberOfItemsInSearchResult { get; set; } = 0;
		public int NumberOfItemsOnThisPage { get; set; } = 0;

		//
		// Facets
		//

		public List<SearchFacetItem> SearchFacetItems { get; set; } = 
			new List<SearchFacetItem>();
	}
}