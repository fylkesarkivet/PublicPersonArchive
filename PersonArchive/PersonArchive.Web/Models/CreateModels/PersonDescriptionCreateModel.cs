using PersonArchive.Web.Models.Helpers;

namespace PersonArchive.Web.Models.CreateModels
{
	public class PersonDescriptionCreateModel
	{
		public string Text { get; set; }
		public PersonDescriptionTypeOptions Type { get; set; }
	}
}