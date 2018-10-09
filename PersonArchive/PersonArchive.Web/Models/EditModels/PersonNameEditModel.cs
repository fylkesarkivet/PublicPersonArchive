namespace PersonArchive.Web.Models.EditModels
{
	public class PersonNameEditModel
	{
		public string Prefix { get; set; }
		public string First { get; set; }
		public string Middle { get; set; }
		public string Last { get; set; }
		public string Suffix { get; set; }
		public int NameWeight { get; set; }

		//
		// Helpers
		//

		public int NameCount { get; set; } = 1; // default value
	}
}