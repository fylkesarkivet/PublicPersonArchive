using System.ComponentModel.DataAnnotations;

namespace PersonArchive.Web.Models.Helpers
{
	public enum PersonDescriptionTypeOptions
	{
		[Display(Name = "Norsk")]
		Norwegian = 1,

		[Display(Name = "Engelsk")]
		English = 2
	}
}