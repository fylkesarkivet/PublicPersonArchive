using PersonArchive.Web.Models.Helpers;

namespace PersonArchive.Web.Models.EditModels
{
	public class PersonDescriptionEditModel
	{
		public string Text { get; set; }
		public PersonDescriptionTypeOptions Type { get; set; }
	}
}