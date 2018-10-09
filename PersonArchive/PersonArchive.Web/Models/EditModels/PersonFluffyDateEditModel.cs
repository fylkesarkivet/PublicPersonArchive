using PersonArchive.Web.Models.Helpers;

namespace PersonArchive.Web.Models.EditModels
{
	public class PersonFluffyDateEditModel
	{
		public int? Year { get; set; }
		public int? Month { get; set; }
		public int? Day { get; set; }
		public PersonFluffyDateTypeOptions Type { get; set; }
	}
}