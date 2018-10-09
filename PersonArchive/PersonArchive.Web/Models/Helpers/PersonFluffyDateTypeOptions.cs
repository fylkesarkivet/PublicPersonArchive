using System.ComponentModel.DataAnnotations;

namespace PersonArchive.Web.Models.Helpers
{
	public enum PersonFluffyDateTypeOptions
	{
		[Display(Name = "Fødd")]
		Birth = 1,

		[Display(Name = "Død")]
		Death = 2
	}
}