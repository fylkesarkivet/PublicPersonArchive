using PersonArchive.Entities.SearchIndex;

namespace PersonArchive.Web.Models.ViewModels.Search
{
	public class FacetItemInIndexViewModel
	{
		public FacetItemInIndexViewModel(
			SearchFacetItem searchFacetItem,
			string filterText,
			bool displayFacetItemsWithLinks)
		{
			Type = searchFacetItem.Name;
			Count = searchFacetItem.Count;
			FilterText = filterText;
			DisplayWithLink = displayFacetItemsWithLinks;
		}

		public string Type { get; }
		public int Count { get; }

		private string FilterText { get; }

		public string Label
		{
			get
			{
				switch (Type)
				{
					case "Male":
						return "Han";
					case "Female":
						return "Ho";
					case "GenderVariant":
						return "Variant";
					case "Undefined":
						return "Udefinert";
					default:
						return Type;
				}
			}
		}

		public bool SearchIsFilteredAfterThisFacet
		{
			get
			{
				var facetType = $"'{Type}'";

				return 
					!string.IsNullOrWhiteSpace(FilterText) &&
					FilterText.Contains("gender") &&
					FilterText.Contains(facetType);
			}
		}

		public bool DisplayWithLink { get; }
	}
}