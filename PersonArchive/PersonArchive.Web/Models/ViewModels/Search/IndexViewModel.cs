using System.Collections.Generic;
using System.Linq;
using PersonArchive.Entities.SearchIndex;
using PersonArchive.Web.Models.SearchModels;

namespace PersonArchive.Web.Models.ViewModels.Search
{
	public class IndexViewModel
	{
		//
		// Search input from user
		//

		public string s => SearchText;
		public string f => FilterText;

		public string SearchText { get; private set; }
		public string FilterText { get; private set; }

		// Helpers
		public bool DisplaySearchText => !string.IsNullOrWhiteSpace(SearchText);
		public bool DisplayFilterText => !string.IsNullOrWhiteSpace(FilterText);

		//
		// Facets
		//

		public List<FacetItemInIndexViewModel> FacetItemsInSearchResult { get; }
			= new List<FacetItemInIndexViewModel>();

		// Helpers
		public bool DisplayFacetItemsWithLinks => string.IsNullOrWhiteSpace(FilterText);

		//
		// Search results
		//

		public int NumberOfItemsInSearchResult { get; private set; }
		public int NumberOfItemsOnThisPage { get; private set; }
		public List<PersonItemInIndexViewModel> PersonItemsInSearchResult { get; }
			= new List<PersonItemInIndexViewModel>();

		// Helpers
		public bool DisplaySearchResult => PersonItemsInSearchResult.Any();

		//
		// Add user search input to ViewModel
		//

		public void AddSearchInputFromUser(SearchInputModel searchInputModel)
		{
			SearchText = searchInputModel.s;
			FilterText = searchInputModel.f;
		}

		// Helpers
		public bool SearchHaveBeenConducted =>
			!string.IsNullOrWhiteSpace(SearchText) ||
			!string.IsNullOrWhiteSpace(FilterText);

		//
		// Adding search result to ViewModel
		//

		public void AddPersonSearchResult(PersonSearchResult personSearchResult)
		{
			NumberOfItemsInSearchResult = personSearchResult.NumberOfItemsInSearchResult;
			NumberOfItemsOnThisPage = personSearchResult.NumberOfItemsOnThisPage;

			// Add persons
			foreach (var personDocument in personSearchResult.PersonDocuments)
			{
				PersonItemsInSearchResult.Add(
					new PersonItemInIndexViewModel(personDocument));
			}

			// Add facets
			foreach (var searchFacetItem in personSearchResult.SearchFacetItems)
			{
				FacetItemsInSearchResult.Add(
					new FacetItemInIndexViewModel(
						searchFacetItem, 
						FilterText, 
						DisplayFacetItemsWithLinks));
			}
		}
	}
}