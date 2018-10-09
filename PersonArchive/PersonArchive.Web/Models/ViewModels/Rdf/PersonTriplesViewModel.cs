using System;

namespace PersonArchive.Web.Models.ViewModels.Rdf
{
	public class PersonTriplesViewModel
	{
		public Guid PersonGuid { get; set; }
		public string TriplesAsText { get; set; }
	}
}