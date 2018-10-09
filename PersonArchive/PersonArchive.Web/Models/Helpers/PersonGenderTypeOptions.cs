using System.ComponentModel.DataAnnotations;

namespace PersonArchive.Web.Models.Helpers
{
	public enum PersonGenderTypeOptions
	{
		[Display(Name = "Han")]
		Male = 1,

		[Display(Name = "Ho")]
		Female = 2,

		[Display(Name = "Variant")]
		GenderVariant = 3,

		[Display(Name = "Udefinert")]
		Undefined = 4
	}
}